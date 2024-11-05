namespace CK.Core;

/// <summary>
/// Defines the outcome of a ReaDI method.
/// methods:
/// <list type="bullet">
///     <item>
///     The call is successful (use <see cref="Success"/>).
///     </item>
///     <item>
///     The call failed (use <see cref="Failed"/>).
///     </item>
///     <item>
///     The same method must be retried during the next pass (use <see cref="Retry"/>).
///     </item>
///     <item>
///     Another ReaDI method on the same object must be called: use
///     <see cref="SuccessAndContinueWith(string)"/> with the method name.
///     </item>
/// </list>
/// </summary>
public sealed class ReaDIResult : IReaDIResult
{
    readonly string? _methodName;
    readonly int _flag;

    /// <inheritdoc />
    public bool HasError => _flag == 0;

    /// <inheritdoc />
    public bool IsSuccess => _flag == 1;

    /// <inheritdoc />
    public bool IsRetry => _flag == 2;

    /// <inheritdoc />
    public string? MethodName => _methodName;

    ReaDIService? IReaDIResult.Created => null;

    /// <summary>
    /// Express a failed result.
    /// </summary>
    public static readonly ReaDIResult Failed = new ReaDIResult( 0 );

    /// <summary>
    /// Express a successful result.
    /// </summary>
    public static readonly ReaDIResult Success = new ReaDIResult( 1 );

    /// <summary>
    /// Asks for a retry on the current method.
    /// </summary>
    public static readonly ReaDIResult Retry = new ReaDIResult( 2 );

    /// <summary>
    /// Returns a successful final result and asks for a continuation.
    /// See <see cref="MethodName"/>.
    /// </summary>
    /// <param name="unambiguousMethodName">The name of the method to call.</param>
    public static ReaDIResult SuccessAndContinueWith( string unambiguousMethodName )
    {
        Throw.CheckNotNullOrWhiteSpaceArgument( unambiguousMethodName );
        return new ReaDIResult( unambiguousMethodName );
    }

    ReaDIResult( int f )
    {
        _methodName = null;
        _flag = f;
    }

    ReaDIResult( string unambiguousMethodName )
    {
        _methodName = unambiguousMethodName;
        _flag = 1;
    }

}
