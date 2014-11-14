using Microsoft.Framework.Runtime;
using System;
using System.Linq;
using System.Reflection;

namespace Glimpse
{
    public class Manager
    {
        public void Setup(ILibraryManager manager)
        {
            var libraries = manager.GetReferencingLibraries("ConsoleApp1");
            var assemblyNames = libraries.SelectMany(l => l.LoadableAssemblies);
            var assemblies = assemblyNames.Select(x => Assembly.Load(x));
            var types = assemblies.SelectMany(a => a.DefinedTypes);
        }
    }
}
