using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <summary>
/// This class has no code, and is never created. Its purpose is simply
/// to be the place to apply <see cref="CollectionDefinitionAttribute"/> and all the
/// <see cref="ICollectionFixture{TFixture}"/>> interfaces.
/// </summary>
/// <remarks>
/// For more details go to: <see href="https://xunit.net/docs/shared-context"/>
/// </remarks>
[CollectionDefinition(nameof(ClientIntegrationTestWebAppFactoryCollection))]
public sealed class ClientIntegrationTestWebAppFactoryCollection : ICollectionFixture<ClientIntegrationTestWebAppFactory>;