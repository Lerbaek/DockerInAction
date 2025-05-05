using IntegrationTests.Tests.Testcontainers.Fixtures;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
/// <summary>
/// xUnit collection definition for Client application tests using the hybrid Testcontainers approach.
/// <para>
/// This class defines a test collection that shares the <see cref="ClientTestFixtures"/> across all tests,
/// where the Client runs in-process and the Server runs in a Docker container.
/// </para>
/// </summary>
[CollectionDefinition(nameof(ClientTestcontainersCollection))]
public sealed class ClientTestcontainersCollection : ICollectionFixture<ClientTestFixtures>;