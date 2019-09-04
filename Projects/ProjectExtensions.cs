using System.IO;

namespace Projects
{
    public static class ProjectExtensions
    {
        public static void WriteTo(this Project project,  TextWriter writer, int indent = 0)
        {
            for (int i = 0; i < indent; ++i)
            {
                writer.Write("   ");
            }

            writer.WriteLine($"{project.Name} ({project.Location})");
            foreach (var reference in project.References)
            {
                switch (reference)
                {
                    case ProjectReference projectReference:
                        projectReference.WriteTo(writer, indent);
                        break;
                }
            }
        }
        
        public static void WriteTo(this ProjectReference projectReference, TextWriter writer, int indent = 0)
        {
            projectReference.Project.WriteTo(writer, indent + 1);

        }
    }
}
