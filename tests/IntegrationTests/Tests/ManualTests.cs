using System.Diagnostics;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using FluentAssertions;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Tests
{
    /// <summary>
    /// Contains proof-of-concept tests demonstrating basic Testcontainers usage.
    /// <para>
    /// This class serves as a minimal example of how to manually create and interact with containerized 
    /// dependencies during test execution, separate from the more structured test fixture approach.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Unlike the structured test fixtures in the Testcontainers namespace, these tests show the raw 
    /// container creation process, which is useful for experimentation, debugging, or creating one-off tests.
    /// </remarks>
    public class ManualTests
    {
        /// <summary>
        /// Demonstrates how to start a RabbitMQ container for manual testing and debugging.
        /// <para>
        /// This test:
        /// </para>
        /// <list type="number">
        /// <item>Creates a RabbitMQ container with the management plugin</item>
        /// <item>Starts the container and maps its management port</item>
        /// <item>When a debugger is attached, opens the RabbitMQ management UI in a browser and breaks execution</item>
        /// </list>
        /// <para>
        /// This allows developers to manually interact with RabbitMQ's management interface during test execution,
        /// which is valuable for exploratory testing, debugging message flows, or understanding RabbitMQ behavior.
        /// </para>
        /// </summary>
        [Fact]
        public async Task StartRabbitMq()
        {
            // Arrange
            const int rabbitMqManagementPort = 15672;
            TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.11.0");
            var container = new RabbitMqBuilder()
                .WithImage("rabbitmq:3.11.20-management")
                .WithPortBinding(rabbitMqManagementPort, assignRandomHostPort: true)
                .WithName($"rabbitmq-{Guid.NewGuid()}")
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Integration")
                .Build();

            // Act
            await container.StartAsync();

            if (Debugger.IsAttached)
            {
                Process.Start(new ProcessStartInfo(
                    $"http://{container.Hostname}:{container.GetMappedPublicPort(rabbitMqManagementPort)}")
                {
                    UseShellExecute = true
                });

                // Username: rabbitmq
                // Password: rabbitmq

                Debugger.Break();
            }

            // Assert
            container.State.Should().Be(TestcontainersStates.Running);
        }
    }
}
