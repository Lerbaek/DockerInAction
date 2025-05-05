using IntegrationTests.Tests.Testcontainers.Fixtures;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
/// <summary>
/// xUnit collection definition for Server application tests using the hybrid Testcontainers approach.
/// <para>
/// This class defines a test collection that shares the <see cref="ServerTestFixtures"/> across all tests,
/// where the Server runs in-process and the Client runs in a Docker container.
/// </para>
/// </summary>
[CollectionDefinition(nameof(ServerTestcontainersCollection))]
public sealed class ServerTestcontainersCollection : ICollectionFixture<ServerTestFixtures>;