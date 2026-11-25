using System.Collections.Generic;
using System.Linq;

namespace Licuadomino.Core
{
    public enum MotivoFin { SinFichas, Tranca }

    public readonly struct Posicion
    {
        public readonly int JugadorId;
        public readonly string Nombre;
        public readonly int FichasRestantes;

        public Posicion(int jugadorId, string nombre, int fichasRestantes)
        {
            JugadorId = jugadorId;
            Nombre = nombre;
            FichasRestantes = fichasRestantes;
        }
    }

    public class ResultadoPartida
    {
        public MotivoFin Motivo { get; private set; }
        public int GanadorId { get; private set; } = -1; 
        public int Puntos { get; private set; }
        public IReadOnlyList<Posicion> Tabla { get; private set; }
        public bool EsEmpate => GanadorId < 0;

        public static ResultadoPartida Calcular(Partida p, MotivoFin motivo)
        {
            var tabla = p.Jugadores
                .Select(j => new Posicion(j.Id, j.Nombre, j.Mano.Count))
                .OrderBy(x => x.FichasRestantes)
                .ToList();

            int minimo = tabla[0].FichasRestantes;
            bool empate = tabla.Count(x => x.FichasRestantes == minimo) > 1;
            int ganador = empate ? -1 : tabla[0].JugadorId;
            int puntos = empate ? 0 : tabla.Where(x => x.JugadorId != ganador).Sum(x => x.FichasRestantes);

            return new ResultadoPartida
            {
                Motivo = motivo,
                GanadorId = ganador,
                Puntos = puntos,
                Tabla = tabla
            };
        }

        public override string ToString()
        {
            string cab = Motivo == MotivoFin.Tranca ? "Juego trancado. " : "";
            if (EsEmpate) return cab + "Empate.";
            return cab + $"Gana {Tabla[0].Nombre} con {Puntos} punto{(Puntos == 1 ? "" : "s")}.";
        }
    }
}
