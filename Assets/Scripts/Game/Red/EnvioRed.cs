using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public static class EnvioRed
{
    public static void AlHost(byte codigo, object datos) =>
        Enviar(codigo, datos, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient });

    public static void ATodos(byte codigo, object datos) =>
        Enviar(codigo, datos, new RaiseEventOptions { Receivers = ReceiverGroup.All });

    public static void A(int actor, byte codigo, object datos) =>
        Enviar(codigo, datos, new RaiseEventOptions { TargetActors = new[] { actor } });

    public static void MensajeATodos(string texto, bool esError) =>
        ATodos(CodigosRed.Mensaje, new object[] { texto, esError });

    public static void MensajeA(int actor, string texto, bool esError) =>
        A(actor, CodigosRed.Mensaje, new object[] { texto, esError });

    static void Enviar(byte codigo, object datos, RaiseEventOptions opciones) =>
        PhotonNetwork.RaiseEvent(codigo, datos, opciones, SendOptions.SendReliable);
}