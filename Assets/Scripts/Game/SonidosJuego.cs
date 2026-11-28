using System.Collections;
using Licuadomino.Core;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidosJuego : MonoBehaviour
{
    [SerializeField] PantallaJuego pantalla;
    [SerializeField] AudioClip colocar;
    [SerializeField] AudioClip robar;
    [SerializeField] AudioClip doble;
    [SerializeField] AudioClip error;
    [SerializeField] AudioClip turno;
    [SerializeField] AudioClip fin;
    [SerializeField] float retrasoTurno = 0.35f;

    AudioSource fuente;
    EstadoVisible anterior;

    void Awake() => fuente = GetComponent<AudioSource>();

    void OnEnable()
    {
        pantalla.EstadoMostrado += AlMostrarEstado;
        pantalla.MensajeMostrado += AlMostrarMensaje;
    }

    void OnDisable()
    {
        pantalla.EstadoMostrado -= AlMostrarEstado;
        pantalla.MensajeMostrado -= AlMostrarMensaje;
    }

    void AlMostrarMensaje(string texto, bool esError)
    {
        if (esError) Tocar(error);
    }

    void AlMostrarEstado(EstadoVisible e)
    {
        EstadoVisible antes = anterior;
        anterior = e;

        if (e.Terminada)
        {
            if (antes == null || !antes.Terminada) Tocar(fin);
            return;
        }

        bool partidaNueva = antes == null || antes.Terminada || e.Cadena.Count < antes.Cadena.Count;
        if (!partidaNueva)
        {
            bool coloco = e.Cadena.Count > antes.Cadena.Count;
            bool alguienRecibio = AlguienRecibioFicha(antes, e);

            if (coloco && alguienRecibio) Tocar(doble);
            else if (coloco) Tocar(colocar);
            else if (alguienRecibio) Tocar(robar);
        }

        bool meToca = e.Turno == e.MiId && (partidaNueva || antes.Turno != e.Turno);
        if (meToca) StartCoroutine(TocarDespues(turno, retrasoTurno));
    }

    static bool AlguienRecibioFicha(EstadoVisible antes, EstadoVisible ahora)
    {
        if (antes.FichasPorJugador.Length != ahora.FichasPorJugador.Length) return false;
        for (int i = 0; i < ahora.FichasPorJugador.Length; i++)
            if (ahora.FichasPorJugador[i] > antes.FichasPorJugador[i]) return true;
        return false;
    }

    void Tocar(AudioClip clip)
    {
        if (clip != null) fuente.PlayOneShot(clip);
    }

    IEnumerator TocarDespues(AudioClip clip, float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Tocar(clip);
    }
}