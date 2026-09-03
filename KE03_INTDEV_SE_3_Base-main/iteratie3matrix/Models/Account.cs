namespace iteratie3matrix.Models;

/// <summary>
/// WHAT:
/// Represents a customer account.
///
/// WHY:
/// Used to replace raw AccountId in UI with meaningful data.
/// </summary>
public class Account
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public int Points { get; set; }
    public bool IsActive { get; set; }
}