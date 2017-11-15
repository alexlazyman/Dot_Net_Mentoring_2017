using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Task5.Attributes;

namespace Task5
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, Type> _maps;

        public Container()
        {
            _maps = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var maps = assembly.DefinedTypes
                .Where(HasCreatableAttributes)
                .SelectMany(GetMaps);

            foreach (var map in maps)
            {
                _maps.Add(map.Key, map.Value);
            }
        }

        public void AddType(Type type)
        {
            AddType(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            if (!baseType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"{nameof(baseType)} is not assignable from {nameof(type)}");
            }

            if (!IsInstantiatableType(type))
            {
                throw new ArgumentException($"It is not instantiatable type");
            }

            _maps.Add(baseType, type);
        }

        public object CreateInstance(Type type)
        {
            if (!_maps.ContainsKey(type))
            {
                throw new InvalidOperationException("There is no appropriate type");
            }

            var targetType = _maps[type];

            var targetCtor = targetType.GetConstructors().Single();
            var args = targetCtor.GetParameters().Select(p => CreateInstance(p.ParameterType)).ToArray();

            var instance = Activator.CreateInstance(targetType, args);

            var props = targetType.GetProperties()
                .Where(p => p.CanWrite)
                .Where(p => p.GetCustomAttribute<ImportAttribute>() != null);
            foreach (var prop in props)
            {
                prop.SetValue(instance, CreateInstance(prop.PropertyType));
            }

            return instance;
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        private bool IsInstantiatableType(Type type)
        {
            return (type.IsClass && !type.IsAbstract) || type.IsValueType;
        }

        private bool HasCreatableAttributes(Type type)
        {
            return type.GetCustomAttributes<ImportConstructorAttribute>().Any()
                || type.GetCustomAttributes<ExportAttribute>().Any();
        }

        private IEnumerable<KeyValuePair<Type, Type>> GetMaps(Type type)
        {
            foreach (var exportAttribute in type.GetCustomAttributes<ExportAttribute>())
            {
                yield return new KeyValuePair<Type, Type>(exportAttribute.Contract ?? type, type);
            }

            if (type.GetCustomAttributes<ImportConstructorAttribute>().Any())
            {
                yield return new KeyValuePair<Type, Type>(type, type);
            }
        }
    }
}