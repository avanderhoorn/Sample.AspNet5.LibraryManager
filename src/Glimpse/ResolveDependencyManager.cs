using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glimpse
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> GetCandidateAssemblies(string coreLibrary);
    }

    public class DefaultAssemblyProvider : IAssemblyProvider
    { 
        private readonly ILibraryManager _manager;

        public DefaultAssemblyProvider(ILibraryManager manager)
        {
            _manager = manager;
        }

        public IEnumerable<Assembly> GetCandidateAssemblies(string coreLibrary)
        { 
            var libraries = _manager.GetReferencingLibraries(coreLibrary);
            var assemblyNames = libraries.SelectMany(l => l.LoadableAssemblies);
            var assemblies = assemblyNames.Select(x => Assembly.Load(x));

            return assemblies;
        }
    }


    public interface ITypeDiscovery
    {
        IEnumerable<TypeInfo> FindTypes(IEnumerable<Assembly> targetAssmblies, TypeInfo targetTypeInfo);
    }

    public class DefaultTypeDiscovery : ITypeDiscovery
    {
        public IEnumerable<TypeInfo> FindTypes(IEnumerable<Assembly> targetAssmblies, TypeInfo targetTypeInfo)
        {
            var types = targetAssmblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.IsClass && 
                        !t.IsAbstract && 
                        !t.ContainsGenericParameters &&
                        targetTypeInfo.IsAssignableFrom(t));

            return types;
        }
    }


    public interface ITypesActivator
    {
        IEnumerable<object> CreateInstances(IEnumerable<TypeInfo> types);

        IEnumerable<T> CreateInstances<T>(IEnumerable<TypeInfo> types);
    }

    public class DefaultTypesActivator : ITypesActivator
    {
        private readonly ITypeActivator _typeActivator;
        private readonly IServiceProvider _serviceProvider;

        public DefaultTypesActivator(IServiceProvider serviceProvider, ITypeActivator typeActivator)
        {
            _typeActivator = typeActivator;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<object> CreateInstances(IEnumerable<TypeInfo> types)
        {
            var activated = types.Select(t => CreateInstance(t));

            return activated;
        }

        public IEnumerable<T> CreateInstances<T>(IEnumerable<TypeInfo> types)
        {
            var activated = types.Select(t => (T)CreateInstance(t));

            return activated;
        }

        private object CreateInstance(TypeInfo type)
        {
            return _typeActivator.CreateInstance(_serviceProvider, type.AsType());
        }
    }


    public interface ITypeResolver
    {
        IEnumerable<object> Resolve(string coreLibrary, Type targetType);

        IEnumerable<T> Resolve<T>(string coreLibrary);
    }

    public class AssemblyTypeResolver : ITypeResolver
    {
        private readonly ITypesActivator _typesActivator;
        private readonly ITypeDiscovery _typeDiscovery;
        private readonly IAssemblyProvider _assemblyProvider;

        public AssemblyTypeResolver(ITypesActivator typesActivator, ITypeDiscovery typeDiscovery, IAssemblyProvider assemblyProvider)
        {
            _typesActivator = typesActivator;
            _typeDiscovery = typeDiscovery;
            _assemblyProvider = assemblyProvider;
        }

        public IEnumerable<object> Resolve(string coreLibrary, Type targetType)
        {
            var types = DiscoverTypes(coreLibrary, targetType);
            var instances = _typesActivator.CreateInstances(types);

            return instances;
        }

        public IEnumerable<T> Resolve<T>(string coreLibrary)
        {
            var types = DiscoverTypes(coreLibrary, typeof(T));
            var instances = _typesActivator.CreateInstances<T>(types);

            return instances;
        }

        private IEnumerable<TypeInfo> DiscoverTypes(string coreLibrary, Type targetType)
        {
            var assemblies = _assemblyProvider.GetCandidateAssemblies(coreLibrary);
            var types = _typeDiscovery.FindTypes(assemblies, targetType.GetTypeInfo());

            return types;
        }
    }



    public class ResolveDependencyManager
    {
        private readonly ITypeActivator _typeActivator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILibraryManager _libraryManager;

        public ResolveDependencyManager(IServiceProvider serviceProvider, ITypeActivator typeActivator, ILibraryManager libraryManager)
        {
            _typeActivator = typeActivator;
            _serviceProvider = serviceProvider;
            _libraryManager = libraryManager;
        }

        public IEnumerable<ITab> RegisteredTabs(string libarary)
        {
            var resolver = new AssemblyTypeResolver(new DefaultTypesActivator(_serviceProvider, _typeActivator), new DefaultTypeDiscovery(), new DefaultAssemblyProvider(_libraryManager));
             
            return resolver.Resolve<ITab>(libarary);
        }
    }
}
