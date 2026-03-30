using Xunit;

namespace DuckBill.IntegrationTests;

[CollectionDefinition("API collection")]
public class ApiTestCollection : ICollectionFixture<ApiFactory>
{
}
