using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates
{
    public class NamespaceSchema
    {
        public string Name { get; }

        public IEnumerable<ClassSchema> Classes { get; }

        public NamespaceSchema(string name, IEnumerable<ClassSchema> classes)
        {
            Name = name;
            Classes = classes;
        }
    }
}
