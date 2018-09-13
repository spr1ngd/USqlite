
// ****************************************************************************
// 文件名称(File Name):			JsonHelper.cs
//
// 功能描述(Description):		Json工具类 添加反序列化更快的fastJson ，默认反序列化用fastJson,序列化用Litjson
//                              使用时请将Litjson.dll文件和fastJson文件导入Assets/Plugins
//                              ** 注意若没有库文件，请注释掉代码顶部的对应的预编译指令，脚本会自动更具动态链接库选择合适的方案
//
// 作者(Author): 				springdong
//
// 日期(Create Date): 			2018.08.02
//
// 修改记录(Revision History):	
//                              ++: 新增对UnityEngine.Vector3和UnityEngine.Quaternion对象的序列化与反序列化的支持
// ****************************************************************************

//#define DEBUG_DUARTION
#define JSONTOOL_LITJSON
#define JSONTOOL_FASTJSON

using System;
using UnityEngine;

namespace miniMVC
{
    public enum E_JsonTool : int
    {
        None = -1,
        LitJson = 0,
        FastJson = 1
    }

    #region JsonHelper

    /// <summary>
    /// Json序列化/反序列化工具类
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonLoader jsonLoader =
#if (UNITY_IPHONE || ! JSONTOOL_FASTJSON) && JSONTOOL_LITJSON
            new JsonLoader(E_JsonTool.LitJson);
#else
#if JSONTOOL_FASTJSON
            new JsonLoader(E_JsonTool.FastJson);
#else
            new JsonLoader(E_JsonTool.None);
#endif
#endif
        private static readonly JsonWriter jsonWriter = new JsonWriter(E_JsonTool.LitJson);

        public static T ToObject<T>(string json) where T : class, new()
        {
            return jsonLoader.ToObject<T>(json);
        }

        public static T ToObject<T>(string json,Action callback) where T : class, new()
        {
            return jsonLoader.ToObject<T>(json,callback);
        }

        public static T ToObject<T>(string json,Action<T> callback) where T : class, new()
        {
            return jsonLoader.ToObject<T>(json,callback);
        }

        public static string ToJson(object jsonData)
        {
            return jsonWriter.ToJson(jsonData);
        }

        public static string ToJson(object jsonData,Action callback)
        {
            return jsonWriter.ToJson(jsonData,callback);
        }

