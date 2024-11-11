using CK.ReaDI.Engine;
using System;
using System.Runtime.CompilerServices;

namespace CK.Core;

/// <summary>
/// ReaDI implementation for dynamic ReaDI services.
/// These objects are created by a <c>[ReaDImpl]</c> method that return a <see cref="ReaDIResult{T}"/>
/// or by <see cref="ReaDIServiceBuilder{T}"/>.
/// <para>
/// Specialized classes must be <c>sealed</c> (specialization is prohitited).
/// </para>
/// <para>
/// Strict reference equality is used and cannot be changed. 
/// </para>
/// </summary>
public abstract class ReaDIService
{
    internal string? _reaDIName;

    /// <summary>
    /// Seals the use of <see cref="RuntimeHelpers.GetHashCode(object?)"/>.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override sealed int GetHashCode() => RuntimeHelpers.GetHashCode( this );

    /// <summary>
    /// Seals the use of referencial equality.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if <paramref name="obj"/> is the same as this object.</returns>
    public override bool Equals( object? obj ) => ReferenceEquals( obj, this );
}

public enum ReaDIServiceBinding
{
    /// <summary>
    /// No binding to a type or a member. This is the default.
    /// </summary>
    None,
    /// <summary>
    /// The ReaDIService is bound to a <see cref="ReaDITypeHost"/>.
    /// </summary>
    Type,
    /// <summary>
    /// The ReaDIService is bound to a <see cref="ReaDIMemberHost"/>.
    /// </summary>
    Member
}


public abstract class ReaDITypeService
{
    internal string? _reaDIName;

    /// <summary>
    /// Seals the use of <see cref="RuntimeHelpers.GetHashCode(object?)"/>.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override sealed int GetHashCode() => RuntimeHelpers.GetHashCode( this );

    /// <summary>
    /// Seals the use of referencial equality.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if <paramref name="obj"/> is the same as this object.</returns>
    public override bool Equals( object? obj ) => ReferenceEquals( obj, this );
}

public abstract class ReaDIMemberService
{
    internal string? _reaDIName;

    /// <summary>
    /// Seals the use of <see cref="RuntimeHelpers.GetHashCode(object?)"/>.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override sealed int GetHashCode() => RuntimeHelpers.GetHashCode( this );

    /// <summary>
    /// Seals the use of referencial equality.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if <paramref name="obj"/> is the same as this object.</returns>
    public override bool Equals( object? obj ) => ReferenceEquals( obj, this );
}

