using System.Linq;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;
using Scriban;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Helpers;

public static class SchemaGeneratorHelper
{
    /// <summary>
    /// Generates attribute class string
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="namespaceValue"></param>
    /// <returns></returns>
    public static string GenerateAttribute(AttributeSchema schema, string namespaceValue)
    {
        var templateString = ResourceReaderHelper.GetResource("attribute_template.sbncs");
        var template = Template.Parse(templateString);
        return template.Render(new { namespacevalue = namespaceValue, name = schema.Name});
    }
    
    /// <summary>
    /// Generate an schema from a namespace
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static string GenerateNamespaceSchema(NamespaceSchema schema)
    {
        var templateString = ResourceReaderHelper.GetResource("namespace_template.sbncs");
        var template = Template.Parse(templateString);
        return template.Render(new {@namespace = schema});
    }
    
    /// <summary>
    /// Generate an schema from an enum
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static string GenerateEnumSchema(EnumSchema schema)
    {
        var templateString = ResourceReaderHelper.GetResource("enum_template.sbncs");
        var template = Template.Parse(templateString);
        return template.Render(new {template = schema});
    }
}
