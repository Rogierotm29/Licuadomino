using System.Collections.Generic;

namespace Licuadomino.Core
{

    public class EstadoVisible
    {
        public int MiId;
        public string[] Nombres;
        public int[] FichasPorJugador;
        public List<Ficha> MiMano = new List<Ficha>();
        public List<FichaColocada> Cadena = new List<FichaColocada>();
        public int IdFichaInicial = -1;
        public int FichasEnPozo;
        public int Turno;
        public bool Terminada;
        public ResultadoPartida Resultado;

        public bool EsMiTurno => !Terminada && Turno == MiId;
        public int CantidadJugadores => Nombres.Length;
        public bool MesaVacia => Cadena.Count == 0;
        public Fruta ExtremoIzquierdo => Cadena[0].Izq;
        public Fruta ExtremoDerecho => Cadena[Cadena.Count - 1].Der;
    }
}
