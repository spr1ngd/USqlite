
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace USqlite
{
    public class ColumnMapper
    {
        private readonly IDictionary<string,MemberInfo> m_memberInfoDic = null;

        public ColumnMapper()
        {
            m_memberInfoDic = new Dictionary<string,MemberInfo>();
        }

        public void AddMemberInfo(string memberName,MemberInfo memberInfo)
        {
            m_memberInfoDic.Add(memberName,memberInfo);
        }

        public bool TryGetMember(string memberName,out MemberInfo memberInfo)
        {
            return m_memberInfoDic.TryGetValue(memberName,out memberInfo);
        }
    }

    public class CellMapper
    {
        public string columeName { get; set; }
        public DbType dbType { get; set; }
        public Type type { get; set; }
        public bool notNull { get; set; }
        public bool isPrimaryKey { get; set; }
        public bool isAutoIncrease { get; set; }
        public MemberInfo memberInfo { get; set; }
        public object value { get; set; }
    }
}