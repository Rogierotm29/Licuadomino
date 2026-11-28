using Licuadomino.Core;
using UnityEngine;

public class TelemetriaRed : MonoBehaviour
{
    [SerializeField] HostPartida host;

    readonly Telemetria telemetria = new Telemetria();

    public int Partidas => telemetria.Partidas.Count;
    public string ComoCsv() => telemetria.ComoCsv();

    void OnEnable() => host.PartidaCreada += Escuchar;
    void OnDisable() => host.PartidaCreada -= Escuchar;

    void Escuchar(Partida p)
    {
        telemetria.Observar(p);
        p.PartidaTerminada += r => Debug.Log("[Telemetría]\n" + telemetria.ComoCsv());
    }
}