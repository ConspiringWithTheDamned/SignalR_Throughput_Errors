using System;

namespace Domain
{
    public class Data : IEquatable<Data>
    {
        public string String { get; set; }
        public int Number { get; set; }

        public bool Equals(Data other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(String, other.String);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Data) obj);
        }

        public override int GetHashCode()
        {
            return (String != null ? String.GetHashCode() : 0);
        }
    }
}
