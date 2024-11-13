using CK.ReaDI.Engine;
using System;
using System.Runtime.CompilerServices;

namespace CK.Core;

/// <summary>
/// Internal interface marker.
/// </summary>
interface IReaDIService { }

/// <summary>
/// Internal interface marker.
/// </summary>
interface IReaDITypeBoundService : IReaDIService { }

/// <summary>
/// Internal interface marker.
/// </summary>
interface IReaDIMemberBoundService : IReaDITypeBoundService { }


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
public abstract class ReaDIService : IReaDIService 
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
    public override sealed bool Equals( object? obj ) => ReferenceEquals( obj, this );
}


public abstract class ReaDITypeBoundService : IReaDITypeBoundService
{
    readonly ReaDITypeHost _typeHost;

    protected ReaDITypeBoundService( ReaDITypeHost typeHost )
    {
        Throw.CheckNotNullArgument( typeHost );
        _typeHost = typeHost;
    }

    public ReaDITypeHost TypeHost => _typeHost;

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

public abstract class ReaDIMemberBoundService : IReaDIMemberBoundService
{
    readonly ReaDIMemberHost _memberHost;

    protected ReaDIMemberBoundService( ReaDIMemberHost memberHost )
    {
        Throw.CheckNotNullArgument( memberHost );
        _memberHost = memberHost;
    }

    public ReaDIMemberHost MemberHost => _memberHost;

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

