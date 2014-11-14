using ConsoleApp1;
using System;

namespace OtherDependency
{
    public class OtherTab : ITab
    {
        public string Execute()
        {
            return "Other";
        }
    }
}
