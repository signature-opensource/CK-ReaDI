using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CK.ReaDI.Engine;

public sealed partial class ReaDICache
{
    class CachedItem
    {
        readonly ImmutableArray<CustomAttributeData> _customAttributes;
        ImmutableArray<object> _attributes;

        public CachedItem(ImmutableArray<CustomAttributeData> customAttributes)
        {
            _customAttributes = customAttributes;
        }

        public ImmutableArray<CustomAttributeData> CustomAttributes => _customAttributes;

        public ImmutableArray<object> Attributes
        {
            get
            {
                if( _attributes.IsDefault )
                {
                    _attributes = ImmutableCollectionsMarshal.AsImmutableArray( _type.GetCustomAttributes( false ) );
                }
                return _attributes;
            }
        }
    }


}
