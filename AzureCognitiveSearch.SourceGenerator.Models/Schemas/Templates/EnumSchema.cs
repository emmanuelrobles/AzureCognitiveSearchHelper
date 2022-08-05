using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;

public class EnumSchema
{
    public string Namespace { get; }

    public string Name { get; }

    public IEnumerable<EnumValueSchema> Values { get; }

    public EnumSchema(string ns, string name, IEnumerable<EnumValueSchema> values)
    {
        Namespace = ns;
        Name = name;
        Values = values;
    }
}


public class EnumValueSchema
{
    public string EnumValue { get; }
    
    public string StringValue { get; }

    public EnumValueSchema(string enumValue, string stringValue)
    {
        EnumValue = enumValue;
        StringValue = stringValue;
    }
}
