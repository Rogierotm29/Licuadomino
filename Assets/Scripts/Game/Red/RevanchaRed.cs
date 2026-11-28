using Licuadomino.Core;
using Photon.Pun;
using UnityEngine;

public class RevanchaRed : MonoBehaviour
{
    [SerializeField] ClienteRed cliente;
    [SerializeField] GameObject botonRevancha;
    [SerializeField] GameObject textoEsperando;

    void OnEnable()
    {
        cliente.EstadoRecibido += AlRecibirEstado;
        Mostrar(false, false);
    }

    void OnDisable() => cliente.EstadoRecibido -= AlRecibirEstado;

    void AlRecibirEstado(EstadoVisible e)
    {
        bool terminoNormal = e.Terminada && e.Resultado != null && e.Resultado.Motivo != MotivoFin.Abandono;
        bool soyHost = PhotonNetwork.IsMasterClient;
        Mostrar(terminoNormal && soyHost, terminoNormal && !soyHost);
    }

    public void PedirRevancha()
    {
        Mostrar(false, true);
        EnvioRed.AlHost(CodigosRed.PedirRevancha, null);
    }

    void Mostrar(bool boton, bool esperando)
    {
        botonRevancha.SetActive(boton);
        textoEsperando.SetActive(esperando);
    }
}