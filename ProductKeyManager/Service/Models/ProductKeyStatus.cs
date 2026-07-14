using System;
using System.Collections.Generic;
using System.Linq;
using NuciExtensions;

namespace ProductKeyManager.Service.Models
{
    public sealed class ProductKeyStatus : IEquatable<ProductKeyStatus>
    {
        static readonly Dictionary<string, ProductKeyStatus> entries = new()
        {
            { Unknown.Name, Unknown },
            { Used.Name, Used },
            { Vacant.Name, Vacant },
            { Invalid.Name, Invalid },
            { AlreadyOwned.Name, AlreadyOwned },
            { RequiresBaseProduct.Name, RequiresBaseProduct },
            { RegionLocked.Name, RegionLocked }
        };

        public string Name { get; }

        ProductKeyStatus(string name) => Name = name;

        public static ProductKeyStatus Unknown => new(nameof(Unknown));
        public static ProductKeyStatus Used => new(nameof(Used));
        public static ProductKeyStatus Vacant => new(nameof(Vacant));
        public static ProductKeyStatus Invalid => new(nameof(Invalid));
        public static ProductKeyStatus AlreadyOwned => new(nameof(AlreadyOwned));
        public static ProductKeyStatus RequiresBaseProduct => new(nameof(RequiresBaseProduct));
        public static ProductKeyStatus RegionLocked => new(nameof(RegionLocked));

        public static IEnumerable<ProductKeyStatus> Values => entries.Values;

        public static ProductKeyStatus FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return entries.First().Value;
            }

            return entries[name];
        }

        public override string ToString() => Name;

        public bool Equals(ProductKeyStatus other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType().NotEquals(GetType()))
            {
                return false;
            }

            return Equals((ProductKeyStatus)obj);
        }

        public static bool operator == (ProductKeyStatus first, ProductKeyStatus second)
        {
            if (first is null)
            {
                return second is null;
            }

            return first.Equals(second);
        }

        public static bool operator != (ProductKeyStatus first, ProductKeyStatus second)
            => !(first == second);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name is not null ? Name.GetHashCode() : 0) * 397;
            }
        }
    }
}