using Xunit;

namespace IntegrationTests.Factories;

[CollectionDefinition(nameof(IntegrationTestWebAppFactoryCollection))]
public sealed class IntegrationTestWebAppFactoryCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    // For more details go to:
    // https://xunit.net/docs/shared-context
}