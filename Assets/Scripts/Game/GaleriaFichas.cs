using Licuadomino.Core;
using TMPro;
using UnityEngine;


public class GaleriaFichas : MonoBehaviour
{
    [SerializeField] FichaView prefabFicha;
    [SerializeField] RectTransform contenedor;
    [SerializeField] TMP_Text textoInfo;

    FichaView seleccionada;

    void Start()
    {
        MostrarSetCompleto();
        MostrarCadenaDeEjemplo();
        MostrarBocaAbajo();
        textoInfo.text = "Dale clic a una ficha.";
    }

    void MostrarSetCompleto()
    {
        const int columnas = 12;
        float sepX = FichaView.Ancho + 20f;
        float sepY = FichaView.Largo + 20f;
        float x0 = -(columnas - 1) * sepX / 2f;
        const float y0 = 400f;

        var set = GeneradorFichas.SetCompleto;
        for (int i = 0; i < set.Count; i++)
        {
            FichaView v = Crear(new Vector2(x0 + (i % columnas) * sepX, y0 - (i / columnas) * sepY));
            v.Mostrar(set[i]);
        }
    }

    void MostrarCadenaDeEjemplo()
    {
        // Simula las primeras jugadas de una partida para tener una cadena válida.
        var p = new Partida(new[] { "A", "B" }, new ReglaLicuados(), 11);
        p.Iniciar();
        while (!p.Terminada && p.Tablero.Cadena.Count < 7)
        {
            var j = p.JugadorEnTurno;
            var jugadas = p.Tablero.JugadasPosibles(j.Mano);
            if (jugadas.Count > 0) p.Enviar(new ComandoColocar(j.Id, jugadas[0]));
            else if (p.FichasEnPozo > 0) p.Enviar(new ComandoRobar(j.Id));
            else p.Enviar(new ComandoPasar(j.Id));
        }

        var cadena = p.Tablero.Cadena;
        float sep = FichaView.Largo + 6f;
        float x0 = -(cadena.Count - 1) * sep / 2f;
        for (int i = 0; i < cadena.Count; i++)
        {
            FichaView v = Crear(new Vector2(x0 + i * sep, -170f));
            v.Mostrar(cadena[i].Ficha, cadena[i].Izq, cadena[i].Der, true);
        }
    }

    void MostrarBocaAbajo()
    {
        // Como la mano del rival: 7 fichas encimadas.
        for (int i = 0; i < 7; i++)
            Crear(new Vector2(-300f + i * 45f, -370f)).MostrarBocaAbajo();

        // Como el pozo: una ficha acostada.
        Crear(new Vector2(250f, -370f)).MostrarBocaAbajo(true);
    }

    FichaView Crear(Vector2 pos)
    {
        FichaView v = Instantiate(prefabFicha, contenedor);
        ((RectTransform)v.transform).anchoredPosition = pos;
        v.Clic += AlDarClic;
        return v;
    }

    void AlDarClic(FichaView v)
    {
        if (seleccionada != null) seleccionada.SetSeleccionada(false);
        if (seleccionada == v)
        {
            seleccionada = null;
            textoInfo.text = "Dale clic a una ficha.";
            return;
        }

        seleccionada = v;
        v.SetSeleccionada(true);

        if (v.BocaAbajo)
        {
            textoInfo.text = "Ficha boca abajo: no puedes ver qué frutas tiene.";
            return;
        }

        Ficha f = v.Ficha;
        textoInfo.text =
            $"{f}   {Describir(f.A)}  ·  {Describir(f.B)}" + (f.EsDoble ? "   (doble)" : "");
    }

    static string Describir(Fruta fr)
    {
        Perfil p = FrutaInfo.PerfilDe(fr);
        return $"{FrutaInfo.Nombre(fr)} es {FrutaInfo.Adjetivo(p)}, combina con {FrutaInfo.ListaDePerfil(FrutaInfo.Complemento(p))}";
    }
}
