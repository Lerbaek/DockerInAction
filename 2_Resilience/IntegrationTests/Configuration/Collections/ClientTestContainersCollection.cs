using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;
using IntegrationTests.Tests.Testcontainers.Client;
using Server.Configuration;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(ClientTestcontainersCollection))]
public sealed class ClientTestcontainersCollection
    : ICollectionFixture<ClientIntegrationTestWebAppFactory>,
      ICollectionFixture<RabbitMqFixture<ClientTests>>,
      ICollectionFixture<ServerFixture<ClientTests>>;