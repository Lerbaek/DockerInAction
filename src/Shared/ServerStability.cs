namespace Shared;

/// <summary>
/// Defines the stability modes that control how the Server processes payments.
/// <para>
/// This enum is used as a header value in messages and HTTP requests to simulate
/// different server response patterns for testing error handling and resilience.
/// </para>
/// </summary>
public enum ServerStability
{
    /// <summary>
    /// Server processes all payments successfully without errors.
    /// </summary>
    Functional,
    
    /// <summary>
    /// Server randomly succeeds or fails (with 50% probability based on system clock ticks).
    /// </summary>
    Flaky,
    
    /// <summary>
    /// Server always fails with an exception when processing payments.
    /// </summary>
    Failing
}