
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace miniMVC.USqlite
{
    [TestFixture]
    public class DatabaseEditor : Editor
    {
        private const string TABLENAME = "Point";

        //[Test]
        [MenuItem("Database Utility/Sqlite/Create Database")]
        public static void CreateDatabase()
        {
            SqliteUtility.CreateDatabase(FilePath.normalPath + "TestDB.db");
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Drop Database")]
        public static void DropDatabase()
        {
            SqliteUtility.DropDatabase(FilePath.normalPath + "TestDB.db");
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Open Database")]
        public static void OpenDatabase()
        {
            SqliteUtility.OpenDatabase(FilePath.normalPath + "TestDB.db");
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Close Database")]
        public static void CloseDatabase()
        {
            SqliteUtility.CloseDatabase(FilePath.normalPath + "TestDB.db");
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Create Table")]
        public static void CreateTable()
        {
            SqliteUtility.CreateTable("maxPoints",
                new string[]
                {
                    "ID",
                    "POSITION",
                    "SCALE",
                    "ROTATION"
                },
                new string[]
                {
                    "INT PRIMARY KEY NOT NULL",
                    "VARCHAR(60) NOT NULL",
                    "VARCHAR(60) NOT NULL",
                    "VARCHAR(60) NOT NULL"
                });
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Delete Table")]
        public static void DeleteTable()
        {
            SqliteUtility.DeleteTable("maxPoints");
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Insert Without Transaction")]
        public static void Insert()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for(int i = 0; i < 1500; i++)
            {
                SqliteUtility.Insert("maxPoints",new string[] { i.ToString(),@"""0,0,0""",@"""0,0,0""",@"""0,0,0""" });
            }
            watch.Stop();
            UnityEngine.Debug.Log(string.Format("插入数据 并不使用事务处理 耗时：[{0}] ",watch.Elapsed));
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Insert With Transaction")]
        public static void InsertWithTransaction()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SqliteUtility.BeginTransaction(() =>
            {
                for(int i = 0; i < 1500; i++)
                {
                    SqliteUtility.Insert("maxPoints",new string[] { i.ToString(),@"""0,0,0""",@"""0,0,0""",@"""0,0,0""" });
                }
            });
            watch.Stop();
            UnityEngine.Debug.Log(string.Format("使用事务处理插入数据 耗时：[{0}] ",watch.Elapsed));
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Update")]
        public static void Update()
        {
            
        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Delete")]
        public static void Delete()
        {

        }

        //[Test]
        [MenuItem("Database Utility/Sqlite/Query")]
        public static void Query()
        {
            Stopwatch watch = new Stopwatch();
            SqliteUtility.BeginTransaction(() =>
            {
                SqliteUtility.QueryObject<List<Point>>(TABLENAME);
            });
            watch.Stop();
            UnityEngine.Debug.Log(string.Format("查询耗时：[{0}] ",watch.Elapsed));
        }

        [Test]
        public static void QueryOnce()
        {
            
            SqliteUtility.OpenDatabase(FilePath.normalPath + "TestDB.db");
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                SqliteUtility.BeginTransaction(() =>
                {
                    // tip List
                    var pointDatas = SqliteUtility.QueryObject<Point[]>(TABLENAME,null,"ID < 15000");
                    //foreach(Point pointData in pointDatas)
                    //    UnityEngine.Debug.Log(string.Format("<color=red>{0}</color>",pointData.ToString()));

                    // tip object
                    //var point = SqliteUtility.QueryObject<Point>(TABLENAME,null," ID = 5");
                    //UnityEngine.Debug.Log(point);

                    // tip property
                    //var quaternion = SqliteUtility.QueryObject<Quaternion>(TABLENAME, new string[] {"QUATERNION"}, " ID = 5");
                    //UnityEngine.Debug.Log(quaternion);

                    // tip dictionary

                    // tip array
                });
                watch.Stop();
                UnityEngine.Debug.Log(string.Format("查询耗时：[{0}] ",watch.Elapsed));
            }
            catch ( Exception exception )
            {
                UnityEngine.Debug.Log(exception);
            }
            SqliteUtility.CloseDatabase(FilePath.normalPath + "TestDB.db");
        }

        [Test]
        public static void InsertData()
        {
            SqliteUtility.OpenDatabase(FilePath.normalPath + "TestDB.db");
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                SqliteUtility.BeginTransaction(() =>
                {
                    IList<Point> points = new List<Point>();
                    for (int i = 0; i < 15000; i++)
                    {
                        points.Add(new Point()
                        {
                            ID = i,
                            POSITION = Vector3.one.ToString(),                                                                                                                                                                                                                                                                                                                      
                            //SCALE = Vector3.one,
                            //ROTATION = Vector3.one
                        });
                    }
                    SqliteUtility.InsertObject(TABLENAME,points);
                });
                watch.Stop();
                UnityEngine.Debug.Log(string.Format("插入耗时：[{0}] ",watch.Elapsed));
            }
            catch(Exception exception)
            {
                UnityEngine.Debug.Log(exception);
            }
            SqliteUtility.CloseDatabase(FilePath.normalPath + "TestDB.db");
        }

        [Test]
        public static void UpdateData()
        {
            SqliteUtility.OpenDatabase(FilePath.normalPath + "TestDB.db");
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                SqliteUtility.BeginTransaction(() =>
                {
                    var point = SqliteUtility.QueryObject<Point>(TABLENAME,null,"ID = 5");
                    //point.ROTATION = UnityEngine.Random.Range(0.1f, 1.0f) * Vector3.one;
                    SqliteUtility.UpdateObject(TABLENAME, point);
                });
                watch.Stop();
                UnityEngine.Debug.Log(string.Format("插入耗时：[{0}] ",watch.Elapsed));
            }
            catch(Exception exception)
            {
                UnityEngine.Debug.Log(exception);
            }
            SqliteUtility.CloseDatabase(FilePath.normalPath + "TestDB.db");
        }

        [Test]
        public static void DeleteData()
        {
            SqliteUtility.OpenDatabase(FilePath.normalPath + "TestDB.db");
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                SqliteUtility.BeginTransaction(() =>
                {
                    var point = SqliteUtility.QueryObject<List<Point>>(TABLENAME);
                    SqliteUtility.DeleteObject(TABLENAME,point);
                });
                watch.Stop();
                UnityEngine.Debug.Log(string.Format("插入耗时：[{0}] ",watch.Elapsed));
            }
            catch(Exception exception)
            {
                UnityEngine.Debug.Log(exception);
            }
            SqliteUtility.CloseDatabase(FilePath.normalPath + "TestDB.db");
        }

        [Test]
        public static void TempTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < 150000; i++)
            {
                string strA = "APple";
                string strB = "apple";
                //if (strA.ToUpper() == strB.ToUpper())
                //{

                //}

                if(strA.Equals(strB,StringComparison.OrdinalIgnoreCase))
                {

                }
            }

            watch.Stop();

            UnityEngine.Debug.Log(string.Format("比较：[{0}] ",watch.Elapsed));
        }
    }
}