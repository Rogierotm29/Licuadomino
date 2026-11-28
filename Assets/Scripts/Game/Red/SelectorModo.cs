using UnityEngine;
using Photon.Pun;

public class SelectorModo : MonoBehaviour
{
    [SerializeField] JuegoLocal juegoLocal;
    [SerializeField] GameObject red;
    [SerializeField] HostPartida host;

    [SerializeField] GameObject botonNuevaPartida;

    [SerializeField] GameObject botonSalir;

    [SerializeField] GuiaLicuados guia;
    void Awake()
    {
        bool enRed = PhotonNetwork.InRoom;
        juegoLocal.enabled = !enRed;
        host.enabled = enRed && PhotonNetwork.IsMasterClient;
        red.SetActive(enRed);
        botonNuevaPartida.SetActive(!enRed);
        botonSalir.SetActive(enRed);
        Debug.Log(enRed ? $"[Modo] Red ({(PhotonNetwork.IsMasterClient ? "host" : "cliente")})" : "[Modo] Local (prueba)");
        if (!enRed) guia.Mostrar(null, false);
    }
}
