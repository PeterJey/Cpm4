using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Services.Allocations
{
    public class Product : IEquatable<Product>
    {
        [Required]
        public string Type { get; set; }
        [Required]
        [Range(1,999999)]
        public decimal PerTray { get; set; }
        [Required]
        [Range(1,999999)]
        public decimal PerPunnet { get; set; }

        public string ToKey() => $"{Type}|{PerTray:F0}|{PerPunnet:F0}";
    
        public string ToDescription() => $"{Type} {PerTray:F0}x{PerPunnet:F0}";
    
        public bool Equals(Product other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Type, other.Type, StringComparison.InvariantCultureIgnoreCase) && PerTray == other.PerTray && PerPunnet == other.PerPunnet;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Type != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Type) : 0);
                hashCode = (hashCode * 397) ^ PerTray.GetHashCode();
                hashCode = (hashCode * 397) ^ PerPunnet.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Product left, Product right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Product left, Product right)
        {
            return !Equals(left, right);
        }
    }
}