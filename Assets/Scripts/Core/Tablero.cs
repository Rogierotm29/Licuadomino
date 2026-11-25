using System;
using System.Collections.Generic;

namespace Licuadomino.Core
{
    public enum Lado { Izquierda, Derecha }

    public readonly struct FichaColocada
    {
        public readonly Ficha Ficha;
        public readonly Fruta Izq;
        public readonly Fruta Der;

        public FichaColocada(Ficha ficha, Fruta izq, Fruta der)
        {
            Ficha = ficha;
            Izq = izq;
            Der = der;
        }

        public override string ToString() => $"[{FrutaInfo.Nombre(Izq)}|{FrutaInfo.Nombre(Der)}]";
    }

    public readonly struct Jugada
    {
        public readonly Ficha Ficha;
        public readonly Lado Lado;
        public readonly Fruta Toca;

        public Jugada(Ficha ficha, Lado lado, Fruta toca)
        {
            Ficha = ficha;
            Lado = lado;
            Toca = toca;
        }

        public override string ToString() => $"{Ficha} en {Lado} tocando con {FrutaInfo.Nombre(Toca)}";
    }

    public class Tablero
    {
        readonly List<FichaColocada> cadena = new List<FichaColocada>();

        public IReglaCompatibilidad Regla { get; }
        public IReadOnlyList<FichaColocada> Cadena => cadena;
        public bool Vacio => cadena.Count == 0;

        public Tablero(IReglaCompatibilidad regla)
        {
            Regla = regla ?? throw new ArgumentNullException(nameof(regla));
        }

        public Fruta Extremo(Lado lado)
        {
            if (Vacio) throw new InvalidOperationException("El tablero está vacío.");
            return lado == Lado.Izquierda ? cadena[0].Izq : cadena[cadena.Count - 1].Der;
        }

        public ResultadoConexion Validar(Jugada j)
        {
            if (!j.Ficha.Tiene(j.Toca))
                return ResultadoConexion.Error("Esa fruta no está en la ficha.");
            if (Vacio)
                return ResultadoConexion.Ok("¡Arranca el licuado!");
            return Regla.Evaluar(Extremo(j.Lado), j.Toca);
        }

        public ResultadoConexion Colocar(Jugada j)
        {
            ResultadoConexion r = Validar(j);
            if (!r.Valida) return r;

            Ficha f = j.Ficha;
            Fruta libre = f.Otra(j.Toca);

            if (Vacio)
                cadena.Add(new FichaColocada(f, f.A, f.B));
            else if (j.Lado == Lado.Derecha)
                cadena.Add(new FichaColocada(f, j.Toca, libre));
            else
                cadena.Insert(0, new FichaColocada(f, libre, j.Toca));

            return r;
        }


        public List<Jugada> JugadasPosibles(Ficha f)
        {
            var res = new List<Jugada>();
            if (Vacio)
            {
                res.Add(new Jugada(f, Lado.Derecha, f.A));
                return res;
            }
            foreach (Lado lado in new[] { Lado.Izquierda, Lado.Derecha })
            {
                TryAgregar(res, new Jugada(f, lado, f.A));
                if (!f.EsDoble) TryAgregar(res, new Jugada(f, lado, f.B));
            }
            return res;
        }


        public List<Jugada> JugadasPosibles(IEnumerable<Ficha> mano)
        {
            var res = new List<Jugada>();
            foreach (var f in mano) res.AddRange(JugadasPosibles(f));
            return res;
        }

        public bool PuedeJugar(IEnumerable<Ficha> mano)
        {
            foreach (var f in mano)
                if (JugadasPosibles(f).Count > 0) return true;
            return false;
        }

        public void Limpiar() => cadena.Clear();

        void TryAgregar(List<Jugada> lista, Jugada j)
        {
            if (Validar(j).Valida) lista.Add(j);
        }
    }
}
