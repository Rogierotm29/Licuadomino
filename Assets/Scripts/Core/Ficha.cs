using System;

namespace Licuadomino.Core
{

    public readonly struct Ficha : IEquatable<Ficha>
    {
        public readonly int Id;
        public readonly Fruta A;
        public readonly Fruta B;

        public Ficha(int id, Fruta a, Fruta b)
        {
            Id = id;
            A = a;
            B = b;
        }

        public bool EsDoble => A == B;
        public bool Tiene(Fruta f) => A == f || B == f;

        public Fruta Otra(Fruta f) => f == A ? B : A;

        public bool Equals(Ficha other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Ficha f && Equals(f);
        public override int GetHashCode() => Id;
        public static bool operator ==(Ficha x, Ficha y) => x.Id == y.Id;
        public static bool operator !=(Ficha x, Ficha y) => x.Id != y.Id;

        public override string ToString() => $"[{FrutaInfo.Nombre(A)}|{FrutaInfo.Nombre(B)}]";
    }
}
