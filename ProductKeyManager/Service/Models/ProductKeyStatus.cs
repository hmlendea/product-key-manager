using System;
using System.Collections.Generic;
using System.Linq;

using NuciExtensions;

namespace ProductKeyManager.Service.Models
{
    public sealed class ProductKeyStatus : IEquatable<ProductKeyStatus>
    {
        private static readonly Dictionary<string, ProductKeyStatus> values = new()
        {
            { nameof(Unknown), new ProductKeyStatus(nameof(Unknown)) },
            { nameof(Used), new ProductKeyStatus(nameof(Used)) },
            { nameof(Vacant), new ProductKeyStatus(nameof(Vacant)) },
            { nameof(Invalid), new ProductKeyStatus(nameof(Invalid)) },
            { nameof(AlreadyOwned), new ProductKeyStatus(nameof(AlreadyOwned)) },
            { nameof(RequiresBaseProduct), new ProductKeyStatus(nameof(RequiresBaseProduct)) },
            { nameof(RegionLocked), new ProductKeyStatus(nameof(RegionLocked)) }
        };

        public string Name { get; }

        private ProductKeyStatus(string name) => Name = name;

        public static ProductKeyStatus Unknown => values[nameof(Unknown)];
        public static ProductKeyStatus Used => values[nameof(Used)];
        public static ProductKeyStatus Vacant => values[nameof(Vacant)];
        public static ProductKeyStatus Invalid => values[nameof(Invalid)];
        public static ProductKeyStatus AlreadyOwned => values[nameof(AlreadyOwned)];
        public static ProductKeyStatus RequiresBaseProduct => values[nameof(RequiresBaseProduct)];
        public static ProductKeyStatus RegionLocked => values[nameof(RegionLocked)];

        public static Array GetValues() => values.Values.ToArray();

        public static ProductKeyStatus FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !values.ContainsKey(name))
            {
                return Unknown;
            }

            return values[name];
        }

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

        public override int GetHashCode() => $"{nameof(ProductKeyStatus)}:{Name}".GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(ProductKeyStatus current, ProductKeyStatus other)
        {
            if (current is null)
            {
                return other is null;
            }

            return object.Equals(current, other);
        }

        public static bool operator !=(ProductKeyStatus current, ProductKeyStatus other)
            => !(current == other);

        public static implicit operator string(ProductKeyStatus status) => status.Name;
    }
}