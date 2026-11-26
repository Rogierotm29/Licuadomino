using Licuadomino.Core;
using UnityEngine;
using UnityEngine.UI;


public class JuegoLocal : MonoBehaviour
{
    [SerializeField] PantallaJuego pantalla;
    [SerializeField] Button botonNueva;
    [SerializeField] string[] nombres = { "Jugador 1", "Jugador 2" };

    Partida partida;

    void Start()
    {
        pantalla.Accion += Enviar;
        if (botonNueva != null) botonNueva.onClick.AddListener(Nueva);
        Nueva();
    }

    void Nueva()
    {
        partida = new Partida(nombres, new ReglaLicuados(), Random.Range(0, int.MaxValue));
        partida.FichaJugada += (j, jugada, msg) => pantalla.MostrarMensaje($"{j.Nombre}: {msg}", false);
        partida.FichaRobada += (j, f) => pantalla.MostrarMensaje($"{j.Nombre} robó una ficha del pozo.", false);
        partida.TurnoPasado += j => pantalla.MostrarMensaje($"{j.Nombre} pasó: no tenía con qué jugar.", false);
        partida.PartidaTerminada += r => pantalla.MostrarMensaje(r.ToString(), false);

        partida.Iniciar();
        pantalla.MostrarMensaje("Elige una ficha de tu mano y luego de qué lado de la mesa ponerla.", false);
        Refrescar();
    }

    void Enviar(IComandoJugada cmd)
    {
        ResultadoConexion r = partida.Enviar(cmd);
        if (!r.Valida) pantalla.MostrarMensaje(r.Mensaje, true);
        Refrescar();
    }

    void Refrescar()
    {
        int quien = partida.Turno;
        if (partida.Terminada && !partida.Resultado.EsEmpate) quien = partida.Resultado.GanadorId;
        pantalla.Mostrar(partida.VistaPara(quien));
    }
}
