namespace Shared;

/// <summary>
/// Represents a payment transaction.
/// <para>
/// This record contains the essential details of a payment used for
/// demonstrating message passing between the Client and Server applications.
/// </para>
/// </summary>
/// <param name="CardNumber">The payment card number.</param>
/// <param name="ExpiryDate">The expiration date of the payment card.</param>
/// <param name="Cvv">The card verification value.</param>
/// <param name="Amount">The payment amount as a string.</param>
/// <param name="Currency">The payment currency code.</param>
public record Payment(string CardNumber, string ExpiryDate, string Cvv, string Amount, string Currency);