using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Licuadomino.Core
{
    public class RegistroPartida
    {
        public double Segundos;
        public int Invalidas;
        public int Robos;
        public int Pases;
        public bool Tranca;
        public string Ganador = "";
    }

    public class Telemetria
    {
        public const string Encabezado = "partida,duracion_s,jugadas_invalidas,robos,pases,tranca,ganador";

        readonly Func<DateTime> reloj;
        readonly List<RegistroPartida> partidas = new List<RegistroPartida>();
        RegistroPartida actual;
        DateTime inicio;

        public IReadOnlyList<RegistroPartida> Partidas => partidas;

        public Telemetria(Func<DateTime> reloj = null)
        {
            this.reloj = reloj ?? (() => DateTime.Now);
        }

        public void Observar(Partida p)
        {
            p.EstadoCambiado += estado => { if (estado is EstadoRepartiendo) Empezar(); };
            p.JugadaRechazada += (id, msg) => { if (actual != null) actual.Invalidas++; };
            p.FichaRobada += (j, f) => { if (actual != null) actual.Robos++; };
            p.TurnoPasado += j => { if (actual != null) actual.Pases++; };
            p.PartidaTerminada += Terminar;
        }

        void Empezar()
        {
            actual = new RegistroPartida();
            inicio = reloj();
        }

        void Terminar(ResultadoPartida r)
        {
            if (actual == null) return;
            actual.Segundos = (reloj() - inicio).TotalSeconds;
            actual.Tranca = r.Motivo == MotivoFin.Tranca;
            actual.Ganador = r.EsEmpate ? "Empate" : r.Tabla.First(x => x.JugadorId == r.GanadorId).Nombre;
            partidas.Add(actual);
            actual = null;
        }


        public string ComoCsv()
        {
            var sb = new StringBuilder(Encabezado);
            for (int i = 0; i < partidas.Count; i++)
            {
                RegistroPartida p = partidas[i];
                sb.Append('\n').Append(string.Join(",",
                    (i + 1).ToString(CultureInfo.InvariantCulture),
                    Math.Round(p.Segundos).ToString(CultureInfo.InvariantCulture),
                    p.Invalidas.ToString(CultureInfo.InvariantCulture),
                    p.Robos.ToString(CultureInfo.InvariantCulture),
                    p.Pases.ToString(CultureInfo.InvariantCulture),
                    p.Tranca ? "si" : "no",
                    p.Ganador.Replace(",", " ")));
            }
            return sb.ToString();
        }
    }
}