using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;

public class PropertySchema
{
    public string Name { get; }
    public string Type { get; }
    public IEnumerable<AttributeSchema> Attributes { get; }

    public PropertySchema(string name, string type, IEnumerable<AttributeSchema> attributes)
    {
        Name = name;
        Type = type;
        Attributes = attributes;
    }
}
