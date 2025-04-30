using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(ServerTestHarnessCollection))]
public sealed class ServerTestHarnessCollection : ICollectionFixture<ServerIntegrationTestWebAppFactory>;