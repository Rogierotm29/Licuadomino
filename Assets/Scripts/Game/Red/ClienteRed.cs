using ExitGames.Client.Photon;
using Licuadomino.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ClienteRed : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] PantallaJuego pantalla;

    bool esperandoRespuesta;
    public event System.Action<EstadoVisible> EstadoRecibido;

    void Start()
    {
        pantalla.Accion += EnviarComando;
    }

    void OnDestroy()
    {
        if (pantalla != null) pantalla.Accion -= EnviarComando;
    }

    void EnviarComando(IComandoJugada cmd)
    {
        if (esperandoRespuesta) { Debug.Log("[Cliente] Clic ignorado: esperando respuesta del host"); return; }
        Debug.Log("[Cliente] Mandando " + string.Join(",", cmd.Serializar()));
        esperandoRespuesta = true;
        EnvioRed.AlHost(CodigosRed.Comando, cmd.Serializar());
    }

    public void OnEvent(EventData ev)
    {
        switch (ev.Code)
        {
            case CodigosRed.Estado: RecibirEstado(ev.CustomData); break;
            case CodigosRed.Mensaje: RecibirMensaje(ev.CustomData); break;
        }
    }

    void RecibirEstado(object datos)
    {
        esperandoRespuesta = false;
        try
        {
            EstadoVisible e = SerializadorEstado.Desempacar(datos as object[]);
            pantalla.Mostrar(e);
            EstadoRecibido?.Invoke(e);
        }
        catch (System.ArgumentException e)
        {
            Debug.LogError("[Cliente] Estado inválido: " + e.Message);
        }
    }

    void RecibirMensaje(object datos)
    {
        if (datos is object[] d && d.Length == 2 && d[0] is string texto && d[1] is bool esError)
        {
            if (esError) esperandoRespuesta = false;
            pantalla.MostrarMensaje(texto, esError);
        }
    }
}