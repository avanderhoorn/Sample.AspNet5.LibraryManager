using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glimpse
{
    public class Manager
    {
        public IEnumerable<string> Setup(ILibraryManager manager, string libarary)
        {
            var libraries = manager.GetReferencingLibraries(libarary);
            var assemblyNames = libraries.SelectMany(l => l.LoadableAssemblies); 
            var assemblies = assemblyNames.Select(x => Assembly.Load(x));
            var types = assemblies.SelectMany(a => a.DefinedTypes);

            return assemblyNames.Select(x => x.Name);
        }
    }
}
