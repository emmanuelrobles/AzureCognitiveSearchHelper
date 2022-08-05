using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.SearchFields;
using AzureCognitiveSearch.SourceGenerator.Models.Schemas.Templates;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.Helpers;

/// <summary>
/// Helpers function that build schemas
/// </summary>
public static class SchemaBuilderHelper
{
    /// <summary>
    /// Builds a namespace given a list of fields
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="nameSpaceName"></param>
    /// <param name="rootClassName"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    public static NamespaceSchema BuildNamespaceSchema(IEnumerable<IFieldSchema> fields, string nameSpaceName,
        string rootClassName, IEnumerable<string> usings)
    {
        // Filters and gets all the complex classes to create the schema from a complex field
        static IEnumerable<ClassSchema> BuildComplexClasses(IComplexFieldSchema complexFieldSchema)
            => complexFieldSchema.Fields.Where(e =>
                    e.Type is SchemaTypeEnum.Complex or SchemaTypeEnum.CollectionComplex)
                .SelectMany(complexSubSchema => BuildComplexClasses((IComplexFieldSchema)complexSubSchema))
                .Prepend(BuildClassSchema(complexFieldSchema.Fields, complexFieldSchema.Name));

        // return a namespace with root complex class
        return new NamespaceSchema(
            nameSpaceName, 
            BuildComplexClasses(new ValueComplexFieldSchema(rootClassName,SchemaTypeEnum.Complex,fields)),
            usings);
    }

    /// <summary>
    /// Builds a className given a list of fields
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    public static ClassSchema BuildClassSchema(IEnumerable<IFieldSchema> fields, string className)
        => new ClassSchema(className, fields.Select(BuildPropertySchema));

    /// <summary>
    /// Builds a Property given a field
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static PropertySchema BuildPropertySchema(IFieldSchema field)
    {
        static string SchemaTypeToString(IFieldSchema field)
            => field.Type switch
            {
                SchemaTypeEnum.String => "string",
                SchemaTypeEnum.Boolean => "bool",
                SchemaTypeEnum.Int32 => "int",
                SchemaTypeEnum.Int64 => "long",
                SchemaTypeEnum.Double => "double",
                SchemaTypeEnum.DateTimeOffset => "DateTime",
                SchemaTypeEnum.Complex => field.Name,
                SchemaTypeEnum.CollectionString => "IEnumerable<string>",
                SchemaTypeEnum.CollectionBoolean => "IEnumerable<bool>",
                SchemaTypeEnum.CollectionInt32 => "IEnumerable<int>",
                SchemaTypeEnum.CollectionInt64 => "IEnumerable<long>",
                SchemaTypeEnum.CollectionDouble => "IEnumerable<double>",
                SchemaTypeEnum.CollectionDateTimeOffset => "IEnumerable<DateTime>",
                SchemaTypeEnum.CollectionComplex => $"IEnumerable<{field.Name}>",
                SchemaTypeEnum.GeographicPoint => throw new NotSupportedException(),
                SchemaTypeEnum.CollectionGeographicPoint => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        static IEnumerable<AttributeSchema> FieldToAttributes(IFieldSchema schema)
            => schema.Type switch
            {
                SchemaTypeEnum.Complex or SchemaTypeEnum.CollectionComplex => Enumerable.Empty<AttributeSchema>(),
                _ => ((ISimpleFieldSchema)schema).Properties.Select(BuildPropertySchema)
            };

        return new PropertySchema(field.Name, SchemaTypeToString(field), FieldToAttributes(field));
    }

    /// <summary>
    /// Builds an Attribute given a property
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static AttributeSchema BuildPropertySchema(SchemaPropertiesEnum property)
        => new(property.ToString());
    
    
    /// <summary>
    /// Builds a field schema from a json stream
    /// </summary>
    /// <param name="jsonStream"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IEnumerable<IFieldSchema> BuildSchemaFromAzureJsonObject(Stream jsonStream)
    {
        var node = JsonNode.Parse(jsonStream);
        var fieldsArray = node?["fields"] as JsonArray ?? throw new NullReferenceException("");
        return fieldsArray.Select(AzureNodeToFieldSchema);
    }
    
    /// <summary>
    /// Build a list of field schemas from a json node
    /// </summary>
    /// <param name="jsonNode"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IEnumerable<IFieldSchema> BuildSchemaFromAzureJsonNode(JsonNode jsonNode)
    {
        var fieldsArray = jsonNode?["fields"] as JsonArray ?? throw new NullReferenceException("");
        return fieldsArray.Select(AzureNodeToFieldSchema);
    }

    /// <summary>
    /// json node to schema
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="NoNullAllowedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IFieldSchema AzureNodeToFieldSchema(JsonNode node)
    {
        static IEnumerable<SchemaPropertiesEnum> GetPropertiesFromNode(JsonNode node)
        {
            if (node["searchable"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Searchable;
            }

            if (node["filterable"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Filterable;
            }

            if (node["retrievable"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Retrievable;
            }

            if (node["sortable"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Sortable;
            }

            if (node["facetable"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Facetable;
            }

            if (node["key"]?.GetValue<bool>() ?? throw new NoNullAllowedException())
            {
                yield return SchemaPropertiesEnum.Key;
            }
        }

        static SchemaTypeEnum GetTypeFromNode(JsonNode node) =>
            node["type"]?.GetValue<string>() switch
            {
                "Edm.String" => SchemaTypeEnum.String,
                "Edm.Boolean" => SchemaTypeEnum.Boolean,
                "Edm.Int32" => SchemaTypeEnum.Int32,
                "Edm.Int64" => SchemaTypeEnum.Int64,
                "Edm.Double" => SchemaTypeEnum.Double,
                "Edm.DateTimeOffset" => SchemaTypeEnum.DateTimeOffset,
                "Edm.GeographyPoint" => SchemaTypeEnum.GeographicPoint,
                "Edm.ComplexType" => SchemaTypeEnum.Complex,

                "Collection(Edm.String)" => SchemaTypeEnum.CollectionString,
                "Collection(Edm.Boolean)" => SchemaTypeEnum.CollectionBoolean,
                "Collection(Edm.Int32)" => SchemaTypeEnum.CollectionInt32,
                "Collection(Edm.Int64)" => SchemaTypeEnum.CollectionInt64,
                "Collection(Edm.Double)" => SchemaTypeEnum.CollectionDouble,
                "Collection(Edm.DateTimeOffset)" => SchemaTypeEnum.CollectionDateTimeOffset,
                "Collection(Edm.GeographyPoint)" => SchemaTypeEnum.CollectionGeographicPoint,
                "Collection(Edm.ComplexType)" => SchemaTypeEnum.CollectionComplex,
                _ => throw new ArgumentOutOfRangeException("type")
            };

        var nodeType = GetTypeFromNode(node);
        var name = node["name"]?.GetValue<string>();
        return nodeType switch
        {
            SchemaTypeEnum.Complex or SchemaTypeEnum.CollectionComplex =>
                new ValueComplexFieldSchema(name, nodeType, BuildSchemaFromAzureJsonNode(node)),
            _ => new ValueSimpleFieldSchema(node["name"]?.GetValue<string>(), nodeType,
                GetPropertiesFromNode(node))
        };
    }
}
