using System;
using Licuadomino.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiaLicuados : MonoBehaviour
{
    [SerializeField] FrutaSprites sprites;
    [SerializeField] GameObject panel;
    [SerializeField] RectTransform filas;
    [SerializeField] Button botonListo;
    [SerializeField] GameObject textoEsperando;
    [SerializeField] float tamFruta = 90f;
    [SerializeField] float tamTexto = 34f;
    [SerializeField] Color colorTexto = new Color32(0x33, 0x33, 0x33, 0xFF);

    Action alListo;
    bool esperarALosDemas;
    bool construida;

    public void Mostrar(Action alListo, bool esperarALosDemas)
    {
        if (!construida) Construir();
        this.alListo = alListo;
        this.esperarALosDemas = esperarALosDemas;
        panel.SetActive(true);
        botonListo.gameObject.SetActive(true);
        textoEsperando.SetActive(false);
    }

    void Cerrar()
    {
        Action accion = alListo;
        alListo = null;

        if (esperarALosDemas)
        {
            botonListo.gameObject.SetActive(false);
            textoEsperando.SetActive(true);
        }
        else
        {
            Ocultar();
        }
        accion?.Invoke();
    }

    public void Ocultar() => panel.SetActive(false);


    void Construir()
    {
        construida = true;
        botonListo.onClick.AddListener(Cerrar);

        var v = filas.GetComponent<VerticalLayoutGroup>();
        if (v == null) v = filas.gameObject.AddComponent<VerticalLayoutGroup>();
        v.spacing = 10;
        v.childAlignment = TextAnchor.UpperCenter;
        v.childControlWidth = v.childControlHeight = false;
        v.childForceExpandWidth = v.childForceExpandHeight = false;

        foreach (Perfil p in Enum.GetValues(typeof(Perfil)))
        {
            Perfil pareja = FrutaInfo.Complemento(p);
            if (pareja < p) continue;
            CrearFila(p, pareja);
        }
    }

    void CrearFila(Perfil a, Perfil b)
    {
        var fila = NuevoHijo("Fila " + FrutaInfo.Adjetivo(a), filas);
        fila.sizeDelta = new Vector2(960, tamFruta + 40);
        var h = fila.gameObject.AddComponent<HorizontalLayoutGroup>();
        h.spacing = 16;
        h.childAlignment = TextAnchor.MiddleCenter;
        h.childControlWidth = h.childControlHeight = false;
        h.childForceExpandWidth = h.childForceExpandHeight = false;

        foreach (Fruta f in FrutaInfo.DePerfil(a)) CrearFruta(f, fila);
        CrearTexto($"{Mayuscula(FrutaInfo.Adjetivo(a))}  +  {Mayuscula(FrutaInfo.Adjetivo(b))}", fila, 420, tamFruta);
        foreach (Fruta f in FrutaInfo.DePerfil(b)) CrearFruta(f, fila);

        var razon = CrearTexto(FrutaInfo.RazonBalance(a), filas, 960, 44);
        razon.fontSize = tamTexto * 0.75f;
        razon.fontStyle = FontStyles.Italic;
    }



    void CrearFruta(Fruta f, Transform padre)
    {
        var col = NuevoHijo(FrutaInfo.Nombre(f), padre);
        col.sizeDelta = new Vector2(tamFruta + 20, tamFruta + 40);

        var img = NuevoHijo("Imagen", col).gameObject.AddComponent<Image>();
        img.sprite = sprites.SpriteDe(f);
        img.preserveAspect = true;
        img.raycastTarget = false;
        Colocar(img.rectTransform, new Vector2(0.5f, 1f), new Vector2(tamFruta, tamFruta));

        var nombre = CrearTexto(FrutaInfo.Nombre(f), col, tamFruta + 20, 34);
        nombre.fontSize = tamTexto * 0.6f;
        Colocar(nombre.rectTransform, new Vector2(0.5f, 0f), new Vector2(tamFruta + 20, 34));
    }

    TextMeshProUGUI CrearTexto(string texto, Transform padre, float ancho, float alto)
    {
        var t = NuevoHijo("Texto", padre).gameObject.AddComponent<TextMeshProUGUI>();
        t.text = texto;
        t.fontSize = tamTexto;
        t.color = colorTexto;
        t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false;
        t.rectTransform.sizeDelta = new Vector2(ancho, alto);
        return t;
    }

    static RectTransform NuevoHijo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        return (RectTransform)go.transform;
    }

    static void Colocar(RectTransform rt, Vector2 ancla, Vector2 tam)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = ancla;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = tam;
    }

    static string Mayuscula(string s) => char.ToUpper(s[0]) + s.Substring(1);
}