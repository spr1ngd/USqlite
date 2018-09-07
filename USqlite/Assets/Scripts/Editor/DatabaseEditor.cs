
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using USqlite;

namespace miniMVC.USqlite
{
    [TestFixture]
    public class DatabaseEditor : Editor
    {
        private const string TABLENAME = "Point";

        //[NUnit.Framework.]
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
        [MenuItem("Database Utility/Sqlite/Select")]
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
                    var pointDatas = SqliteUtility.QueryObject<Point[]>(TABLENAME,null,"ID < 1500");
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

        //[Test]
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

        //[Test]
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

        //[Test]
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
            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            //for (int i = 0; i < 150000; i++)
            //{
            //    string strA = "APple";
            //    string strB = "apple";

            //    //if (strA.ToUpper() == strB.ToUpper())
            //    //{

            //    //}

            //    if(strA.Equals(strB,StringComparison.OrdinalIgnoreCase))
            //    {

            //    }
            //}
            //watch.Stop();
            //UnityEngine.Debug.Log(string.Format("比较：[{0}] ",watch.Elapsed));

            Expression<Func<int, bool>> expression = x => x < 200;
            Func<int, bool> func = expression.Compile();
            var result = func(20);
            UnityEngine.Debug.Log(result);

            InvocationExpression invocation = Expression.Invoke(
                expression,
                Expression.Constant(50));
            UnityEngine.Debug.Log(invocation.ToString());
            UnityEngine.Debug.Log(Expression.Lambda<Func<bool>>(invocation).Compile()());

            // invoke 1 + 2
            var addExpression = Expression.Add(Expression.Constant(100), Expression.Constant(200));
            var addLambda = Expression.Lambda<Func<int>>(addExpression).Compile();
            UnityEngine.Debug.Log("1 + 2 = " + addLambda());

            // sin
            var sinParam = Expression.Parameter(typeof(double), "radian");
            var sinMethod = Expression.Call(null,
                typeof(Math).GetMethod("Sin", BindingFlags.Public | BindingFlags.Static),sinParam);
            var sinLambda = Expression.Lambda<Func<double, double>>(sinMethod,sinParam).Compile();
            UnityEngine.Debug.Log("Sin(x) = " + sinLambda(30));

            // delegate
            Expression<Func<int, int>> func1 = i => i*2;
            var funcParam = Expression.Parameter(typeof(int),"param1");
            var funcInvocation = Expression.Invoke(func1, funcParam);
            var funcLambda = Expression.Lambda<Func<int, int>>(funcInvocation,funcParam).Compile();
            UnityEngine.Debug.Log("Func:" + funcLambda(200));

            // AndAlso
            Expression<Func<Point, bool>> conditionA = point => point.ID == 5;
            Expression<Func<Point, bool>> conditionB = point => point.POSITION == "(0,0,0)";
            var funcA = Expression.Invoke(conditionB,conditionA.Parameters.ToArray());
            UnityEngine.Debug.Log("funcA : " + funcA);

            var funcB = Expression.AndAlso(conditionA.Body,funcA);
            UnityEngine.Debug.Log("funcB : " + funcB);
        }


        [Test]
        public static void USqliteQuery()
        {
            Sqlite3.Open(FilePath.normalPath + "TestDB.db");
            Sqlite3.Table<Point>().Select().Where(point => point.ID < 15000 ).Execute();
            Sqlite3.Close();
        }

        [Test]
        public static void USqliteInsert()
        {
            //Sqlite3.Open(FilePath.normalPath + "TestDB.db");
            //Sqlite3.Insert<Point[]>("Point", null).Execute();
            //Sqlite3.Close();
        }

        [Test]
        public static void USqliteUpdate()
        {
            //Sqlite3.Open(FilePath.normalPath + "TestDB.db");
            //Sqlite3.Update<Point[]>("Point", null).Execute();
            //Sqlite3.Close();
        }

        [Test]
        public static void USqliteDelete()
        {
            //Sqlite3.Open(FilePath.normalPath + "TestDB.db");
            //Sqlite3.Delete("Point").Execute();
            //Sqlite3.Close();
        }
    }
}