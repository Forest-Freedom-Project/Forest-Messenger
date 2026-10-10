using DotI2p;

namespace Forest.Core.Models
{
    public sealed class I2PEndPoint : IEquatable<I2PEndPoint>
    {
        public DestinationKey? Destination { get; }
        public string? B32Hostname { get; }

        public I2PEndPoint(DestinationKey destination)
        {
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));

            B32Hostname = destination.GetB32Hostname();
        }

        public I2PEndPoint(string b32Hostname)
        {
            if (string.IsNullOrWhiteSpace(b32Hostname))
            { throw new ArgumentException("b32 hostname cannot be empty"); }

            B32Hostname = b32Hostname;
            Destination = null;
        }

        public bool Equals(I2PEndPoint? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.IsNullOrEmpty(B32Hostname) && !string.IsNullOrEmpty(other.B32Hostname))
            { return string.Equals(B32Hostname, other.B32Hostname, StringComparison.OrdinalIgnoreCase); }

            if (Destination != null && other.Destination != null)
            { return Destination.Equals(other.Destination); }

            return false;
        }

        public override bool Equals(object? obj) => Equals(obj as I2PEndPoint);

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(B32Hostname))
            { return B32Hostname!.ToLowerInvariant().GetHashCode(); }
            return Destination?.GetHashCode() ?? 0;            
        }

        public override string ToString() => B32Hostname ?? Destination?.GetB32Hostname() ?? "unknown.i2p";        
    }
}