using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;

public class ClassSchema
{
    public string Name { get; }
    public IEnumerable<PropertySchema> Properties { get; }

    public ClassSchema(string name, IEnumerable<PropertySchema> properties)
    {
        Name = name;
        Properties = properties;
    }
}
