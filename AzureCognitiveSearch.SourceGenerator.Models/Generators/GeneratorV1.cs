using System;
using System.Text;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Helpers;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.SearchFields;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AzureCognitiveSearch.SourceGenerator.Models.Generators;

[Generator]
public class GeneratorV1 : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Generating attributes
        foreach (var name in Enum.GetNames(typeof(SchemaPropertiesEnum)))
        {
            var attributeFileText = SchemaGeneratorHelper.GenerateAttribute(new AttributeSchema(name), "AzureGenerated.Attributes");
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                $"{name}.g.cs",
                SourceText.From(attributeFileText, Encoding.UTF8)));
        }
    }
}
