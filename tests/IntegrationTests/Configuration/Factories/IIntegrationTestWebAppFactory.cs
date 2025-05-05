using IntegrationTests.Configuration.Fixtures;

namespace IntegrationTests.Configuration.Factories;

/// <summary>
/// Defines the contract for integration test web application factories.
/// <para>
/// This interface provides methods for configuring test web applications
/// to use containerized dependencies, particularly RabbitMQ for messaging.
/// </para>
/// </summary>
public interface IIntegrationTestWebAppFactory
{
    /// <summary>
    /// Configures the application to use a containerized RabbitMQ instance.
    /// <para>
    /// This method connects the in-process web application to the specified
    /// RabbitMQ container, enabling integration testing with real message brokers.
    /// </para>
    /// </summary>
    /// <param name="rabbitMqFixture">The RabbitMQ container fixture providing connection details.</param>
    void ConfigureRabbitMq(RabbitMqFixture rabbitMqFixture);
}