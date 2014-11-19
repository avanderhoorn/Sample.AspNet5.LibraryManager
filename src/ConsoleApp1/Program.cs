using Microsoft.Framework.Runtime;
using System;
using Glimpse;
using Microsoft.Framework.DependencyInjection;

namespace ConsoleApp1
{
    public class Program
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITypeActivator _typeActivator;

        public Program(ILibraryManager libraryManager, IServiceProvider serviceProvider)
        {
            _libraryManager = libraryManager;
            _serviceProvider = serviceProvider;
            _typeActivator = new TypeActivator();
        }

        public void Main(string[] args)
        { 
            var manager = new ResolveDependencyManager(_serviceProvider, _typeActivator, _libraryManager);
            var tabs = manager.RegisteredTabs("Glimpse");

            if (tabs != null)
            {
                Console.WriteLine("Found Tabs:");
                foreach (var tab in tabs)
                    Console.WriteLine("    - \{tab.GetType().FullName}");
            }

            Console.ReadLine();
        } 
    }
}
