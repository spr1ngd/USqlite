
using miniMVC.USqlite;
using UnityEngine;
using USqlite;

namespace miniMVC
{
    [TableAttribute("Point")]
    public class Point
    {
        [PrimaryKey]
        [Column("ID")] public int ID;
        [Column("POSITION")] public string POSITION;
        [USqliteSerialize] public Vector3 SCALE;
        [USqliteSerialize] public Vector3 ROTATION;
        [Column] public string tower;
        [Column] public string building;
        [Column] public string room;
        [Column] public string level;
        [Column] public string sys;
        /*[Column]*/ public int floor;
        [USqliteSerialize] public Quaternion quaternion;

        public Point()
        {

        }

        public override string ToString()
        {
            //return string.Format("ID : [{0}] ,POSITION : [{1}] ,SCALE : [{2}] ,ROTATION : [{3}]",ID,POSITION,SCALE,ROTATION);
            return string.Format("ID : [{0}] ,POSITION : [{1}] ,SCALE : [{2}] ,ROTATION : [{3}],tower:{4}",ID,POSITION,SCALE,ROTATION,tower);
        }
    }
}