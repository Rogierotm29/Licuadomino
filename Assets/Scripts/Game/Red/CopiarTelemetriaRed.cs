using Licuadomino.Core;
using Photon.Pun;
using UnityEngine;

public class CopiarTelemetriaRed : MonoBehaviour
{
    [SerializeField] ClienteRed cliente;
    [SerializeField] TelemetriaRed telemetria;
    [SerializeField] PantallaJuego pantalla;
    [SerializeField] GameObject boton;

    void OnEnable()
    {
        cliente.EstadoRecibido += AlRecibirEstado;
        boton.SetActive(false);
    }

    void OnDisable() => cliente.EstadoRecibido -= AlRecibirEstado;

    void AlRecibirEstado(EstadoVisible e) =>
        boton.SetActive(e.Terminada && PhotonNetwork.IsMasterClient);

    public void Copiar()
    {
        Portapapeles.Copiar(telemetria.ComoCsv());
        pantalla.MostrarMensaje($"Datos de {telemetria.Partidas} partida(s) copiados. Pégalos en tu hoja de cálculo.", false);
    }
}