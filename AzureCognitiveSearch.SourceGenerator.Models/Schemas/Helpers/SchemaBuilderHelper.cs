using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <returns></returns>
    public static NamespaceSchema BuildNamespaceSchema(IEnumerable<IFieldSchema> fields, string nameSpaceName,
        string rootClassName)
    {
        // Filters and gets all the complex classes to create the schema from a complex field
        static IEnumerable<ClassSchema> BuildComplexClasses(IComplexFieldSchema complexFieldSchema)
            => complexFieldSchema.Fields.Where(e =>
                    e.Type is SchemaTypeEnum.Complex or SchemaTypeEnum.CollectionComplex)
                .SelectMany(complexSubSchema => BuildComplexClasses((IComplexFieldSchema)complexSubSchema))
                .Prepend(BuildClassSchema(complexFieldSchema.Fields, complexFieldSchema.Name));

        // return a namespace with root complex class
        return new NamespaceSchema(nameSpaceName, BuildComplexClasses(new ValueComplexFieldSchema(rootClassName,SchemaTypeEnum.Complex,fields)));
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
}
