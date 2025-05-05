using IntegrationTests.Tests.Testcontainers.Fixtures;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
/// <summary>
/// xUnit collection definition for end-to-end tests using Testcontainers.
/// <para>
/// This class defines a test collection that shares the <see cref="EndToEndTestFixtures"/> across all tests,
/// where both Client and Server run in Docker containers for full integration testing.
/// </para>
/// </summary>
[CollectionDefinition(nameof(EndToEndTestcontainersCollection))]
public sealed class EndToEndTestcontainersCollection : ICollectionFixture<EndToEndTestFixtures>;