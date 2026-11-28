using System;
using Licuadomino.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PantallaJuego : MonoBehaviour
{
    [SerializeField] MesaView mesa;
    [SerializeField] ManoView mano;
    [SerializeField] RivalesView rivales;
    [SerializeField] PozoView pozo;
    [SerializeField] BotonLado botonIzquierda;
    [SerializeField] BotonLado botonDerecha;
    [SerializeField] Button botonPasar;
    [SerializeField] TMP_Text textoTurno;
    [SerializeField] AvisoTemporal aviso;
    [SerializeField] Color colorNormal = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] Color colorError = new Color(0.8f, 0.15f, 0.2f);

    public event Action<IComandoJugada> Accion;

    public event System.Action<EstadoVisible> EstadoMostrado;
    public event System.Action<string, bool> MensajeMostrado;

    readonly IReglaCompatibilidad regla = new ReglaLicuados();
    EstadoVisible estado;

    void Awake()
    {
        mano.SeleccionCambio += _ => RefrescarLados();
        botonIzquierda.Clic += () => Poner(Lado.Izquierda);
        botonDerecha.Clic += () => Poner(Lado.Derecha);
        pozo.Clic += () => Enviar(new ComandoRobar(estado.MiId));
        botonPasar.onClick.AddListener(() => Enviar(new ComandoPasar(estado.MiId)));
    }

    public void Mostrar(EstadoVisible e)
    {
        estado = e;
        mesa.Mostrar(e.Cadena, e.IdFichaInicial);
        mano.Mostrar(e.MiMano, e.EsMiTurno);
        rivales.Mostrar(e);
        pozo.Mostrar(e.FichasEnPozo, e.EsMiTurno);
        botonPasar.interactable = e.EsMiTurno;

        if (e.Terminada)
            textoTurno.text = "Fin de la partida.";
        else if (e.EsMiTurno)
            textoTurno.text = $"Tu turno, {e.Nombres[e.MiId]}";
        else
            textoTurno.text = $"Turno de {e.Nombres[e.Turno]}";

        RefrescarLados();
        EstadoMostrado?.Invoke(e);
    }

    public void MostrarMensaje(string texto, bool esError)
    {
        aviso.Mostrar(texto, esError ? colorError : colorNormal);
        MensajeMostrado?.Invoke(texto, esError);
    }

    void RefrescarLados()
    {
        bool mostrar = estado != null && estado.EsMiTurno && mano.SeleccionadaId >= 0;
        if (!mostrar)
        {
            botonIzquierda.Ocultar();
            botonDerecha.Ocultar();
            return;
        }
        if (estado.MesaVacia)
        {
            botonIzquierda.Ocultar();
            botonDerecha.Mostrar(null, "Empezar aquí");
            return;
        }
        botonIzquierda.Mostrar(estado.ExtremoIzquierdo, "< Aquí");
        botonDerecha.Mostrar(estado.ExtremoDerecho, "Aquí >");
    }

    void Poner(Lado lado)
    {
        if (estado == null || !estado.EsMiTurno) return;
        int id = mano.SeleccionadaId;
        if (id < 0)
        {
            MostrarMensaje("Primero elige una ficha de tu mano.", true);
            return;
        }

        Ficha f = GeneradorFichas.PorId(id);
        if (estado.MesaVacia)
        {
            Enviar(new ComandoColocar(estado.MiId, f.Id, Lado.Derecha, f.A));
            return;
        }


        Fruta extremo = lado == Lado.Izquierda ? estado.ExtremoIzquierdo : estado.ExtremoDerecho;
        Fruta toca = regla.Evaluar(extremo, f.A).Valida ? f.A
                   : regla.Evaluar(extremo, f.B).Valida ? f.B
                   : f.A;
        Enviar(new ComandoColocar(estado.MiId, f.Id, lado, toca));
    }

    void Enviar(IComandoJugada cmd)
    {
        if (estado == null) return;
        Accion?.Invoke(cmd);
    }
}
