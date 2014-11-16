using Microsoft.Framework.Runtime;
using System;
using Glimpse;

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
            var manager = new Manager();

            PrintResults("Glimpse");

            Console.ReadLine();
        }

        private void PrintResults(string library)
        {
            var manager = new Manager(); 
            var references = manager.Setup(_manager, library);

            Console.WriteLine("\{library} References:");
            foreach (var reference in references)
                Console.WriteLine("    - \{reference}");
            Console.WriteLine("\n\n");
        }
    }
}
