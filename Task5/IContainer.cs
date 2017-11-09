using System;
using System.Reflection;

namespace Task5
{
    public interface IContainer
    {
        void AddAssembly(Assembly assembly);

        void AddType(Type type);

        void AddType(Type type, Type baseType);

        object CreateInstance(Type type);

        T CreateInstance<T>();
    }
}