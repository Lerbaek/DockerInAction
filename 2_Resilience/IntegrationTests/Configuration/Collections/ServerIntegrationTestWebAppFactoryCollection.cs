using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientIntegrationTestWebAppFactoryCollection"/>
[CollectionDefinition(nameof(ServerIntegrationTestWebAppFactoryCollection))]
public sealed class ServerIntegrationTestWebAppFactoryCollection : ICollectionFixture<ServerIntegrationTestWebAppFactory>;