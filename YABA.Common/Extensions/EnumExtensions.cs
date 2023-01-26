using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YABA.Common.Attributes;
using YABA.Common.Lookups;

namespace YABA.Common.Extensions
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetAttribute<DisplayAttribute>().Name;
        }

        public static string GetClaimName(this ClaimsLookup claimLookup)
        {
            return claimLookup.GetAttribute<ClaimNameAttribute>().Name;
        }

    }
}
