
using System;
using System.Reflection;

namespace USqlite
{
    public static class AttributeHelper
    {
        /// <summary>
        /// 获取对象已标注的指定特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(object obj) where T : Attribute
        {
            var type = obj.GetType();
            var atts = type.GetCustomAttributes(typeof(T),true);
            if (atts.Length <= 0)
            {
                //UnityEngine.DebugLogger.LogWarning(obj + "没有标注特性 " + typeof(T));
                return null;
            }
            T att = atts[0] as T;
            return att;
        }

        /// <summary>
        /// 拓展方法：获取类型标注的指定特性 ， 多次标注只取第一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T AttributeOf<T>(this object obj) where T : Attribute
        {
            return GetAttribute<T>(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T AttributeOf<T>(this Type type) where T : Attribute
        {
            var atts = type.GetCustomAttributes(typeof(T),true);
            if(atts.Length <= 0)
            {
                //UnityEngine.DebugLogger.LogWarning(obj + "没有标注特性 " + typeof(T));
                return null;
            }
            T att = atts[0] as T;
            return att;
        }

        public static T AttributeOf<T>(this PropertyInfo property) where T : Attribute
        {
            var atts = property.GetCustomAttributes(typeof(T), true);
            if(atts.Length <= 0)
            {
                //UnityEngine.DebugLogger.LogWarning(obj + "没有标注特性 " + typeof(T));
                return null;
            }
            T att = atts[0] as T;
            return att;
        }

        public static T AttributeOf<T>(this FieldInfo fieldInfo) where T : Attribute
        {
            var atts = fieldInfo.GetCustomAttributes(typeof(T),true);
            if(atts.Length <= 0)
            {
                //UnityEngine.DebugLogger.LogWarning(obj + "没有标注特性 " + typeof(T));
                return null;
            }
            T att = atts[0] as T;
            return att;
        }
    }
}