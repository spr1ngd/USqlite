
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class BaseCommand
    {
        private readonly SqliteConnection m_sqliteConnection = null;
        protected string m_commandText = string.Empty;
        public string commandText { get { return string.Format("{0} {1} {2}",m_commandText,conditionCommand,orderbyCommand); } }

        private readonly string m_tableName;
        protected string tableName
        {
            get
            {
                if(!string.IsNullOrEmpty(m_tableName))
                    return m_tableName;
                string result = "";
                if(string.IsNullOrEmpty(result))
                    result = mapperType.AttributeOf<TableAttribute>().tableName;
                if(string.IsNullOrEmpty(result))
                    result = mapperType.Name;
                return result;
            }
        }

        protected Type mapperType = null;
        protected string conditionCommand = string.Empty;
        protected string orderbyCommand = string.Empty;

        public BaseCommand(SqliteConnection connection,Type type,string tableName)
        {
            this.m_sqliteConnection = connection;
            this.mapperType = type;
            this.m_tableName = tableName;
        }

        public virtual SqliteDataReader Execute()
        {
            try
            {
                var command = m_sqliteConnection.CreateCommand();
                command.CommandText = commandText;
                //UnityEngine.Debug.Log(string.Format("<color=green>USqlite execute : {0}</color>",command.CommandText));
                return command.ExecuteReader();
            }
            catch(Exception @exception)
            {
                throw new USqliteException("Database Rollback",@exception.InnerException);
            }
        }

        public virtual BaseCommand Where(string condition)
        {
            if(!string.IsNullOrEmpty(condition))
            {
                if(string.IsNullOrEmpty(conditionCommand)) conditionCommand = string.Format("WHERE {0}",condition);
                else conditionCommand += string.Format("AND {0}",condition);
            }
            return this;
        }

        private string ConvertExpression2Sql<T>(Expression<Func<T,bool>> lambdaExpression)
        {
            Dictionary<string,string> keyValues = new Dictionary<string,string>();
            string sql = lambdaExpression.Body.ToString();
            var expression = lambdaExpression.Body;
            BinaryExpression binaryExpression = expression as BinaryExpression;
            ConvertExpression2Sql(binaryExpression,ref keyValues);
            foreach(KeyValuePair<string,string> pair in keyValues)
                sql = sql.Replace(pair.Key,pair.Value);
            return sql;
        }

        private void ConvertExpression2Sql(BinaryExpression binaryExpresion,ref Dictionary<string,string> keyValues)
        {
            if(binaryExpresion.Left is MemberExpression)
            {
                Expression member = binaryExpresion.Right;
                string key = binaryExpresion.Right.ToString();
                var value = Expression.Lambda(member).Compile().DynamicInvoke().ToString();
                int intValue;
                float floatValue;
                double doubleValue;
                bool boolValue;
                if(int.TryParse(value,out intValue) |
                    float.TryParse(value,out floatValue) |
                    double.TryParse(value,out doubleValue) |
                    bool.TryParse(value,out boolValue))
                {
                }
                else
                    value = string.Format(@"""{0}""",value);
                keyValues.Add(key,value);
            }
            else
            {
                BinaryExpression leftExpression = binaryExpresion.Left as BinaryExpression;
                if (null != leftExpression) ConvertExpression2Sql(leftExpression, ref keyValues);
                else throw new USqliteException("表达式转换失败");

                BinaryExpression rightExpression = binaryExpresion.Right as BinaryExpression;
                if(null != rightExpression) ConvertExpression2Sql(rightExpression,ref keyValues);
                else throw new USqliteException("表达式转换失败");
            }
        }

        public virtual BaseCommand Where<T>(Expression<Func<T,bool>> whereExpression)
        {
            var condition = ConvertExpression2Sql(whereExpression);
            condition = condition.Replace("(","").Replace(")","");
            int tagEnd = condition.IndexOf(".",StringComparison.Ordinal) + 1;
            string keyPoint = condition.Substring(0,tagEnd);
            condition = condition.Replace(keyPoint,"");
            condition = condition.Replace("&&","AND").Replace("|","OR");
            return Where(condition);
        }

        public virtual BaseCommand OrderBy(string columnName)
        {
            if(!string.IsNullOrEmpty(columnName))
            {
                if(string.IsNullOrEmpty(orderbyCommand)) orderbyCommand = string.Format("ORDER BY {0}",columnName);
                else orderbyCommand += string.Format(", {0}",columnName);
            }
            return this;
        }

        public virtual BaseCommand OrderBy<T>(Expression<Func<T,bool>> orderByExpression)
        {
            return this;
        }

        protected string GetColumes(Type type,out string[] columeNames,out string[] columeTypes)
        {
            FieldInfo[] fieldInfos = type.GetFields();
            IList<CellMapper> cells = new List<CellMapper>();
            foreach(FieldInfo fieldInfo in fieldInfos)
            {
                var columeAtt = fieldInfo.AttributeOf<ColumnAttribute>();
                if(null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if(string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = fieldInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = fieldInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if(columeAtt.columnType == (DbType.Int32 | DbType.Int64 | DbType.Int16))
                    {
                        var autoIncrease = fieldInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cells.Add(cell);
                }
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach(PropertyInfo propertyInfo in properties)
            {
                var columeAtt = propertyInfo.AttributeOf<ColumnAttribute>();
                if(null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if(string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = propertyInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = propertyInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if(columeAtt.columnType == DbType.Int32 || columeAtt.columnType == DbType.Int64 | columeAtt.columnType == DbType.Int16)
                    {
                        var autoIncrease = propertyInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cells.Add(cell);
                }
            }

            columeNames = new string[cells.Count];
            columeTypes = new string[cells.Count];
            for(int i = 0; i < columeNames.Length; i++)
            {
                columeNames[i] = cells[i].columeName;
                columeTypes[i] = cells[i].dbType.ToString();
                if(cells[i].isPrimaryKey)
                    columeTypes[i] += " PRIMARY KEY";
                if(cells[i].isAutoIncrease)
                    columeTypes[i] += "AUTOINCREMENT";
            }

            TableAttribute tableAtt = type.AttributeOf<TableAttribute>();
            if(null != tableAtt)
                return tableAtt.tableName;
            return type.Name;
        }
    }

    public static class ExpressionExtensions
    {
        public static IEnumerable ExtractConstants(this Expression<Action> expression)
        {
            var lambdaExpression = expression as LambdaExpression; if(lambdaExpression == null) throw new InvalidOperationException("Please provide a lambda expression.");

            var methodCallExpression = lambdaExpression.Body as MethodCallExpression;
            if(methodCallExpression == null)
                throw new InvalidOperationException("Please provide a *method call* lambda expression.");

            return ExtractConstants(methodCallExpression);
        }

        public static IEnumerable<object> ExtractConstants(Expression expression)
        {
            if(expression == null || expression is ParameterExpression)
                return new object[0];

            var memberExpression = expression as MemberExpression;
            if(memberExpression != null)
                return ExtractConstants(memberExpression);

            var constantExpression = expression as ConstantExpression;
            if(constantExpression != null)
                return ExtractConstants(constantExpression);

            var newArrayExpression = expression as NewArrayExpression;
            if(newArrayExpression != null)
                return ExtractConstants(newArrayExpression);

            var newExpression = expression as NewExpression;
            if(newExpression != null)
                return ExtractConstants(newExpression);

            var unaryExpression = expression as UnaryExpression;
            if(unaryExpression != null)
                return ExtractConstants(unaryExpression);

            return new object[0];
        }

        private static IEnumerable<object> ExtractConstants(MethodCallExpression methodCallExpression)
        {
            var constants = new List<object>();
            foreach(var arg in methodCallExpression.Arguments)
            {
                constants.AddRange(ExtractConstants(arg));
            }

            constants.AddRange(ExtractConstants(methodCallExpression.Object));

            return constants;
        }

        private static IEnumerable<object> ExtractConstants(UnaryExpression unaryExpression)
        {
            return ExtractConstants(unaryExpression.Operand);
        }

        private static IEnumerable<object> ExtractConstants(NewExpression newExpression)
        {
            var arguments = new List<object>();
            foreach(var argumentExpression in newExpression.Arguments)
            {
                arguments.AddRange(ExtractConstants(argumentExpression));
            }

            yield return newExpression.Constructor.Invoke(arguments.ToArray());
        }

        private static IEnumerable<object> ExtractConstants(NewArrayExpression newArrayExpression)
        {
            Type type = newArrayExpression.Type.GetElementType();
            if(type is IConvertible)
                return ExtractConvertibleTypeArrayConstants(newArrayExpression,type);

            return ExtractNonConvertibleArrayConstants(newArrayExpression,type);
        }

        private static IEnumerable<object> ExtractNonConvertibleArrayConstants(NewArrayExpression newArrayExpression,Type type)
        {
            var arrayElements = CreateList(type);
            foreach(var arrayElementExpression in newArrayExpression.Expressions)
            {
                object arrayElement;

                if(arrayElementExpression is ConstantExpression)
                    arrayElement = ((ConstantExpression)arrayElementExpression).Value;
                else
                    arrayElement = ExtractConstants(arrayElementExpression).ToArray();

                if(arrayElement is object[])
                {
                    foreach(var item in (object[])arrayElement)
                        arrayElements.Add(item);
                }
                else
                    arrayElements.Add(arrayElement);
            }

            return ToArray(arrayElements);
        }

        private static IEnumerable<object> ToArray(IList list)
        {
            var toArrayMethod = list.GetType().GetMethod("ToArray");
            yield return toArrayMethod.Invoke(list,new Type[] { });
        }

        private static IList CreateList(Type type)
        {
            return (IList)typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]).Invoke(BindingFlags.CreateInstance,null,null,null);
        }

        private static IEnumerable<object> ExtractConvertibleTypeArrayConstants(NewArrayExpression newArrayExpression,Type type)
        {
            var arrayElements = CreateList(type);
            foreach(var arrayElementExpression in newArrayExpression.Expressions)
            {
                var arrayElement = ((ConstantExpression)arrayElementExpression).Value;
                arrayElements.Add(Convert.ChangeType(arrayElement,arrayElementExpression.Type,null));
            }

            yield return ToArray(arrayElements);
        }

        private static IEnumerable<object> ExtractConstants(ConstantExpression constantExpression)
        {
            var constants = new List<object>();

            if(constantExpression.Value is Expression)
            {
                constants.AddRange(ExtractConstants((Expression)constantExpression.Value));
            }
            else
            {
                if(constantExpression.Type == typeof(string) ||
                    constantExpression.Type.IsPrimitive ||
                    constantExpression.Type.IsEnum ||
                    constantExpression.Value == null)
                    constants.Add(constantExpression.Value);
            }

            return constants;
        }

        private static IEnumerable<object> ExtractConstants(MemberExpression memberExpression)
        {
            var constants = new List<object>();
            var constExpression = (ConstantExpression)memberExpression.Expression;
            var valIsConstant = constExpression != null;
            Type declaringType = memberExpression.Member.DeclaringType;
            object declaringObject = memberExpression.Member.DeclaringType;

            if(valIsConstant)
            {
                declaringType = constExpression.Type;
                declaringObject = constExpression.Value;
            }

            var member = declaringType.GetMember(memberExpression.Member.Name,MemberTypes.Field | MemberTypes.Property,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Single();

            if(member.MemberType == MemberTypes.Field)
                constants.Add(((FieldInfo)member).GetValue(declaringObject));
            else
                constants.Add(((PropertyInfo)member).GetGetMethod(true).Invoke(declaringObject,null));

            return constants;
        }
    }
}