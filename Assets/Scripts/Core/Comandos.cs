using System;

namespace Licuadomino.Core
{
    public enum TipoComando { Colocar = 0, Robar = 1, Pasar = 2, Abandonar = 3 }



    public interface IComandoJugada
    {
        TipoComando Tipo { get; }
        int JugadorId { get; }

        ResultadoConexion Validar(Partida p);

        void Ejecutar(Partida p);

        int[] Serializar();
        string Describir(Partida p);
    }

    public abstract class ComandoBase : IComandoJugada
    {
        public abstract TipoComando Tipo { get; }
        public int JugadorId { get; }

        protected ComandoBase(int jugadorId) => JugadorId = jugadorId;

        public ResultadoConexion Validar(Partida p)
        {
            if (JugadorId < 0 || JugadorId >= p.Jugadores.Count)
                return ResultadoConexion.Error("Jugador inválido.");
            if (JugadorId != p.Turno)
                return ResultadoConexion.Error($"No es tu turno: le toca a {p.JugadorEnTurno.Nombre}.");
            return ValidarPropio(p, p.Jugadores[JugadorId]);
        }

        protected abstract ResultadoConexion ValidarPropio(Partida p, Jugador j);
        public abstract void Ejecutar(Partida p);
        public abstract int[] Serializar();
        public abstract string Describir(Partida p);

        protected bool TieneJugadas(Partida p, Jugador j) => p.Tablero.PuedeJugar(j.Mano);
    }

    public class ComandoColocar : ComandoBase
    {
        public override TipoComando Tipo => TipoComando.Colocar;
        public int FichaId { get; }
        public Lado Lado { get; }
        public Fruta Toca { get; }

        public ComandoColocar(int jugadorId, int fichaId, Lado lado, Fruta toca) : base(jugadorId)
        {
            FichaId = fichaId;
            Lado = lado;
            Toca = toca;
        }

        public ComandoColocar(int jugadorId, Jugada j) : this(jugadorId, j.Ficha.Id, j.Lado, j.Toca) { }

        Jugada ComoJugada() => new Jugada(GeneradorFichas.PorId(FichaId), Lado, Toca);

        protected override ResultadoConexion ValidarPropio(Partida p, Jugador j)
        {
            if (FichaId < 0 || FichaId >= GeneradorFichas.Total)
                return ResultadoConexion.Error("Ficha inválida.");
            if (!j.Tiene(FichaId))
                return ResultadoConexion.Error("Esa ficha no está en tu mano.");
            return p.Tablero.Validar(ComoJugada());
        }

        public override void Ejecutar(Partida p)
        {
            Jugador j = p.Jugadores[JugadorId];
            ResultadoConexion r = p.Tablero.Colocar(ComoJugada());
            j.Quitar(FichaId);
            p.PasesSeguidos = 0;
            p.NotificarFichaJugada(j, ComoJugada(), r.Mensaje);
        }

        public override int[] Serializar() =>
            new[] { (int)Tipo, JugadorId, FichaId, (int)Lado, (int)Toca };

        public override string Describir(Partida p) =>
            $"{p.Jugadores[JugadorId].Nombre} pone {GeneradorFichas.PorId(FichaId)} a la {Lado} tocando con {FrutaInfo.Nombre(Toca)}";
    }

    public class ComandoRobar : ComandoBase
    {
        public override TipoComando Tipo => TipoComando.Robar;
        public ComandoRobar(int jugadorId) : base(jugadorId) { }

        protected override ResultadoConexion ValidarPropio(Partida p, Jugador j)
        {
            if (TieneJugadas(p, j))
                return ResultadoConexion.Error("Tienes fichas que sí combinan: no puedes robar.");
            if (p.FichasEnPozo == 0)
                return ResultadoConexion.Error("Ya no hay fichas en el pozo: tienes que pasar.");
            return ResultadoConexion.Ok("Robaste una ficha.");
        }

        public override void Ejecutar(Partida p)
        {
            Jugador j = p.Jugadores[JugadorId];
            Ficha f = p.SacarDelPozo();
            j.Agregar(f);
            p.NotificarFichaRobada(j, f);
        }

        public override int[] Serializar() => new[] { (int)Tipo, JugadorId };
        public override string Describir(Partida p) => $"{p.Jugadores[JugadorId].Nombre} roba del pozo";
    }

    public class ComandoPasar : ComandoBase
    {
        public override TipoComando Tipo => TipoComando.Pasar;
        public ComandoPasar(int jugadorId) : base(jugadorId) { }

        protected override ResultadoConexion ValidarPropio(Partida p, Jugador j)
        {
            if (TieneJugadas(p, j))
                return ResultadoConexion.Error("Tienes fichas que sí combinan: no puedes pasar.");
            if (p.FichasEnPozo > 0)
                return ResultadoConexion.Error("Todavía hay fichas en el pozo: primero roba.");
            return ResultadoConexion.Ok("Pasas el turno.");
        }

        public override void Ejecutar(Partida p)
        {
            Jugador j = p.Jugadores[JugadorId];
            p.PasesSeguidos++;
            p.NotificarTurnoPasado(j);
        }

        public override int[] Serializar() => new[] { (int)Tipo, JugadorId };
        public override string Describir(Partida p) => $"{p.Jugadores[JugadorId].Nombre} pasa";
    }



    public class ComandoAbandonar : IComandoJugada
    {
        public TipoComando Tipo => TipoComando.Abandonar;
        public int JugadorId { get; }

        public ComandoAbandonar(int jugadorId) => JugadorId = jugadorId;

        public ResultadoConexion Validar(Partida p)
        {
            if (JugadorId < 0 || JugadorId >= p.Jugadores.Count)
                return ResultadoConexion.Error("Jugador inválido.");
            return ResultadoConexion.Ok($"{p.Jugadores[JugadorId].Nombre} abandonó la partida.");
        }

        public void Ejecutar(Partida p) => p.Abandono = JugadorId;

        public int[] Serializar() => new[] { (int)Tipo, JugadorId };
        public string Describir(Partida p) => $"{p.Jugadores[JugadorId].Nombre} abandona";
    }

    public static class ComandoSerializer
    {
        public static IComandoJugada Deserializar(int[] d)
        {
            if (d == null || d.Length < 2) throw new ArgumentException("Comando vacío.");
            switch ((TipoComando)d[0])
            {
                case TipoComando.Colocar:
                    if (d.Length < 5) throw new ArgumentException("Comando Colocar incompleto.");
                    return new ComandoColocar(d[1], d[2], (Lado)d[3], (Fruta)d[4]);
                case TipoComando.Robar: return new ComandoRobar(d[1]);
                case TipoComando.Pasar: return new ComandoPasar(d[1]);
                case TipoComando.Abandonar: return new ComandoAbandonar(d[1]);
                default: throw new ArgumentException("Tipo de comando desconocido: " + d[0]);
            }
        }
    }
}
