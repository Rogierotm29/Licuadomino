namespace Licuadomino.Core
{

    public interface IEstadoPartida
    {
        string Nombre { get; }
        void Entrar();
        ResultadoConexion Procesar(IComandoJugada cmd);
    }

    public class EstadoRepartiendo : IEstadoPartida
    {
        readonly Partida p;
        public EstadoRepartiendo(Partida p) => this.p = p;
        public string Nombre => "Repartiendo";

        public void Entrar()
        {
            p.Repartir();
            p.CambiarEstado(new EstadoTurno(p));
        }

        public ResultadoConexion Procesar(IComandoJugada cmd) =>
            ResultadoConexion.Error("Todavía se están repartiendo las fichas.");
    }

    public class EstadoTurno : IEstadoPartida
    {
        readonly Partida p;
        public EstadoTurno(Partida p) => this.p = p;
        public string Nombre => "Turno";

        public void Entrar() => p.NotificarTurno();

        public ResultadoConexion Procesar(IComandoJugada cmd)
        {
            ResultadoConexion r = cmd.Validar(p);
            if (!r.Valida)
            {
                p.NotificarRechazo(cmd.JugadorId, r.Mensaje);
                return r;
            }

            cmd.Ejecutar(p);
            p.RegistrarEnHistorial(cmd);

            Jugador j = p.Jugadores[cmd.JugadorId];
            if (j.Mano.Count == 0)
            {
                p.CambiarEstado(new EstadoFinPartida(p, MotivoFin.SinFichas));
                return r;
            }
            if (p.PasesSeguidos >= p.Jugadores.Count)
            {
                p.CambiarEstado(new EstadoFinPartida(p, MotivoFin.Tranca));
                return r;
            }

            if (cmd.Tipo != TipoComando.Robar)
                p.AvanzarTurno();
            return r;
        }
    }

    public class EstadoFinPartida : IEstadoPartida
    {
        readonly Partida p;
        readonly MotivoFin motivo;

        public EstadoFinPartida(Partida p, MotivoFin motivo)
        {
            this.p = p;
            this.motivo = motivo;
        }

        public string Nombre => "FinPartida";

        public void Entrar()
        {
            p.Resultado = ResultadoPartida.Calcular(p, motivo);
            p.NotificarFin();
        }

        public ResultadoConexion Procesar(IComandoJugada cmd) =>
            ResultadoConexion.Error("La partida ya terminó.");
    }
}
