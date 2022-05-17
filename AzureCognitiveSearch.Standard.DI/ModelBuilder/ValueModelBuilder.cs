using System.Reflection;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

internal struct ValueModelBuilder : IModelBuilder
{
    public ValueModelBuilder()
    {
    }
    public IEnumerable<IProperty> Properties { get; set; } = Enumerable.Empty<IProperty>();
}
