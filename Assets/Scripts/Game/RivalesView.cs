using System.Collections.Generic;
using Licuadomino.Core;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class RivalesView : MonoBehaviour
{
    [SerializeField] FichaView prefabFicha;
    [SerializeField] float escalaFicha = 0.5f;
    [SerializeField] float pasoFicha = 24f;
    [SerializeField] Color colorNormal = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] Color colorTurno = new Color(0.91f, 0.31f, 0.36f);

    readonly List<GameObject> grupos = new List<GameObject>();

    public void Mostrar(EstadoVisible e)
    {
        foreach (var g in grupos) Destroy(g);
        grupos.Clear();

        var rivales = new List<int>();
        for (int i = 0; i < e.CantidadJugadores; i++)
            if (i != e.MiId) rivales.Add(i);
        if (rivales.Count == 0) return;

        Rect r = ((RectTransform)transform).rect;
        float paso = r.width / rivales.Count;

        for (int k = 0; k < rivales.Count; k++)
        {
            int id = rivales[k];
            bool suTurno = !e.Terminada && e.Turno == id;
            int n = e.FichasPorJugador[id];

            var grupo = new GameObject("Rival " + e.Nombres[id], typeof(RectTransform));
            var gr = (RectTransform)grupo.transform;
            gr.SetParent(transform, false);
            gr.anchoredPosition = new Vector2(-r.width / 2f + paso * (k + 0.5f), 0f);
            gr.sizeDelta = new Vector2(paso, r.height);
            grupos.Add(grupo);


            var tgo = new GameObject("Nombre", typeof(RectTransform));
            var tr = (RectTransform)tgo.transform;
            tr.SetParent(gr, false);
            tr.anchoredPosition = new Vector2(0f, r.height / 2f - 22f);
            tr.sizeDelta = new Vector2(paso, 40f);
            var t = tgo.AddComponent<TextMeshProUGUI>();
            t.text = (suTurno ? "> " : "") + $"{e.Nombres[id]}  ·  {n} ficha{(n == 1 ? "" : "s")}";
            t.fontSize = 28;
            t.alignment = TextAlignmentOptions.Center;
            t.color = suTurno ? colorTurno : colorNormal;
            t.fontStyle = suTurno ? FontStyles.Bold : FontStyles.Normal;
            t.raycastTarget = false;


            float x0 = -(n - 1) * pasoFicha / 2f;
            for (int i = 0; i < n; i++)
            {
                FichaView v = Instantiate(prefabFicha, gr);
                v.MostrarBocaAbajo();
                v.Interactiva = false;
                var vr = (RectTransform)v.transform;
                vr.localScale = new Vector3(escalaFicha, escalaFicha, 1f);
                vr.anchoredPosition = new Vector2(x0 + i * pasoFicha, -18f);
            }
        }
    }
}
