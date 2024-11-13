using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace CK.ReaDI.Engine;

public sealed partial class ReaDICache
{


    sealed class CachedMember : CachedItem, ICachedMember
    {
        readonly ICachedType _type;
        string? _name;

        public CachedMember( ICachedType type, MemberInfo memberInfo, ImmutableArray<CustomAttributeData> customAttributes )
            : base( memberInfo, customAttributes )
        {
            _type = type;
        }

        public new MemberInfo MemberInfo => base.MemberInfo;

        public ICachedType DeclaringType => _type;

        public string ReaDIName => _name ??= $"{_type.ReaDIName}.{MemberInfo.Name}";;
    }


}
