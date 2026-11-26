using System.Collections.Generic;
using Licuadomino.Core;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MesaView : MonoBehaviour
{
    [SerializeField] FichaView prefabFicha;
    [Tooltip("Ancho máximo de una fila antes de dar vuelta (en unidades del canvas, sin escalar).")]
    [SerializeField] float anchoFila = 1500f;
    [SerializeField] float separacion = 6f;
    [Tooltip("La mesa nunca se dibuja más grande que esto.")]
    [SerializeField] float escalaMaxima = 1f;

    const float Largo = FichaView.Largo;
    const float Ancho = FichaView.Ancho;

    readonly Dictionary<int, FichaView> vistas = new Dictionary<int, FichaView>();
    readonly List<Colocacion> colocaciones = new List<Colocacion>();

    RectTransform area;
    RectTransform contenido;

    public Vector2 SiguienteIzquierda { get; private set; }
    public Vector2 SiguienteDerecha { get; private set; }
    public float Escala => contenido != null ? contenido.localScale.x : 1f;

    struct Colocacion
    {
        public FichaColocada c;
        public Vector2 pos;
        public Fruta primera;   
        public Fruta segunda;  
        public bool horizontal;
    }

    void Awake() => Preparar();

    void Preparar()
    {
        if (contenido != null) return;
        area = (RectTransform)transform;
        var go = new GameObject("Contenido", typeof(RectTransform));
        contenido = (RectTransform)go.transform;
        contenido.SetParent(area, false);
    }

    public void Limpiar()
    {
        Preparar();
        foreach (var v in vistas.Values)
            if (v != null) Destroy(v.gameObject);
        vistas.Clear();
        SiguienteIzquierda = SiguienteDerecha = Vector2.zero;
    }

    public void Mostrar(IReadOnlyList<FichaColocada> cadena, int idFichaInicial)
    {
        Preparar();
        if (cadena == null || cadena.Count == 0)
        {
            Limpiar();
            return;
        }

        int ancla = 0;
        for (int i = 0; i < cadena.Count; i++)
            if (cadena[i].Ficha.Id == idFichaInicial) { ancla = i; break; }

        colocaciones.Clear();


        FichaColocada c0 = cadena[ancla];
        Agregar(c0, Vector2.zero, c0.Izq, c0.Der, true);
        float medio = Largo / 2f;


        var derecho = new List<(FichaColocada c, Fruta adentro, Fruta afuera)>();
        for (int i = ancla + 1; i < cadena.Count; i++)
            derecho.Add((cadena[i], cadena[i].Izq, cadena[i].Der));
        SiguienteDerecha = Brazo(derecho, medio + separacion, +1, -1);

        var izquierdo = new List<(FichaColocada c, Fruta adentro, Fruta afuera)>();
        for (int i = ancla - 1; i >= 0; i--)
            izquierdo.Add((cadena[i], cadena[i].Der, cadena[i].Izq));
        SiguienteIzquierda = Brazo(izquierdo, -(medio + separacion), -1, +1);

        Sincronizar();
        AjustarEscala();
    }

    Vector2 Brazo(List<(FichaColocada c, Fruta adentro, Fruta afuera)> fichas, float cursor, int dir, int giro)
    {
        float y = 0f;
        float limite = anchoFila / 2f;

        foreach (var item in fichas)
        {
            FichaColocada c = item.c;
            Fruta adentro = item.adentro;
            Fruta afuera = item.afuera;
            const float tam = Largo;

            if (dir * cursor + tam > limite)
            {

                var pos = new Vector2(cursor + dir * Ancho / 2f, y + giro * (Largo - Ancho) / 2f);
                if (giro < 0) Agregar(c, pos, adentro, afuera, false); 
                else Agregar(c, pos, afuera, adentro, false);         
                y += giro * (Largo - Ancho + separacion);
                cursor -= dir * separacion;
                dir = -dir;
                continue;
            }

            var centro = new Vector2(cursor + dir * tam / 2f, y);
            if (dir > 0) Agregar(c, centro, adentro, afuera, true);
            else Agregar(c, centro, afuera, adentro, true);

            cursor += dir * (tam + separacion);
        }

        return new Vector2(cursor + dir * Largo / 2f, y);
    }

    void Agregar(FichaColocada c, Vector2 pos, Fruta primera, Fruta segunda, bool horizontal)
    {
        colocaciones.Add(new Colocacion
        {
            c = c, pos = pos, primera = primera, segunda = segunda, horizontal = horizontal
        });
    }

    void Sincronizar()
    {
        var vivos = new HashSet<int>();
        foreach (var col in colocaciones)
        {
            int id = col.c.Ficha.Id;
            vivos.Add(id);
            if (!vistas.TryGetValue(id, out FichaView v) || v == null)
            {
                v = Instantiate(prefabFicha, contenido);
                v.Interactiva = false;
                vistas[id] = v;
            }
            v.Mostrar(col.c.Ficha, col.primera, col.segunda, col.horizontal);
            ((RectTransform)v.transform).anchoredPosition = col.pos;
        }

        var sobran = new List<int>();
        foreach (var kv in vistas)
            if (!vivos.Contains(kv.Key)) sobran.Add(kv.Key);
        foreach (int id in sobran)
        {
            if (vistas[id] != null) Destroy(vistas[id].gameObject);
            vistas.Remove(id);
        }
    }

    void AjustarEscala()
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (var col in colocaciones)
        {
            float w = (col.horizontal ? Largo : Ancho) / 2f;
            float h = (col.horizontal ? Ancho : Largo) / 2f;
            minX = Mathf.Min(minX, col.pos.x - w);
            maxX = Mathf.Max(maxX, col.pos.x + w);
            minY = Mathf.Min(minY, col.pos.y - h);
            maxY = Mathf.Max(maxY, col.pos.y + h);
        }

        float ancho = maxX - minX, alto = maxY - minY;
        Rect r = area.rect;
        float escala = Mathf.Min(escalaMaxima, r.width / ancho, r.height / alto);
        var centro = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);

        contenido.localScale = new Vector3(escala, escala, 1f);
        contenido.anchoredPosition = -centro * escala;


        SiguienteIzquierda = contenido.anchoredPosition + SiguienteIzquierda * escala;
        SiguienteDerecha = contenido.anchoredPosition + SiguienteDerecha * escala;
    }
}
