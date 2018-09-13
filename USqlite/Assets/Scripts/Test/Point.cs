
using System;
using System.Data;
using UnityEngine;
using USqlite;

namespace miniMVC
{
    [TableAttribute("PointMax")]
    public class Point
    {
        [AutoIncrement]
        [PrimaryKey]
        [Column("ID",DbType.Int64,true)] public Int64 ID;
        [Column("POSITION")] public string POSITION;
        [Column] public Vector3 SCALE;
        [Column] public Vector3 ROTATION;
        [Column("TOWER")] public string tower;
        [Column] public string building;
        [Column] public string room;
        [Column] public string level;
        [Column] public string sys;
        [Column("FLOOR",DbType.Int64,false)] public Int64 floor;
        [Column] public Quaternion quaternion;

        public Point()
        {

        }

        public override string ToString()
        {
            return string.Format("ID : [{0}] ,POSITION : [{1}] ,SCALE : [{2}] ,ROTATION : [{3}],tower:{4}",ID,POSITION,SCALE,ROTATION,tower);
        }
    }
}