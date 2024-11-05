using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

public sealed partial class ReaDICache
{


    sealed class CachedMember : CachedItem, ICachedMember
    {
        readonly ICachedType _type;
        readonly MemberInfo _memberInfo;
        readonly string _name;

        public CachedMember( ICachedType type, MemberInfo memberInfo, ImmutableArray<CustomAttributeData> customAttributes )
            : base( customAttributes )
        {
            _type = type;
            _memberInfo = memberInfo;
            _name = $"{type.ReaDIName}.{memberInfo.Name}";
        }

        public MemberInfo MemberInfo => _memberInfo;

        public ICachedType DeclaringType => _type;

        public string ReaDIName => _name;
    }


}
