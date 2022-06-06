# Azure cognitive search DI

Dependency injection package

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

## Sample

This is a sample solution [DI sample](https://github.com/emmanuelrobles/AzureCognitiveSearchHelper/tree/master/Sample.AzureCognitiveSearch.DI)

# Azure cognitive search DI (v2 - preview)

## Features
 - Provides fluent DI services to configure queryable Azure search services

### Step 1 - Model index data

Create a class to represent the azure search index document:

 ```csharp  
 
public class AzureModelIndex
{

}

```

### Step 2 - Register queryable services with fluent DI extensions

Add DI services as follows:

```csharp   
services.AddAzureSearch(new Uri("https://your-endpoint.search.windows.net"), new AzureKeyCredential("--your-azure-key--"))
        .WithQueryableIndex<AzureModelIndex>("--your-index-name--");

```

### Step 3 - Resolve queryable services

Resolve your azure queryable for the specified model using constructor injection where you see fit as follows:

```csharp 

public class DataContext
{
    private IAzureQueryable<AzureModelIndex> _baseQuery;

    public AzureContext(IAzureQueryable<AzureModelIndex> queryable)
    {
        _baseQuery = queryable;
    }
}

```
Note: Leverage fluent operations and build custom DSL to compose search index queries. 



