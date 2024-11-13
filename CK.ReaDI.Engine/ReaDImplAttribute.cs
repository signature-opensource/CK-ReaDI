using CK.ReaDI.Engine;
using System;

namespace CK.Core;

/// <summary>
/// When applied to a class, this defines a ReaDI entry point implementation, a target of a source <see cref="ReaDIAttribute"/>.
/// This implementation class can be public or internal and:
/// <list type="bullet">
///     <item>
///     It must be <c>sealed</c>.
///     </item>
///     <item>
///     Can have at most one constructor also decorated with <c>[ReadImpl]</c>.
///     <para>
///     This <c>[ReadImpl]</c> constructor can declare the following parameters:
///     <list type="bullet">
///         <item>
///         A single optional <see cref="ReaDIAttribute"/> type that is the source <c>[ReaDI]</c> attribute.
///         </item>
///         <item>
///         A single optional <see cref="ICachedItem"/>. This is always available, it is the either the Type or the
///         Method, Property or Event decorated by the <c>[ReaDI]</c> attribute (an item is a <see cref="ICachedType"/>
///         xor a <see cref="ICachedMember"/>).
///         </item>
///         <item>
///         A single optional <see cref="ICachedType"/>. This is always available, it is the Type that is decorated
///         by the <c>[ReaDI]</c> attribute or the type that defines the <c>[ReaDI]</c> method, property or event.
///         </item>
///         <item>
///         A single optional <see cref="ICachedMember"/> that is the <c>[ReaDI]</c> decorated method, propertie or event.
///         This de facto restricts the source <c>[ReaDI]</c> attribute to target only methods, properties or events.
///         </item>
///         <item>
///         Any service that should be available in the root <see cref="IServiceProvider"/>.
///         </item>
///     </list>
///     </para>
///     </item>
///     <item>
///     Can have any number of <c>[ReadImpl]</c> methods that return a <see cref="ReaDIResult"/> or a <see cref="ReaDIResult{T}"/>.
///     These methods can declare the same parameters as the <c>[ReadImpl]</c> constructor plus:
///     <list type="bullet">
///         <item>
///         A single optional <see cref="IReaDIHost"/> or a <see cref="ReaDITypeHost"/> or a <see cref="ReaDIMemberHost"/> when the
///         <c>[ReaDI]</c> attribute targets only methods, properties or events.
///         </item>
///         <item>
///         Any <see cref="ReaDIService"/> type.
///         </item>
///     </list>
///     </item>
/// </list>
/// When applied to a method of a <see cref="ReaDIService"/>, the method must return a <see cref="ReaDIResult"/>
/// or a <see cref="ReaDIResult{T}"/> and can declare the following parameters:
/// <list type="bullet">
///     <item>
///     Any service that should be available in the root <see cref="IServiceProvider"/>.
///     </item>
///     <item>
///     Any <see cref="ReaDIService"/> type.
///     </item>
/// </list>
/// </summary>
[AttributeUsage( ReaDITargets, AllowMultiple = false, Inherited = false )]
public sealed class ReaDImplAttribute : Attribute
{
    /// <summary>
    /// The [ReaDI] applicable targets.
    /// </summary>
    public const AttributeTargets ReaDITargets = AttributeTargets.Class
                                               | AttributeTargets.Method
                                               | AttributeTargets.Constructor;

    /// <summary>
    /// Gets or sets an optional Type that must be assignable from the [ReaDI] decorated type.
    /// <para>
    /// A typical example is that <c>[SqlTable]</c> should only decorate a <c>SqlTable</c> derived class:
    /// the <c>SqlTableImpl</c> can be a <c>[ReaDImpl( RequiredDecoratedType = typeof( SqlTable )]</c>: this kind of
    /// check is implemented once for all in the ReaDI engine.
    /// </para>
    /// </summary>
    public Type? RequiredDecoratedType { get; set; }
}

