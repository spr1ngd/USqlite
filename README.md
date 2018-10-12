# USqlite

## 关于

>  支持unity3d对象的Sqlite工具

> 支持x64 x86，暂不支持Android、IOS

## 使用案例

1. **连接数据库**

```C#
Sqlite3.Open('filepath.db');
```

2. **闭关数据库连接**

```C#
Sqlite3.Close();
```

3. **创建表**

```C#
Sqlite3.CreateTable<Point>();
```

```C#
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
}
```

4. **删除表**

```C#
Sqlite3.DropTable<Point>();
```

5. **插入**

```C#
Sqlite3.Table<Point>().Insert("PointMax",points);
```

6. **删除**

```C#
int MIN = 8;
int MAX = 12;
Sqlite3.Table<Point>().Delete("PointMax").Where(point => (point.ID > MIN && point.ID < MAX)).ExecuteNoQuery();
```

7. **修改**

```C#
Sqlite3.Table<Point>().Update("PointMax",
            point => new string[]{point.POSITION,point.building,point.level},
            new object[]{"(200,200,200)","大楼",1})
            .Where(point=>point.ID > 5 && point.ID < 8).ExecuteNoQuery();
```

8. **查询**

```C#
Point testPoint = new Point();
testPoint.ID = 20;
Sqlite3.Open(FilePath.normalPath + TABLENAME);
var points = Sqlite3.Table<Point>().Select("PointMax").Where(point => point.ID < testPoint.ID).Execute2List();
```

## 支持接口

SQL | Status
----|----
Insert | :white_check_mark:
Update | :white_check_mark:
Select | :white_check_mark:
Delete | :white_check_mark:
AND/OR | :white_check_mark:
Like | :x:
Glob | :x:
Limit | :x:
Order By | :x:
Json | :x: