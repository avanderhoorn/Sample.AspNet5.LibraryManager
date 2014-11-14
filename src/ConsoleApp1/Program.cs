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
            manager.Setup(_manager);


            Console.WriteLine("Hello World");
            Console.ReadLine();
        }
    }
}
