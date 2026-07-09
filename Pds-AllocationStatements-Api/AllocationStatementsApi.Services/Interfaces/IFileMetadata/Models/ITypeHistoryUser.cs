namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;

/// <summary>
/// The user that is accessing the allocation.
/// </summary>
public interface ITypeHistoryUser
{
    /// <summary>
    /// Gets or sets user principle.
    /// </summary>
    string Principle { get; set; }
}