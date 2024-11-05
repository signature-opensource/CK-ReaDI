namespace CK.Core;

/// <summary>
/// Generalizes <see cref="ReaDIResult"/> and <see cref="ReaDIResult{T}"/>.
/// </summary>
public interface IReaDIResult
{
    /// <summary>
    /// Gets whether an error occurred. When true, there is nothing more to do.
    /// </summary>
    bool HasError { get; }

    /// <summary>
    /// Gets whether the current call should be retried during the next pass.
    /// </summary>
    bool IsRetry { get; }

    /// <summary>
    /// Gets whether the call succeed.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the name of a method (that must be a private [ReadDI] method) continuation.
    /// </summary>
    string? MethodName { get; }

    /// <summary>
    /// Gets the <see cref="ReaDIService"/> that may have been created.
    /// </summary>
    ReaDIService? Created { get; }
}
