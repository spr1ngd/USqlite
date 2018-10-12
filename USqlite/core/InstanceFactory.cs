
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace USqlite
{
    public delegate object CreateInstanceDelegate();

    public class InstanceFactory
    {
        private readonly IDictionary<Type,CreateInstanceDelegate> m_instanceDelegateDic = new Dictionary<Type,CreateInstanceDelegate>();

        public CreateInstanceDelegate ConstructeInstance(Type type)
        {
            CreateInstanceDelegate instanceDelegate = null;
            if(!m_instanceDelegateDic.TryGetValue(type,out instanceDelegate))
            {
                DynamicMethod dynamicMethod = new DynamicMethod("CreateInstance",type,new Type[0]);
                ConstructorInfo ctorInfo = type.GetConstructor(new Type[0]);
                ILGenerator ilGen = dynamicMethod.GetILGenerator();
                ilGen.Emit(OpCodes.Newobj,ctorInfo);
                ilGen.Emit(OpCodes.Ret);
                instanceDelegate = (CreateInstanceDelegate)dynamicMethod.CreateDelegate(typeof(CreateInstanceDelegate));
                m_instanceDelegateDic.Add(type,instanceDelegate);
            }
            return instanceDelegate;
        }
    }
}