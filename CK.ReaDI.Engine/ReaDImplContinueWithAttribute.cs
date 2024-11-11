using System;

namespace CK.Core;

/// <summary>
/// Required attribute on methods that returns a <see cref="ReaDIResult"/> (or <see cref="ReaDIResult{T}"/>)
/// with <see cref="ReaDIResult.SuccessAndContinueWith(string)"/> (or <see cref="ReaDIResult{T}.SuccessAndContinueWith(T, string)"/>).
/// <para>
/// Methods decorated with this attribute cannot only succeed, they must continue with the specified method.
/// </para>
/// This attribute excludes <see cref="ReaDImplContinueWithAnyAttribute"/> and <see cref="ReaDImplMayContinueWithAttribute"/>.
/// </summary>
[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false )]
public sealed class ReaDImplContinueWithAttribute : Attribute
{
    public ReaDImplContinueWithAttribute( string methodName )
    {
        MethodName = methodName;
    }

    /// <summary>
    /// Gets the private method name that will necessarily be called by the decorated method.
    /// </summary>
    public string MethodName { get; }
}

