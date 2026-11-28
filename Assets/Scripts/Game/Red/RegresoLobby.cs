using Licuadomino.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegresoLobby : MonoBehaviourPunCallbacks
{
    const string EscenaLobby = "Lobby";

    [SerializeField] ClienteRed cliente;
    [SerializeField] PantallaJuego pantalla;
    [SerializeField] float segundosAntesDeSalir = 5f;

    bool hayPartida;
    bool saliendo;
    bool cargandoLobby;

    bool partidaTerminada;

    public override void OnEnable()
    {
        base.OnEnable();
        cliente.EstadoRecibido += AlRecibirEstado;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        cliente.EstadoRecibido -= AlRecibirEstado;
    }

    void AlRecibirEstado(EstadoVisible e)
    {
        hayPartida = true;
        partidaTerminada = e.Terminada;
        if (e.Terminada && e.Resultado != null && e.Resultado.Motivo == MotivoFin.Abandono)
            ProgramarSalida(e.Resultado.ToString(), false);
    }

    public override void OnMasterClientSwitched(Player nuevoHost) =>
        ProgramarSalida("El host salió de la partida.", true);

    public override void OnPlayerLeftRoom(Player otro)
    {
        if (!hayPartida)
            ProgramarSalida($"{otro.NickName} salió antes de empezar.", true);
        else if (partidaTerminada)
            ProgramarSalida($"{otro.NickName} salió de la sala.", true);
    }
    void ProgramarSalida(string motivo, bool esError)
    {
        if (saliendo) return;
        saliendo = true;
        pantalla.MostrarMensaje($"{motivo} Regresando al lobby en {segundosAntesDeSalir:0} s...", esError);
        Invoke(nameof(Salir), segundosAntesDeSalir);
    }

    public void SalirAhora()
    {
        saliendo = true;
        CancelInvoke(nameof(Salir));
        Salir();
    }

    void Salir()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        else IrAlLobby();
    }

    public override void OnLeftRoom() => IrAlLobby();

    public override void OnDisconnected(DisconnectCause causa) => IrAlLobby();

    void IrAlLobby()
    {
        if (cargandoLobby) return;
        cargandoLobby = true;
        SceneManager.LoadScene(EscenaLobby);
    }
}