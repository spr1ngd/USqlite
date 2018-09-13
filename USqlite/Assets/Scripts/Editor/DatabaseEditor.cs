
using USqlite;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;


namespace miniMVC.USqlite
{
    [TestFixture]
    public class DatabaseEditor : Editor
    {
        private const string TABLENAME = "TestDB.db";

        [Test(Author = "springdong",Description = "创建表")]
        public static void Alpha_1_USqliteCreate()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            Sqlite3.CreateTable<Point>();
            Sqlite3.Close();
        }

        [Test(Author = "springdong",Description = "删除表")]
        public static void Alpha_2_USqliteDrop()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            Sqlite3.DropTable<Point>();
            Sqlite3.Close();
        }

        [Test(Author = "springdong",Description = "查询表信息")]
        public static void Alpha_3_USqliteQuery()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            var points = Sqlite3.Table<Point>().Select("PointMax").Where(point => point.ID < 15000).Execute2List();
            for( int i = 0 ; i < points.Count; i++ )
                UnityEngine.Debug.Log(points[i]);
            Sqlite3.Close();
        }

        [Test(Author = "springdong",Description = "插入表信息")]
        public static void Alpha_4_USqliteInsert()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            IList<Point> points = new List<Point>();
            for (int i = 0; i < 15; i++)
            {
                Point point = new Point
                {
                    ID = i,
                    POSITION = "(1,1,1)",
                    building = "门诊楼"
                };
                points.Add(point);
             
            }
            Sqlite3.Table<Point>().Insert("PointMax",points);
            Sqlite3.Close();
        }

        [Test(Author = "springdong",Description = "更新表信息")]
        public static void Alpha_5_USqliteUpdate()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            Sqlite3.Table<Point>().Update("PointMax",
                point => new string[]{point.POSITION,point.building,point.level},
                new object[]{"(200,200,200)","教保楼",1})
                .Where(point=>point.ID > 5 && point.ID < 8).ExecuteNoQuery();
            Sqlite3.Close();
        }

        [Test(Author = "springdong",Description = "删除表信息")]
        public static void Alpha_6_USqliteDelete()
        {
            Sqlite3.Open(FilePath.normalPath + TABLENAME);
            Sqlite3.Table<Point>().Delete("PointMax").Where(point => point.ID > 8 && point.ID < 12).ExecuteNoQuery();
            Sqlite3.Close();
        }
    }
}