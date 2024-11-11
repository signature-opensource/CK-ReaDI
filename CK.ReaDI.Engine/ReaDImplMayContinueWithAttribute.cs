using System;

namespace CK.Core;

/// <summary>
/// Required attribute on methods that returns a <see cref="ReaDIResult"/> (or <see cref="ReaDIResult{T}"/>)
/// with <see cref="ReaDIResult.SuccessAndContinueWith(string)"/> (or <see cref="ReaDIResult{T}.SuccessAndContinueWith(T, string)"/>).
/// <para>
/// Methods deocrated with this attributes may succeed without continuation or succeed and continue with one of the specified methods.
/// </para>
/// This attribute excludes <see cref="ReaDImplContinueWithAttribute"/> and <see cref="ReaDImplContinueWithAnyAttribute"/>.
/// </summary>
[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false )]
public sealed class ReaDImplMayContinueWithAttribute : Attribute
{
    public ReaDImplMayContinueWithAttribute( params string[] methodNames )
    {
        MethodNames = methodNames;
    }

    /// <summary>
    /// Gets the private method names continuations that may be called by the decorated method.
    /// </summary>
    public string[] MethodNames { get; }
}

