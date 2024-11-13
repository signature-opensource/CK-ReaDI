using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CK.ReaDI.Engine;

public sealed partial class ReaDICache
{
    class CachedItem
    {
        readonly MemberInfo _member;
        readonly ImmutableArray<CustomAttributeData> _customAttributes;
        ImmutableArray<object> _attributes;

        public CachedItem( MemberInfo member, ImmutableArray<CustomAttributeData> customAttributes)
        {
            _member = member;
            _customAttributes = customAttributes;
        }

        protected MemberInfo MemberInfo => _member;

        public ImmutableArray<CustomAttributeData> CustomAttributes => _customAttributes;

        public ImmutableArray<object> Attributes
        {
            get
            {
                if( _attributes.IsDefault )
                {
                    _attributes = ImmutableCollectionsMarshal.AsImmutableArray( _member.GetCustomAttributes( false ) );
                }
                return _attributes;
            }
        }

    }


}
