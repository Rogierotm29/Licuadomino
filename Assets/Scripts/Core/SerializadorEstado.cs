
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Licuadomino.Core
{
    public static class SerializadorEstado
    {
        public const int Campos = 10;

        public static object[] Empacar(EstadoVisible e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var mano = new int[e.MiMano.Count];

            for (int i = 0; i < mano.Length; i++)
                mano[i] = e.MiMano[i].Id;

            var cadena = new int[e.Cadena.Count * 2];

            for (int i = 0; i < e.Cadena.Count; i++)
            {
                cadena[2 * i] = e.Cadena[i].Ficha.Id;
                cadena[2 * i + 1] = (int)e.Cadena[i].Izq;
            }

            return new object[]
            {
                e.MiId,
                (string[])e.Nombres.Clone(),
                (int[])e.FichasPorJugador.Clone(),
                mano,
                cadena,
                e.IdFichaInicial,
                e.FichasEnPozo,
                e.Turno,
                e.Terminada,
                EmpacarResultado(e.Resultado)
            };
        }

        public static EstadoVisible Desempacar(object[] d)
        {
            if (d == null || d.Length < Campos)
                throw new ArgumentException("Estado incompleto.");

            var e = new EstadoVisible
            {
                MiId = Leer<int>(d, 0),
                Nombres = Leer<string[]>(d, 1),
                FichasPorJugador = Leer<int[]>(d, 2),
                IdFichaInicial = Leer<int>(d, 5),
                FichasEnPozo = Leer<int>(d, 6),
                Turno = Leer<int>(d, 7),
                Terminada = Leer<bool>(d, 8)
            };

            int n = e.Nombres.Length;

            if (n < Partida.MinJugadores ||
                n > Partida.MaxJugadores ||
                e.FichasPorJugador.Length != n)
                throw new ArgumentException("Cantidad de jugadores inválida.");

            if (e.MiId < 0 || e.MiId >= n ||
                e.Turno < 0 || e.Turno >= n)
                throw new ArgumentException("Jugador o turno fuera de rango.");

            foreach (int id in Leer<int[]>(d, 3))
                e.MiMano.Add(FichaValida(id));

            int[] c = Leer<int[]>(d, 4);

            if (c.Length % 2 != 0)
                throw new ArgumentException("Cadena mal formada.");

            for (int i = 0; i < c.Length; i += 2)
            {
                Ficha f = FichaValida(c[i]);
                var izq = (Fruta)c[i + 1];

                if (!f.Tiene(izq))
                    throw new ArgumentException(
                        $"La ficha {f} no tiene {izq}.");

                e.Cadena.Add(
                    new FichaColocada(f, izq, f.Otra(izq)));
            }

            e.Resultado = DesempacarResultado(d[9], e.Nombres);

            return e;
        }

        static int[] EmpacarResultado(ResultadoPartida r)
        {
            if (r == null) return null;
            var datos = new int[3 + r.Tabla.Count * 2];
            datos[0] = (int)r.Motivo;
            datos[1] = r.GanadorId;
            datos[2] = r.Puntos;
            for (int k = 0; k < r.Tabla.Count; k++)
            {
                datos[3 + 2 * k] = r.Tabla[k].JugadorId;
                datos[4 + 2 * k] = r.Tabla[k].FichasRestantes;
            }
            return datos;
        }

        static ResultadoPartida DesempacarResultado(object o, string[] nombres)
        {
            if (o == null) return null;
            if (!(o is int[] datos) || datos.Length < 3 || (datos.Length - 3) % 2 != 0)
                throw new ArgumentException("Resultado mal formado.");

            var tabla = new List<Posicion>();
            for (int i = 3; i < datos.Length; i += 2)
            {
                int id = datos[i];
                if (id < 0 || id >= nombres.Length) throw new ArgumentException("Jugador del resultado fuera de rango.");
                tabla.Add(new Posicion(id, nombres[id], datos[i + 1]));
            }
            return ResultadoPartida.Desde((MotivoFin)datos[0], datos[1], datos[2], tabla);
        }
    

    static T Leer<T>(object[] d, int i)
        {
            if (d[i] is T valor) return valor;
            throw new ArgumentException($"Campo {i}: se esperaba {typeof(T).Name}.");
        }

        static Ficha FichaValida(int id)
        {
            if (id < 0 || id >= GeneradorFichas.Total) throw new ArgumentException("Ficha inválida: " + id);
            return GeneradorFichas.PorId(id);
        }
    }
}