using Projects;
using System;

namespace ShowReferences
{
    class Program
    {
        static void Main(string[] args)
        {
            var project = Project.Load(args[0]);

            project.WriteTo(Console.Out);
        }
    }
}
