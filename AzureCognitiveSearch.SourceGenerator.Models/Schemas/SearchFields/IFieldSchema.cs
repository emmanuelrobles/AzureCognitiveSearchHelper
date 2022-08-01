using System.Collections.Generic;

namespace AzureCognitiveSearch.SourceGenerator.Models.Schemas.SearchFields;

/// <summary>
/// A schema of a search field
/// </summary>
public interface IFieldSchema
{
    /// <summary>
    /// field name
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// field type
    /// </summary>
    public SchemaTypeEnum Type { get; }
}

/// <summary>
/// Represents a Complex field
/// </summary>
public interface IComplexFieldSchema : IFieldSchema
{
    /// <summary>
    /// Fields from the complex fields
    /// </summary>
    public IEnumerable<IFieldSchema> Fields { get; }
}

/// <summary>
/// Represents a Simple field
/// </summary>
public interface ISimpleFieldSchema : IFieldSchema
{
    /// <summary>
    /// List of properties
    /// </summary>
    public IEnumerable<SchemaPropertiesEnum> Properties { get; }
}


/// <summary>
/// Struct that represents a simple field
/// </summary>
internal readonly struct ValueSimpleFieldSchema : ISimpleFieldSchema
{
    public string Name { get; }
    public SchemaTypeEnum Type { get; }
    public IEnumerable<SchemaPropertiesEnum> Properties { get; }

    public ValueSimpleFieldSchema(string name, SchemaTypeEnum type, IEnumerable<SchemaPropertiesEnum> properties)
    {
        Name = name;
        Type = type;
        Properties = properties;
    }
}

/// <summary>
/// Struct that represent a complex field
/// </summary>
internal readonly struct ValueComplexFieldSchema : IComplexFieldSchema
{
    public string Name { get; }
    public SchemaTypeEnum Type { get; }
    public IEnumerable<IFieldSchema> Fields { get; }

    public ValueComplexFieldSchema(string name, SchemaTypeEnum type, IEnumerable<IFieldSchema> fields)
    {
        Name = name;
        Type = type;
        Fields = fields;
    }
}