        public static string ToJson(object jsonData,Action<string> callback)
        {
            return jsonWriter.ToJson(jsonData,callback);
        }
    }

    #endregion

    #region JsonLoader

    internal class JsonLoader
    {
        private readonly E_JsonTool m_jsonTool;

        public JsonLoader(E_JsonTool jsonTool = E_JsonTool.LitJson)
        {
            this.m_jsonTool = jsonTool;
            switch(jsonTool)
            {
#if JSONTOOL_LITJSON
                case E_JsonTool.LitJson:
                    LitJson.JsonMapper.RegisterImporter<string,Vector3>(Vector3ImporterFunc);
                    LitJson.JsonMapper.RegisterImporter<string,Quaternion>(Quaternion3ImporterFunc);
                    break;
#endif
#if JSONTOOL_FASTJSON
                case E_JsonTool.FastJson:
                    fastJSON.JSON.RegisterCustomType(typeof(Vector3),
                        jsonData => {
                            Vector3 obj = (Vector3)jsonData;
                            return obj.x + "," + obj.y + "," + obj.z;
                        },
                        jsonConent => Vector3ImporterFunc(jsonConent));
                    fastJSON.JSON.RegisterCustomType(typeof(Quaternion),
                        jsonData => {
                            Quaternion obj = (Quaternion)jsonData;
                            return obj.x + "," + obj.y + "," + obj.z + "," + obj.w;
                        },
                        jsonConent => Quaternion3ImporterFunc(jsonConent));
                    break;
#endif
            }
        }

        #region Helper Functions

        private Vector3 Vector3ImporterFunc(string input)
        {
            string[] array = input.Split(',');
            Vector3 value = new Vector3();
            value.x = float.Parse(array[0]);
            value.y = float.Parse(array[1]);
            value.z = float.Parse(array[2]);
            return value;
        }
        private Quaternion Quaternion3ImporterFunc(string input)
        {
            string[] array = input.Split(',');
            Quaternion value = new Quaternion();
            value.x = float.Parse(array[0]);
            value.y = float.Parse(array[1]);
            value.z = float.Parse(array[2]);
            value.w = float.Parse(array[3]);
            return value;
        }

        #endregion

        public T ToObject<T>(string json) where T : class, new()
        {
            T result = null;
#if DEBUG_DUARTION
            DateTime curTime = DateTime.Now;
#endif
            switch(m_jsonTool)
            {
#if JSONTOOL_LITJSON
                case E_JsonTool.LitJson:
                    result =  LitJson.JsonMapper.ToObject<T>(json);
                    break;
#endif
#if JSONTOOL_FASTJSON
                case E_JsonTool.FastJson:
                    result =  fastJSON.JSON.ToObject<T>(json);
                    break;
#endif
                default:
                    result = JsonUtility.FromJson<T>(json);
                    break;
            }
#if DEBUG_DUARTION
            var offsetTime = DateTime.Now - curTime;
            Debug.Log(string.Format("<color=green>{0} : 反序列化用时 [{1} ms] </color>",m_jsonTool.ToString(),offsetTime.Milliseconds));
#endif
            return result;
        }

        public T ToObject<T>(string json,Action callback) where T : class, new()
        {
            var result = ToObject<T>(json);
            if(null != callback)
                callback();
            return result;
        }

        public T ToObject<T>(string json,Action<T> callback) where T : class, new()
        {
            var result = ToObject<T>(json);
            if(null != callback)
                callback(result);
            return result;
        }
    }

    #endregion

    #region JsonWriter

    internal class JsonWriter
    {
        private readonly E_JsonTool m_jsonTool;

        public JsonWriter(E_JsonTool jsonTool = E_JsonTool.LitJson)
        {
            this.m_jsonTool = jsonTool;
            switch(jsonTool)
            {
#if JSONTOOL_LITJSON
                case E_JsonTool.LitJson:
                    LitJson.JsonMapper.RegisterExporter<Vector3>(Vector3ExporterFunc);
                    LitJson.JsonMapper.RegisterExporter<Quaternion>(QuaternionExporterFunc);
                    break;
#endif
            }
        }

        #region Helper Functions

        private void Vector3ExporterFunc(Vector3 obj,LitJson.JsonWriter writer)
        {
            writer.Write(obj.x + "," + obj.y + "," + obj.z);
        }
        private void QuaternionExporterFunc(Quaternion obj,LitJson.JsonWriter writer)
        {
            writer.Write(obj.x + "," + obj.y + "," + obj.z + "," + obj.w);
        }

        #endregion

        public string ToJson(object jsonData)
        {
            string result = null;
#if DEBUG_DUARTION
            DateTime curTime = DateTime.Now;
#endif
            switch(m_jsonTool)
            {
#if JSONTOOL_LITJSON
                case E_JsonTool.LitJson:
                    result =  LitJson.JsonMapper.ToJson(jsonData);
                    break;
#endif
#if JSONTOOL_FASTJSON
                case E_JsonTool.FastJson:
                    result = fastJSON.JSON.ToJSON(jsonData);
                    break;
#endif
                default:
                    result = UnityEngine.JsonUtility.ToJson(jsonData);
                    break;
            }
#if DEBUG_DUARTION
            var offsetTime = DateTime.Now - curTime;
            Debug.Log(string.Format("<color=red>{0} : 序列化用时 [{1} ms] </color>",m_jsonTool.ToString(),offsetTime.Milliseconds));
#endif
            return result;
        }

        public string ToJson(object jsonData,Action callback)
        {
            var result = ToJson(jsonData);
            if(null != callback)
                callback();
            return result;
        }

        public string ToJson(object jsonData,Action<string> callback)
        {
            var result = ToJson(jsonData);
            if(null != callback)
                callback(result);
            return result;
        }
    }

    #endregion
}