using Licuadomino.Core;
using UnityEngine;

public class GuiaInicioRed : MonoBehaviour
{
    [SerializeField] GuiaLicuados guia;
    [SerializeField] ClienteRed cliente;

    void OnEnable() => cliente.EstadoRecibido += AlRecibirEstado;
    void OnDisable() => cliente.EstadoRecibido -= AlRecibirEstado;

    void Start() => guia.Mostrar(AvisarListo, true);

    void AvisarListo() => EnvioRed.AlHost(CodigosRed.Listo, null);

    void AlRecibirEstado(EstadoVisible e) => guia.Ocultar();
}