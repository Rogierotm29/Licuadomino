using System;
using System.Collections.Generic;

namespace Licuadomino.Core
{

    public class Partida
    {
        public const int FichasPorJugador = 7;
        public const int MinJugadores = 2;
        public const int MaxJugadores = 4;

        readonly List<Jugador> jugadores = new List<Jugador>();
        readonly List<Ficha> pozo = new List<Ficha>();
        readonly List<IComandoJugada> historial = new List<IComandoJugada>();

        public Tablero Tablero { get; }
        public IReadOnlyList<Jugador> Jugadores => jugadores;
        public IReadOnlyList<IComandoJugada> Historial => historial;
        public IEstadoPartida Estado { get; private set; }
        public int Semilla { get; private set; }
        public int JugadorInicial { get; private set; }
        public int Turno { get; private set; }
        public int PasesSeguidos { get; internal set; }
        public ResultadoPartida Resultado { get; internal set; }

        public Jugador JugadorEnTurno => jugadores[Turno];
        public int FichasEnPozo => pozo.Count;
        public bool Terminada => Estado is EstadoFinPartida;

        public event Action<IEstadoPartida> EstadoCambiado;
        public event Action<Jugador> TurnoCambiado;
        public event Action<Jugador, Jugada, string> FichaJugada;  
        public event Action<Jugador, Ficha> FichaRobada;
        public event Action<Jugador> TurnoPasado;
        public event Action<int, string> JugadaRechazada;          
        public event Action<ResultadoPartida> PartidaTerminada;

        public Partida(IReadOnlyList<string> nombres, IReglaCompatibilidad regla, int semilla, int jugadorInicial = 0)
        {
            if (nombres == null || nombres.Count < MinJugadores || nombres.Count > MaxJugadores)
                throw new ArgumentException($"Se necesitan de {MinJugadores} a {MaxJugadores} jugadores.");
            for (int i = 0; i < nombres.Count; i++)
                jugadores.Add(new Jugador(i, nombres[i]));

            Tablero = new Tablero(regla);
            Semilla = semilla;
            JugadorInicial = jugadorInicial % nombres.Count;
        }



        public void Iniciar() => CambiarEstado(new EstadoRepartiendo(this));

        public ResultadoConexion Enviar(IComandoJugada cmd)
        {
            if (Estado == null) return ResultadoConexion.Error("La partida no ha iniciado.");
            return Estado.Procesar(cmd);
        }

        public void Reiniciar(int nuevaSemilla)
        {
            Semilla = nuevaSemilla;
            JugadorInicial = (JugadorInicial + 1) % jugadores.Count;
            Iniciar();
        }

        public int FichasDe(int jugadorId) => jugadores[jugadorId].Mano.Count;


        internal void CambiarEstado(IEstadoPartida nuevo)
        {
            Estado = nuevo;
            EstadoCambiado?.Invoke(nuevo);
            nuevo.Entrar();
        }

        internal void Repartir()
        {
            Tablero.Limpiar();
            pozo.Clear();
            historial.Clear();
            Resultado = null;
            PasesSeguidos = 0;
            foreach (var j in jugadores) j.VaciarMano();

            List<Ficha> mazo = GeneradorFichas.Barajado(new Random(Semilla));
            int k = 0;
            for (int ronda = 0; ronda < FichasPorJugador; ronda++)
                foreach (var j in jugadores)
                    j.Agregar(mazo[k++]);
            for (; k < mazo.Count; k++) pozo.Add(mazo[k]);

            Turno = JugadorInicial;
        }

        internal Ficha SacarDelPozo()
        {
            Ficha f = pozo[pozo.Count - 1];
            pozo.RemoveAt(pozo.Count - 1);
            return f;
        }

        internal void AvanzarTurno()
        {
            Turno = (Turno + 1) % jugadores.Count;
            NotificarTurno();
        }

        internal void RegistrarEnHistorial(IComandoJugada cmd) => historial.Add(cmd);

        internal void NotificarTurno() => TurnoCambiado?.Invoke(JugadorEnTurno);
        internal void NotificarFichaJugada(Jugador j, Jugada jugada, string msg) => FichaJugada?.Invoke(j, jugada, msg);
        internal void NotificarFichaRobada(Jugador j, Ficha f) => FichaRobada?.Invoke(j, f);
        internal void NotificarTurnoPasado(Jugador j) => TurnoPasado?.Invoke(j);
        internal void NotificarRechazo(int jugadorId, string msg) => JugadaRechazada?.Invoke(jugadorId, msg);
        internal void NotificarFin() => PartidaTerminada?.Invoke(Resultado);
    }
}
