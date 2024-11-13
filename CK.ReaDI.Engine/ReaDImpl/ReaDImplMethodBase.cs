using CK.Core;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

/// <summary>
/// Order matters here. The greater, the more constrained is the ReaDIAction and the method's <see cref="ReaDIResult{T}"/> type if any:
/// a type bound parameter (starting at <see cref="ReaDITypeService"/>) implies a ReaDIResult of <see cref="ReaDITypeBoundService"/>
/// and a member bound parameter (starting at <see cref="ReaDIMemberService"/>) ReaDIResult of <see cref="ReaDIMemberBoundService"/>.
/// </summary>
enum ParameterSourceBinding
{
    /// <summary>
    /// External service that must be provided by <see cref="IServiceProvider"/>.
    /// </summary>
    External,

    /// <summary>
    /// A <see cref="ReaDIService"/> object. 
    /// </summary>
    Service,

    /// <summary>
    /// A <see cref="ReaDIScope"/> object. 
    /// </summary>
    Scope,

    /// <summary>
    /// A <see cref="ReaDITypeBoundService"/> (bound to the <see cref="ReaDITypeHost"/>).
    /// </summary>
    TypeBoundService,

    /// <summary>
    /// A type related object. Can be the <see cref="ReaDITypeHost"/> or the <see cref="ICachedType"/>.
    /// </summary>
    TypeHost,

    /// <summary>
    /// A <see cref="ReaDIMemberBoundService"/> (bound to the <see cref="ReaDIMemberHost"/>).
    /// </summary>
    MemberBoundService,

    /// <summary>
    /// A member related object. Can be the <see cref="ReaDIMemberHost"/> or the <see cref="ICachedMember"/> (or any of its specializations).
    /// </summary>
    MemberHost,
}

abstract class ReaDImplMethodBase
{
    readonly MethodBase _methodBase;
    readonly ReaDIMethodParameters _parameters;

    protected ReaDImplMethodBase( MethodBase method,
                                  ReaDIMethodParameters parameters )
    {
        _methodBase = method;
        _parameters = parameters;
    }

    public MethodBase MethodBase => _methodBase;

    public ReaDIMethodParameters Parameters => _parameters;
}
