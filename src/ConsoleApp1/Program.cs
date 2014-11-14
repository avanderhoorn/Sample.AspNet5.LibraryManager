using Microsoft.Framework.Runtime;
using System;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    public class Program
    {
        private readonly ILibraryManager _manager;

        public Program(ILibraryManager manager)
        {
            _manager = manager;
        }

        public void Main(string[] args) 
        {
            var libraries = _manager.GetReferencingLibraries("ConsoleApp1");
            var assemblyNames = libraries.SelectMany(l => l.LoadableAssemblies);
            var assemblies = assemblyNames.Select(x => Assembly.Load(x)); 
            var types = assemblies.SelectMany(a => a.DefinedTypes);


            Console.WriteLine("Hello World");
            Console.ReadLine();
        }
    }
}
