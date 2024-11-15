using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CK.Core;

namespace CK.ReaDI.Engine;

public sealed partial class ReaDICache
{
    sealed class CachedType : CachedItem, ICachedType
    {
        readonly ICachedType? _baseType;
        string? _name;
        ImmutableArray<ICachedMember> _members;

        public CachedType( Type type,
                           ICachedType? baseType,
                           ImmutableArray<CustomAttributeData> customAttributes )
            : base( type, customAttributes )    
        {
            _baseType = baseType;
        }

        public Type Type => Unsafe.As<Type>( MemberInfo );

        public ICachedType? BaseType => _baseType;

        public ImmutableArray<ICachedMember> DeclaredMembers
        {
            get
        {
                if( _members.IsDefault )
                {
                    var members = Type.GetMembers( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly );
                    var b = ImmutableArray.CreateBuilder<ICachedMember>( members.Length );
                    foreach( var member in members )
                    {
                        if( (member is MethodInfo m && !m.IsSpecialName)
                            || member is PropertyInfo
                            || member is EventInfo )
                        {
                            var customAttributes = member.CustomAttributes.ToImmutableArray();
                            b.Add( new CachedMember( this, member, customAttributes ) );
                        }
                    }
                    _members = b.DrainToImmutable();
                }
                return _members;
            }
        }

        public string ReaDIName => _name ??= Type.ToCSharpName();
    }
}
