using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(ServerTestcontainersCollection))]
public sealed class ServerTestcontainersCollection : ICollectionFixture<ServerIntegrationTestWebAppFactory>;