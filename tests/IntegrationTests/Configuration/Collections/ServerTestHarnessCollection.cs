using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
/// <summary>
/// xUnit collection definition for Server application tests using the in-memory test harness approach.
/// <para>
/// This class defines a test collection that shares the <see cref="ServerIntegrationTestWebAppFactory"/>
/// fixture across all tests in the collection.
/// </para>
/// </summary>
[CollectionDefinition(nameof(ServerTestHarnessCollection))]
public sealed class ServerTestHarnessCollection : ICollectionFixture<ServerIntegrationTestWebAppFactory>;