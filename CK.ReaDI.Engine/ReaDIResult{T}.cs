namespace CK.Core;

/// <summary>
/// Defines the outcome of a ReaDI method that produces a new <see cref="ReaDIService"/>.
/// methods:
/// <list type="bullet">
///     <item>
///     The call is successful (use <see cref="Success(T)"/>).
///     </item>
///     <item>
///     The call failed (use <see cref="Failed"/>).
///     </item>
///     <item>
///     The same method must be retried during the next pass (use <see cref="Retry"/>).
///     </item>
///     <item>
///     Another ReaDI method on the same object must be called: use
///     <see cref="SuccessAndContinueWith(T, string)"/> with the method name.
///     </item>
/// </list>
/// </summary>
public sealed class ReaDIResult<T> : IReaDIResult where T : ReaDIService
{
    readonly string? _methodName;
    readonly int _flag;
    readonly T? _created;

    /// <inheritdoc />
    public bool HasError => _flag == 0;

    /// <inheritdoc />
    public bool IsSuccess => _flag == 1;

    /// <inheritdoc />
    public bool IsRetry => _flag == 2;

    /// <inheritdoc />
    public string? MethodName => _methodName;

    /// <inheritdoc />
    public ReaDIService? Created => _created;

    /// <summary>
    /// Express a failed final result.
    /// </summary>
    public static readonly ReaDIResult<T> Failed = new ReaDIResult<T>( 0, null );

    /// <summary>
    /// Express a failed final result.
    /// </summary>
    public static readonly ReaDIResult<T> Retry = new ReaDIResult<T>( 2, null );

    /// <summary>
    /// Returns a successful final result with the created <see cref="ReaDIService"/>.
    /// </summary>
    public static ReaDIResult<T> Success( T created )
    {
        return new ReaDIResult<T>( created, null );
    }

    /// <summary>
    /// Returns a successful final result with the created <see cref="ReaDIService"/> and asks for a continuation.
    /// See <see cref="MethodName"/>.
    /// </summary>
    /// <param name="unambiguousMethodName">The name of the method to call.</param>
    public static ReaDIResult<T> SuccessAndContinueWith( T created, string unambiguousMethodName )
    {
        Throw.CheckNotNullOrWhiteSpaceArgument( unambiguousMethodName );
        return new ReaDIResult<T>( created, unambiguousMethodName );
    }

    ReaDIResult( int f, T? created )
    {
        _flag = f;
        _created = created;
    }

    ReaDIResult( T created, string? unambiguousMethodName )
    {
        Throw.CheckNotNullArgument( created );
        _methodName = unambiguousMethodName;
        _flag = 1;
        _created = created;
    }
}
