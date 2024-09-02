namespace Shared;

public record Payment(string CardNumber, string ExpiryDate, string Cvv, string Amount, string Currency);