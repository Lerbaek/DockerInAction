using IntegrationTests.Tests.Testcontainers.Client;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(ClientTestcontainersCollection))]
public sealed class ClientTestcontainersCollection : ICollectionFixture<ClientTestFixtures>;