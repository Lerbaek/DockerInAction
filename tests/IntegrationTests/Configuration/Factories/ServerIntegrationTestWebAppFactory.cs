namespace IntegrationTests.Configuration.Factories;

/// <summary>
/// Integration test web application factory for the Server application.
/// <para>
/// This factory creates an in-memory test server hosting the Server application,
/// allowing tests to directly inspect and verify the Server's message consumption behavior.
/// </para>
/// </summary>
/// <remarks>
/// This factory is used primarily in hybrid testing approaches where the Server application
/// runs in-process while the Client and RabbitMQ run in containers. This approach provides:
/// <list type="bullet">
/// <item>Direct access to the Server's internal components and message consumers</item>
/// <item>The ability to mock or substitute Server dependencies</item>
/// <item>Detailed verification of message handling through log inspection or mocked services</item>
/// </list>
/// </remarks>
public class ServerIntegrationTestWebAppFactory : IntegrationTestWebAppFactory<Server.Program>;