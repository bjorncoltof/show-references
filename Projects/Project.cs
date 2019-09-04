using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Projects
{
    public class Project
    {
        private List<IReference> _references = new List<IReference>();
        public string Name { get; private set; }

        public string Location { get; private set; }

        public IEnumerable<IReference> References
        {
            get
            {
                return _references;
            }
        }

        public static Project Load(string projectFile)
        {
            using (var stream = File.OpenRead(projectFile))
            {
                var project = new Project();
                project.Name = Path.GetFileName(projectFile);
                project.Location = Path.GetFullPath(projectFile);

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);

                // load project references
                LoadProjectReferences(Path.GetDirectoryName(project.Location), xmlDocument, project);

                return project;
            }
        }

        private static void LoadProjectReferences(string basePath, XmlDocument xmlDocument, Project project)
        {
            var mgr = new XmlNamespaceManager(xmlDocument.NameTable);
            mgr.AddNamespace("ns", xmlDocument.DocumentElement.NamespaceURI);

            var nodes = xmlDocument.SelectNodes("//ns:ProjectReference", mgr);
            foreach (var node in nodes.Cast<XmlNode>())
            {
                var fileName = WebUtility.UrlDecode(node.Attributes["Include"].Value);
                var location = fileName.StartsWith(".", StringComparison.OrdinalIgnoreCase) ? Path.GetFullPath(Path.Combine(basePath, fileName)) : fileName;
                var referencedProject = Project.Load(location);
                var projectReference = new ProjectReference(Path.GetFileName(fileName), location, referencedProject);
                project._references.Add(projectReference);
            }
        }
    }

    public enum ReferenceType
    {
        Unknown,
        Project,
        Package,
        PackageWithoutVersion
    }

    public interface IReference
    {
        ReferenceType ReferenceType { get; }
    }

    public class ProjectReference : IReference
    {
        public ProjectReference(
            string name,
            string location,
            Project project
            )
        {
            Name = name;
            Location = location;
            Project = project;
        }

        public ReferenceType ReferenceType => ReferenceType.Project;
        
        public string Name { get; }
        public string Location { get; }

        public Project Project { get; }
    }

}
