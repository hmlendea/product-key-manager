using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductKeyManager.Service.Models
{
    public sealed class ProductKeyStatus : IEquatable<ProductKeyStatus>
    {
        static Dictionary<string, ProductKeyStatus> entries = new Dictionary<string, ProductKeyStatus>
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

        ProductKeyStatus(string name)
        {
            Name = name;
        }

        public static ProductKeyStatus Unknown => new ProductKeyStatus(nameof(Unknown));
        public static ProductKeyStatus Used => new ProductKeyStatus(nameof(Used));
        public static ProductKeyStatus Vacant => new ProductKeyStatus(nameof(Vacant));
        public static ProductKeyStatus Invalid => new ProductKeyStatus(nameof(Invalid));
        public static ProductKeyStatus AlreadyOwned => new ProductKeyStatus(nameof(AlreadyOwned));
        public static ProductKeyStatus RequiresBaseProduct => new ProductKeyStatus(nameof(RequiresBaseProduct));
        public static ProductKeyStatus RegionLocked => new ProductKeyStatus(nameof(RegionLocked));

        public static IEnumerable<ProductKeyStatus> Values
            => entries.Values;

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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ProductKeyStatus)obj);
        }
        
        public static bool operator == (ProductKeyStatus first, ProductKeyStatus second)
        {
            if (first is null)
            {
                return (second is null);
            }

            return first.Equals(second);
        }

        public static bool operator != (ProductKeyStatus first, ProductKeyStatus second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name != null ? Name.GetHashCode() : 0) * 397;
            }
        }
    }
}