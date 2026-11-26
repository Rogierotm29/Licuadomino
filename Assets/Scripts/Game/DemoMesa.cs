using System.Collections;
using Licuadomino.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DemoMesa : MonoBehaviour
{
    [SerializeField] MesaView mesa;
    [SerializeField] TMP_Text texto;
    [SerializeField] Button botonPaso;
    [SerializeField] Button botonAuto;
    [SerializeField] Button botonLlenar;
    [SerializeField] Button botonNueva;
    [SerializeField] float segundosPorJugada = 0.35f;

    Partida partida;
    System.Random rng;
    Coroutine auto;

    void Start()
    {
        if (mesa == null) mesa = FindAnyObjectByType<MesaView>();
        if (mesa == null) { Debug.LogError("[DemoMesa] No hay ninguna MesaView en la escena."); return; }
        botonPaso.onClick.AddListener(Paso);
        botonAuto.onClick.AddListener(AlternarAuto);
        botonLlenar.onClick.AddListener(LlenarMesa);
        botonNueva.onClick.AddListener(NuevaPartida);
        NuevaPartida();
    }

    void NuevaPartida()
    {
        DetenerAuto();
        int semilla = Random.Range(0, 100000);
        rng = new System.Random(semilla);

        partida = new Partida(new[] { "Bot A", "Bot B" }, new ReglaLicuados(), semilla);
        partida.FichaJugada += (j, jugada, msg) => texto.text = $"{j.Nombre}: {jugada.Ficha}. {msg}";
        partida.FichaRobada += (j, f) => texto.text = $"{j.Nombre} roba del pozo ({partida.FichasEnPozo} quedan).";
        partida.TurnoPasado += j => texto.text = $"{j.Nombre} pasa.";
        partida.PartidaTerminada += r => texto.text = $"FIN: {r}  ({partida.Tablero.Cadena.Count} fichas en la mesa)";

        partida.Iniciar();
        mesa.Limpiar();
        texto.text = $"Partida nueva (semilla {semilla}). Dale a Siguiente jugada o Auto.";
    }

    void Paso()
    {
        if (partida.Terminada) return;
        Jugador j = partida.JugadorEnTurno;
        var jugadas = partida.Tablero.JugadasPosibles(j.Mano);
        if (jugadas.Count > 0) partida.Enviar(new ComandoColocar(j.Id, jugadas[rng.Next(jugadas.Count)]));
        else if (partida.FichasEnPozo > 0) partida.Enviar(new ComandoRobar(j.Id));
        else partida.Enviar(new ComandoPasar(j.Id));

        mesa.Mostrar(partida.Tablero.Cadena, partida.Tablero.IdFichaInicial);
    }

    void AlternarAuto()
    {
        if (auto != null) { DetenerAuto(); return; }
        auto = StartCoroutine(Auto());
    }

    IEnumerator Auto()
    {
        while (!partida.Terminada)
        {
            Paso();
            yield return new WaitForSeconds(segundosPorJugada);
        }
        auto = null;
    }

    void DetenerAuto()
    {
        if (auto != null) StopCoroutine(auto);
        auto = null;
    }

    void LlenarMesa()
    {
        DetenerAuto();
        var t = new Tablero(new ReglaLicuados());
        var resto = GeneradorFichas.Barajado(rng);

        bool puso = true;
        while (puso)
        {
            puso = false;
            for (int i = 0; i < resto.Count; i++)
            {
                var jugadas = t.JugadasPosibles(resto[i]);
                if (jugadas.Count == 0) continue;
                t.Colocar(jugadas[rng.Next(jugadas.Count)]);
                resto.RemoveAt(i);
                puso = true;
                break;
            }
        }

        mesa.Mostrar(t.Cadena, t.IdFichaInicial);
        texto.text = $"Mesa de prueba: {t.Cadena.Count} fichas (sobraron {resto.Count} que ya no combinaban). " +
                     "Nueva partida para volver al juego.";
    }
}
