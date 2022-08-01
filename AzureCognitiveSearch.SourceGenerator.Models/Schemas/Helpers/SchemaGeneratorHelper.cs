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
}
