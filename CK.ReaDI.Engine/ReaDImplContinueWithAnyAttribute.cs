using System;

namespace CK.Core;

/// <summary>
/// Required attribute on methods that returns a <see cref="ReaDIResult"/> (or <see cref="ReaDIResult{T}"/>)
/// with <see cref="ReaDIResult.SuccessAndContinueWith(string)"/> (or <see cref="ReaDIResult{T}.SuccessAndContinueWith(T, string)"/>).
/// <para>
/// Methods deocrated with this attributes cannot only succeed, they must continue with one of the specified methods.
/// </para>
/// This attribute excludes <see cref="ReaDImplContinueWithAttribute"/> and <see cref="ReaDImplMayContinueWithAttribute"/>.
/// </summary>
[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false )]
public sealed class ReaDImplContinueWithAnyAttribute : Attribute
{
    public ReaDImplContinueWithAnyAttribute( params string[] methodNames )
    {
        MethodNames = methodNames;
    }

    /// <summary>
    /// Gets the private method names continuations: one of them will necessarily be called by the decorated method.
    /// </summary>
    public string[] MethodNames { get; }
}

