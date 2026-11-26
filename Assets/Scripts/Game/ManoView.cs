using System;
using System.Collections.Generic;
using Licuadomino.Core;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ManoView : MonoBehaviour
{
    [SerializeField] FichaView prefabFicha;
    [SerializeField] float separacion = 14f;
    [SerializeField] float elevacion = 30f;

    readonly List<FichaView> vistas = new List<FichaView>();
    bool interactiva;

    public int SeleccionadaId { get; private set; } = -1;
    public event Action<int> SeleccionCambio;

    public void Mostrar(IReadOnlyList<Ficha> mano, bool puedeJugar)
    {
        interactiva = puedeJugar;
        foreach (var v in vistas)
        {
            v.gameObject.SetActive(false);
            Destroy(v.gameObject);
        }
        vistas.Clear();

        bool sigueEnMano = false;
        foreach (Ficha f in mano)
        {
            FichaView v = Instantiate(prefabFicha, transform);
            v.Mostrar(f);
            v.Interactiva = interactiva;
            v.Clic += AlDarClic;
            vistas.Add(v);
            if (f.Id == SeleccionadaId) sigueEnMano = true;
        }

        if (!sigueEnMano || !interactiva) SeleccionadaId = -1;
        Acomodar();
    }

    public void Deseleccionar()
    {
        SeleccionadaId = -1;
        Acomodar();
        SeleccionCambio?.Invoke(SeleccionadaId);
    }

    void AlDarClic(FichaView v)
    {
        if (!interactiva) return;
        SeleccionadaId = v.Ficha.Id == SeleccionadaId ? -1 : v.Ficha.Id;
        Acomodar();
        SeleccionCambio?.Invoke(SeleccionadaId);
    }

    void Acomodar()
    {
        int n = vistas.Count;
        if (n == 0) return;

        float ancho = ((RectTransform)transform).rect.width;
        float paso = FichaView.Ancho + separacion;
        if (n > 1) paso = Mathf.Min(paso, (ancho - FichaView.Ancho) / (n - 1)); 
        float x0 = -(n - 1) * paso / 2f;

        for (int i = 0; i < n; i++)
        {
            FichaView v = vistas[i];
            bool sel = v.Ficha.Id == SeleccionadaId;
            v.SetSeleccionada(sel);
            ((RectTransform)v.transform).anchoredPosition = new Vector2(x0 + i * paso, sel ? elevacion : 0f);
        }
    }
}
