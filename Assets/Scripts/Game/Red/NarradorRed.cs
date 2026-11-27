using Licuadomino.Core;
using UnityEngine;

public class NarradorRed : MonoBehaviour
{
    [SerializeField] HostPartida host;

    void OnEnable() => host.PartidaCreada += Escuchar;
    void OnDisable() => host.PartidaCreada -= Escuchar;

    void Escuchar(Partida p)
    {
        p.FichaJugada += (j, jugada, msg) => EnvioRed.MensajeATodos($"{j.Nombre}: {msg}", false);
        p.FichaRobada += (j, ficha) => EnvioRed.MensajeATodos($"{j.Nombre} robó una ficha del pozo.", false);
        p.TurnoPasado += j => EnvioRed.MensajeATodos($"{j.Nombre} pasó: no tenía con qué jugar.", false);
        p.PartidaTerminada += r => EnvioRed.MensajeATodos(r.ToString(), false);
    }
}