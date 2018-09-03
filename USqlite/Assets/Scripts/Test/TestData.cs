
using miniMVC.USqlite;
using UnityEngine;
using USqlite;

namespace miniMVC
{
    public class Point
    {
        [PrimaryKey]
        [USqliteSerialize]
        public int ID;
        [USqliteSerialize] public string POSITION;
        [USqliteSerialize] public Vector3 SCALE;
        [USqliteSerialize] public Vector3 ROTATION;
        [USqliteSerialize] public string tower;
        [USqliteSerialize] public string building;
        [USqliteSerialize] public string room;
        [USqliteSerialize] public string level;
        [USqliteSerialize] public string sys;
        [USqliteSerialize] public int floor;
        [USqliteSerialize] public Quaternion quaternion;

        public override string ToString()
        {
            return string.Format("ID : [{0}] ,POSITION : [{1}] ,SCALE : [{2}] ,ROTATION : [{3}]",ID,POSITION,SCALE,ROTATION);
        }
    }
}