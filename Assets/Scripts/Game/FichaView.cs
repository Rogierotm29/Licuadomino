using System;
using Licuadomino.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class FichaView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const float Largo = 160f;
    public const float Ancho = 80f;
    const float Margen = 10f;
    const float EscalaHover = 1.08f;

    [SerializeField] FrutaSprites sprites;
    [SerializeField] Image marco;
    [SerializeField] Image fondo;
    [SerializeField] Image divisor;
    [SerializeField] Image frutaA;
    [SerializeField] Image frutaB;
    [SerializeField] Image reverso;
    [SerializeField] CanvasGroup grupo;

    public Ficha Ficha { get; private set; }
    public bool BocaAbajo { get; private set; }
    public bool Horizontal { get; private set; }
    public bool Seleccionada { get; private set; }

    /// <summary>Si es false, no reacciona a hover ni a clic (fichas de la mesa, del rival...).</summary>
    public bool Interactiva { get; set; } = true;

    public event Action<FichaView> Clic;

    RectTransform rt;
    bool hover;

    RectTransform Rt => rt != null ? rt : (rt = (RectTransform)transform);


    public void Mostrar(Ficha f, Fruta primera, Fruta segunda, bool horizontal)
    {
        Ficha = f;
        BocaAbajo = false;
        Horizontal = horizontal;
        frutaA.sprite = sprites.SpriteDe(primera);
        frutaB.sprite = sprites.SpriteDe(segunda);
        MostrarFrente(true);
        Acomodar();
        name = "Ficha " + f;
    }

    public void Mostrar(Ficha f, bool horizontal = false) => Mostrar(f, f.A, f.B, horizontal);

    public void MostrarBocaAbajo(bool horizontal = false)
    {
        BocaAbajo = true;
        Horizontal = horizontal;
        MostrarFrente(false);
        Acomodar();
        name = "Ficha (boca abajo)";
    }

    public void SetSeleccionada(bool valor)
    {
        Seleccionada = valor;
        marco.enabled = valor;
    }

    public void SetJugable(bool valor) => grupo.alpha = valor ? 1f : 0.45f;



    public void OnPointerClick(PointerEventData e)
    {
        if (Interactiva) Clic?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!Interactiva || hover) return;
        hover = true;
        Rt.localScale *= EscalaHover;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!hover) return;
        hover = false;
        Rt.localScale /= EscalaHover;
    }

    void OnDisable()
    {
        if (hover) OnPointerExit(null);
    }


    void MostrarFrente(bool frente)
    {
        fondo.enabled = frente;
        divisor.enabled = frente;
        frutaA.enabled = frente;
        frutaB.enabled = frente;
        reverso.enabled = !frente;
    }

    void Acomodar()
    {
        float lado = Ancho - 2 * Margen; 

        if (Horizontal)
        {
            Rt.sizeDelta = new Vector2(Largo, Ancho);
            Poner(frutaA.rectTransform, new Vector2(-Largo / 4f, 0f), lado);
            Poner(frutaB.rectTransform, new Vector2(Largo / 4f, 0f), lado);
            divisor.rectTransform.sizeDelta = new Vector2(4f, Ancho * 0.7f);
        }
        else
        {
            Rt.sizeDelta = new Vector2(Ancho, Largo);
            Poner(frutaA.rectTransform, new Vector2(0f, Largo / 4f), lado);
            Poner(frutaB.rectTransform, new Vector2(0f, -Largo / 4f), lado);
            divisor.rectTransform.sizeDelta = new Vector2(Ancho * 0.7f, 4f);
        }
        divisor.rectTransform.anchoredPosition = Vector2.zero;


        RectTransform rv = reverso.rectTransform;
        rv.sizeDelta = new Vector2(Ancho, Largo);
        rv.anchoredPosition = Vector2.zero;
        rv.localEulerAngles = new Vector3(0f, 0f, Horizontal ? 90f : 0f);
    }

    static void Poner(RectTransform r, Vector2 pos, float lado)
    {
        r.anchoredPosition = pos;
        r.sizeDelta = new Vector2(lado, lado);
    }
}
