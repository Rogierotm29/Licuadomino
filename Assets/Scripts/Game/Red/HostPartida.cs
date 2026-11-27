using System.Collections.Generic;
using ExitGames.Client.Photon;
using Licuadomino.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HostPartida : MonoBehaviourPunCallbacks, IOnEventCallback
{
    readonly HashSet<int> listos = new HashSet<int>();
    Partida partida;
    int[] actores;

    public event System.Action<Partida> PartidaCreada;


    public void OnEvent(EventData ev)
    {
        switch (ev.Code)
        {
            case CodigosRed.Listo: RecibirListo(ev.Sender); break;
            case CodigosRed.Comando: RecibirComando(ev.Sender, ev.CustomData); break;
        }
    }

    void RecibirListo(int actor)
    {
        if (partida != null) return;
        listos.Add(actor);
        Debug.Log($"[Host] Listo de {actor} ({listos.Count}/{PhotonNetwork.CurrentRoom.PlayerCount})");
        if (listos.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            IniciarPartida();
    }

    void IniciarPartida()
    {
        var jugadores = (Player[])PhotonNetwork.PlayerList.Clone();
        System.Array.Sort(jugadores, (a, b) => a.ActorNumber.CompareTo(b.ActorNumber));

        int n = jugadores.Length;
        actores = new int[n];
        var nombres = new string[n];
        for (int i = 0; i < n; i++)
        {
            actores[i] = jugadores[i].ActorNumber;
            nombres[i] = jugadores[i].NickName;
        }

        int inicial = Random.Range(0, n);
        int semilla = Random.Range(0, int.MaxValue);
        partida = new Partida(nombres, new ReglaLicuados(), semilla, inicial);
        PartidaCreada?.Invoke(partida);
        partida.Iniciar();

        EnvioRed.MensajeATodos($"Sorteo: empieza {nombres[inicial]}.", false);
        EnviarEstados();
    }


    void EnviarEstados()
    {
        for (int asiento = 0; asiento < actores.Length; asiento++)
            EnvioRed.A(actores[asiento], CodigosRed.Estado, SerializadorEstado.Empacar(partida.VistaPara(asiento)));
    }



    void RecibirComando(int actor, object datos)
    {
        Debug.Log($"[Host] Comando de actor {actor}: {(datos is int[] x ? string.Join(",", x) : "datos raros")}");
        if (partida == null) return;
        int asiento = System.Array.IndexOf(actores, actor);
        if (asiento < 0 || !(datos is int[] d) || d.Length < 2) return;

        d[1] = asiento;

        IComandoJugada cmd;
        try
        {
            cmd = ComandoSerializer.Deserializar(d);
        }
        catch (System.ArgumentException)
        {
            EnvioRed.MensajeA(actor, "Jugada inválida.", true);
            return;
        }

        ResultadoConexion r = partida.Enviar(cmd);
        if (!r.Valida)
        {
            EnvioRed.MensajeA(actor, r.Mensaje, true);
            return;
        }
        EnviarEstados();
    }
}