using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YABA.Common.Attributes;
using YABA.Common.Lookups;

namespace YABA.Common.Extensions
{
    public static class EnumExtensions
    {
        private static readonly IEnumerable<CrudResultLookup> SuccessfulCrudStatuses = new List<CrudResultLookup>() {
            CrudResultLookup.CreateSucceeded,
            CrudResultLookup.UpdateSucceeded,
            CrudResultLookup.DeleteSucceeded,
            CrudResultLookup.RetrieveSuccessful
        };

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

        public static bool IsCrudResultSuccessful(this CrudResultLookup importStatusLookup) => SuccessfulCrudStatuses.Contains(importStatusLookup);

        public static bool IsCrudResultFailure(this CrudResultLookup importStatusLookup) => !SuccessfulCrudStatuses.Contains(importStatusLookup);
    }
}
