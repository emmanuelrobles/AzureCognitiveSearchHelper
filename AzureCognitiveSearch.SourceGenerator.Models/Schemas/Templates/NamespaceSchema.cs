using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates
{
    public class NamespaceSchema
    {
        public string Name { get; }

        public IEnumerable<ClassSchema> Classes { get; }
        public IEnumerable<string> Usings { get; }

        public NamespaceSchema(string name, IEnumerable<ClassSchema> classes, IEnumerable<string> usings)
        {
            Name = name;
            Classes = classes;
            Usings = usings;
        }
    }
}
