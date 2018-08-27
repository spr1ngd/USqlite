
using miniMVC;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

public class JsonSerializationToolsComparsion : Editor
{
    [MenuItem("Json Comparsion/Desrialize")]
    public static void Deserialize()
    {
        Debug.Log("<color=red>反序列化测试</color>");
        ReadTask();
    }

    private static void ReadTask()
    {
        IFileIO fileIO = new FileIO();
        fileIO.AddTask(new FileIOTask()
        {
            ioType = E_IOType.Read,
            filePath = FilePath.normalPath + "Json/ParkData.json",
            readCallback = content =>
            {
                var data = JsonHelper.ToObject<ParkData>(content);
                Debug.Log(data.buildings.Count);
            }
        });
    }

    [MenuItem("Json Comparsion/Serialize")]
    public static void Serialize()
    {
        Debug.Log("<color=green>反序列化测试</color>");
        WriteTask();
    }

    private static void WriteTask()
    {
        IFileIO fileIO = new FileIO();
        fileIO.AddTask(new FileIOTask()
        {
            ioType = E_IOType.Write,
            //content = GeneratorJsonContent(jsonTool),
            content = GeneratorUnityData(),
            filePath = FilePath.normalPath + "Json/ParkDataNew"+".json",
            writeCallback = () => { Debug.Log("*文件写出成功*"); }
        });
    }

    private static string GeneratorJsonContent()
    {
        ParkData parkData = new ParkData() { id = "苹果飞船总部" };
        AddObject(parkData);

        BuildingData firstBuilding = new BuildingData() { id = "办公楼" };
        AddChildren(firstBuilding);
    
        BuildingData secondBuilding = new BuildingData() { id = "行政楼" };
        AddChildren(secondBuilding);
     
        BuildingData thirdBuilding = new BuildingData() { id = "食堂" };
        AddChildren(thirdBuilding);

        AddObject(firstBuilding);
        AddObject(secondBuilding);
        AddObject(thirdBuilding);

        parkData.AddChild(firstBuilding);
        parkData.AddChild(secondBuilding);
        parkData.AddChild(thirdBuilding);
        
        string content = JsonHelper.ToJson(parkData);
        return content;
    }
    
    private static void AddChildren( ObjectData obj )
    {
        for(int i = 0; i < 1000; i++)
        {
            ObjectData objData = new ObjectData();
            var guid = System.Guid.NewGuid();
            objData.id = guid.ToString();
            objData.name = guid.ToString();
            obj.AddChild(objData);
        }
    }

    private static void AddObject(ObjectData obj)
    {
        for(int i = 0; i < 1000; i++)
        {
            ObjectData objData = new ObjectData();
            var guid = System.Guid.NewGuid();
            objData.id = guid.ToString();
            objData.name = guid.ToString();
            obj.AddObject(objData);
        }
    }

    private static string GeneratorUnityData()
    {
        List<UnityData> unityDatas = new List<UnityData>();
        for (int i = 0; i < 1500; i++)
        {
            UnityData untiyData = new UnityData();
            untiyData.scale = Vector3.up * UnityEngine.Random.Range(0,1);
            untiyData.position = Vector3.one * UnityEngine.Random.Range(0,1);
            untiyData.quaternion = Quaternion.identity;
            unityDatas.Add(untiyData);
        }
        string content = JsonHelper.ToJson(unityDatas);
        return content;
    }

    [Test]
    public static void DeserializeJson()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        IFileIO fileIO = new FileIO();
        fileIO.AddTask(new FileIOTask()
        {
            ioType = E_IOType.Read,
            filePath = FilePath.normalPath + "Json/TestJson.json",
            readCallback = content =>
            {
                var data = JsonHelper.ToObject<List<Point>>(content);
                watch.Stop();
                Debug.Log("Json反序列化"+ data .Count+ "条数据 耗时 : " + watch.Elapsed);
            }
        });
    }
    [Test]
    public static void SerializeObject()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        IList<Point> points = new List<Point>();
        for(int i = 0; i < 15000; i++)
        {
            points.Add(new Point()
            {
                ID = i,
                POSITION = Vector3.one.ToString(),
                //SCALE = Vector3.one,
                //ROTATION = Vector3.one
            });
        }
        string json = JsonHelper.ToJson(points);
        IFileIO fileIO = new FileIO();
        fileIO.AddTask(new FileIOTask()
        {
            ioType = E_IOType.Write,
            content = json,
            filePath = FilePath.normalPath + "Json/TestJson" + ".json",
            writeCallback = () =>
            {
                Debug.Log("*文件写出成功*");
                watch.Stop();
                Debug.Log("Json序列化15000条数据 耗时 : " + watch.Elapsed);
            }
        });
    }
}

public class UnityData
{
    public Vector3 scale;
    public Vector3 position;
    public Quaternion quaternion;
}

public class ObjectData
{
    public string name;
    public string id;
    public Dictionary<string,ObjectData> children = new Dictionary<string,ObjectData>();

    public virtual void AddChild(ObjectData obj)
    {
        this.children.Add(obj.id,obj);
    }

    public virtual void AddObject(ObjectData obj)
    {
        this.children.Add(obj.id,obj);
    }
}

public class ParkData : ObjectData
{
    public Dictionary<string, BuildingData> buildings = new Dictionary<string, BuildingData>();

    public override void AddChild(ObjectData obj)
    {
        BuildingData building = obj as BuildingData;
        
        if (null == building)
            return;
        buildings.Add(building.id,building);
    }
}

public class BuildingData : ObjectData
{
    public Dictionary<string, FloorData> floors = new Dictionary<string, FloorData>();

    public override void AddChild(ObjectData obj)
    {
        FloorData floor = obj as FloorData;
        if (null == floor)
            return;
        floors.Add(floor.id,floor);
    }
}

public class FloorData : ObjectData
{
    public Dictionary<string, RoomData> rooms = new Dictionary<string, RoomData>();

    public override void AddChild(ObjectData obj)
    {
        RoomData room = obj as RoomData;
        if(null == room)
            return;
        rooms.Add(room.id,room);
    }
}

public class RoomData : ObjectData
{
    public override void AddChild(ObjectData obj)
    {
        return;
        //throw new ArgumentNullException("Can not be add child.");
    }
}