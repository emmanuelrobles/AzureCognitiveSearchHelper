using Azure;
using Azure.Search.Documents.Indexes;
using AzureCognitiveSearch.DI.FromNugetPackage;
using Sample.AzureCognitiveSearch.DI.Contexts;
using Sample.AzureCognitiveSearch.DI.Contexts.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// create a search index client
var searchIndexClient = new SearchIndexClient(
    endpoint: new Uri("__URI__"),
    new AzureKeyCredential("__KEY__"));

// Example 1: Add context using fluent api
builder.Services.AddAzureSearchContext(searchIndexClient, instanceBuilder:() => new AzureContextFluentApi("__INDEX_NAME__"));

// Example 2: Add context using attributes
builder.Services.AddAzureSearchContext<AzureContextAttributes>(searchIndexClient);

// Example 3: Add querable data model without a context
builder.Services.AddAzureSearch(new Uri("__URI__"), new AzureKeyCredential("__KEY__"))
                .WithQueryableIndex<AzureModel>("__INDEX_NAME__");

// Optionally create and register a custom application context to aggreate all Azure Models
builder.Services.AddScoped<AzureSearchContext>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
