
using System.Collections.Generic;
using System.Reflection;

namespace miniMVC.USqlite
{
    /// <summary>
    /// Object mapping to table. 
    /// Table mapping to object.
    /// </summary>
    public class TableMapper
    {
        private IDictionary<string, FieldInfo> m_fieldInfoDic = null;

        public TableMapper()
        {
            m_fieldInfoDic = new Dictionary<string, FieldInfo>();
        }

        public void AddFiledInfo( string fieldName,FieldInfo filedInfo )
        {
            m_fieldInfoDic.Add(fieldName,filedInfo);
        }

        public bool TryGetField(string fieldName,out FieldInfo fieldInfo)
        {
            return m_fieldInfoDic.TryGetValue(fieldName, out fieldInfo);
        }
    }
}