using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Helpers;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.SearchFields;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban.Parsing;

namespace AzureCognitiveSearch.SourceGenerator.Models.Generators;

[Generator]
public class GeneratorV1 : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        var rootNamespace = "AzureGenerated";
        var rootClass = "Root";
        
        // Generating attributes
        foreach (var name in Enum.GetNames(typeof(SchemaPropertiesEnum)))
        {
            var attributeFileText = SchemaGeneratorHelper.GenerateAttribute(new AttributeSchema(name), rootNamespace);
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                $"{name}.g.cs",
                SourceText.From(attributeFileText, Encoding.UTF8)));
        }
        
        
        var schemas = 
            SchemaBuilderHelper.BuildSchemaFromAzureJsonObject(File.OpenRead("/home/emmanuel/Documents/azureSchema.txt")).ToArray();

        // build a namespace schema
        var namespaceSchema = SchemaBuilderHelper.BuildNamespaceSchema(
            schemas, 
            rootNamespace, 
            rootClass, 
            new []{$"{rootNamespace}.Attributes"});
        
        // render the namespace schema
        var namespaceText = SchemaGeneratorHelper.GenerateNamespaceSchema(namespaceSchema);
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "AzureModel.g.cs",
            SourceText.From(namespaceText, Encoding.UTF8)));

        static IEnumerable<(IEnumerable<string> parents, ISimpleFieldSchema schema)> FlattenSchemasWith(
            IEnumerable<IFieldSchema> schemas, IEnumerable<string> parents, HashSet<string> visited)
        => schemas
            .Where(s => s is ISimpleFieldSchema)
            .Select(s =>
            {
                var simpleSchema = (ISimpleFieldSchema)s;
                return (parents, simpleSchema);
            }).Concat(schemas
                .Where(s => s is IComplexFieldSchema && !visited.Contains(s.Name))
                .SelectMany(s =>
                {
                    var complexSchema = (IComplexFieldSchema)s;
                    visited.Add(complexSchema.Name);
                    return FlattenSchemasWith(complexSchema.Fields,parents.Append(complexSchema.Name),visited);
                }));


        // all schemas to generate
        var allSchemas = FlattenSchemasWith(schemas, Enumerable.Empty<string>(), new HashSet<string>());

        // creates the enum schema
        static IEnumerable<(SchemaPropertiesEnum schemaEnum,EnumSchema schema)> GenerateEnumSchema(IEnumerable<(IEnumerable<string> parents, ISimpleFieldSchema schema)> schemas, string rootNamespace, string rootClass)
        {
            // dictionary with properties and their value
            var dict = 
                Enum.GetValues(typeof(SchemaPropertiesEnum))
                    .Cast<SchemaPropertiesEnum>()
                    .ToDictionary(value => value, value => new EnumSchema(rootNamespace, $"{rootClass}{value.ToString()}", Enumerable.Empty<EnumValueSchema>()));
            
            foreach (var schema in schemas)
            {
                foreach (var schemaPropertiesEnum in schema.schema.Properties)
                {
                    var oldVal = dict[schemaPropertiesEnum];
                    dict[schemaPropertiesEnum] =
                        new EnumSchema(oldVal.Namespace, oldVal.Name,
                            oldVal.Values
                                .Append(
                                    new EnumValueSchema(string.Join("_", schema.parents.Append(schema.schema.Name)), 
                                string.Join("/",schema.parents.Append(schema.schema.Name))))
                            );
                }
            }

            return dict.Select(e => (e.Key,e.Value));
        }
        
        var enumsSchemas = GenerateEnumSchema(allSchemas, rootNamespace,rootClass);
        
        foreach (var es in enumsSchemas)
        {
            var attributeFileText = SchemaGeneratorHelper.GenerateEnumSchema(es.schema);
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                $"{es.schemaEnum}_enum.g.cs",
                SourceText.From(attributeFileText, Encoding.UTF8)));
        }
    }

    static PropertyDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, string propertyType)
    {
        var propDeclarationSyntax = (PropertyDeclarationSyntax)context.Node;
        
        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in propDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                IMethodSymbol attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
                if (attributeSymbol == null)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == propertyType)
                {
                    // return the parent class of the method
                    return propDeclarationSyntax;
                }
            }
        }

        return null;
    }
    
    private static void Execute(Compilation compilation, ImmutableArray<PropertyDeclarationSyntax> prop, SourceProductionContext context)
    {
        if (prop.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        IEnumerable<PropertyDeclarationSyntax> distinctClasses = prop.Distinct();

        Console.WriteLine();
    }

}
