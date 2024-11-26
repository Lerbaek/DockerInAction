using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;
using IntegrationTests.Tests.Testcontainers.EndToEnd;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(EndToEndTestcontainersCollection))]
public sealed class EndToEndTestcontainersCollection
    : ICollectionFixture<RabbitMqFixture<EndToEndTests>>,
      ICollectionFixture<ServerFixture<EndToEndTests>>,
      ICollectionFixture<ClientFixture<EndToEndTests>>;