# Sample on how to use the Azure cognitive search DI

Small example app on how to use the dependency injection

## Dependencies

- (Required) Azure.Search.Documents -> [nuget](https://www.nuget.org/packages/Azure.Search.Documents)

## How to use DI

- Create the entity model, the one on the example is ```AzureModel```.
- We need to create a Service Index client like the one showing in **Program.cs**
```csharp
// create a search index client
var searchIndexClient = new SearchIndexClient(
    endpoint: new Uri("__URI__"),
    new AzureKeyCredential("__KEY__"));

// Add context using fluent api
builder.Services.AddAzureSearchContext(searchIndexClient, instanceBuilder:() => new AzureContextFluentApi("__INDEX_NAME__"));

// Add context using attributes
builder.Services.AddAzureSearchContext<AzureContextAttributes>(searchIndexClient);
```
- Create a context
- Inject the context somewhere :)


### Context types

all context should have an ```IAzureQueryable<>``` that 

#### Fluent API context

Create a context that implements ```IAzureContextModelBuilder```, 
you will have to override ```BuildModel``` method, this is a simple example
on how it can be done

```csharp
public IEnumerable<IProperty> BuildModel(IModelBuilder modelBuilder)
    {
        modelBuilder
            // property to modify      
            .SetPropertySettings<AzureContextFluentApi, AzureModel>(context => context.Set)
            // sets an index name
            .WithIndexName(_indexName);
        return modelBuilder.Properties;
    }
```

#### Attribute context

Create a context that implements ```IAzureContext```, 
create property a public property with a setter, add ```AzureIndexName``` and
pass a parameter with the index name for that property 

```csharp
public class AzureContextAttributes : IAzureContext
{
    /// <summary>
    /// Azure set that you can query on
    /// </summary>
    [AzureIndexName("__INDEX_NAME__")]
    public IAzureQueryable<AzureModel> Set { get; set; }
}
```




