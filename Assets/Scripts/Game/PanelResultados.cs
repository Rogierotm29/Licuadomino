using System.Linq;
using System.Text;
using Licuadomino.Core;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PanelResultados : MonoBehaviour
{
    [SerializeField] PantallaJuego pantalla;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text titulo;
    [SerializeField] TMP_Text motivo;
    [SerializeField] TMP_Text tabla;
    [SerializeField] Color colorGana = new Color32(0x4C, 0xAF, 0x50, 0xFF);
    [SerializeField] Color colorPierde = new Color32(0xE8, 0x50, 0x5B, 0xFF);
    [SerializeField] Color colorNeutro = new Color32(0x33, 0x33, 0x33, 0xFF);

    void OnEnable()
    {
        pantalla.EstadoMostrado += AlMostrar;
        panel.SetActive(false);
    }

    void OnDisable() => pantalla.EstadoMostrado -= AlMostrar;

    void AlMostrar(EstadoVisible e)
    {
        if (!e.Terminada || e.Resultado == null)
        {
            panel.SetActive(false);
            return;
        }

        ResultadoPartida r = e.Resultado;
        PonerTitulo(e, r);
        motivo.text = Motivo(r);
        tabla.text = Tabla(e, r);
        panel.SetActive(true);
    }

    void PonerTitulo(EstadoVisible e, ResultadoPartida r)
    {
        if (r.EsEmpate)
        {
            titulo.text = "¡Empate!";
            titulo.color = colorNeutro;
        }
        else if (!PhotonNetwork.InRoom)
        {
            titulo.text = $"¡Gana {r.Tabla[0].Nombre}!";
            titulo.color = colorGana;
        }
        else
        {
            bool gane = r.GanadorId == e.MiId;
            titulo.text = gane ? "¡Ganaste!" : "Perdiste";
            titulo.color = gane ? colorGana : colorPierde;
        }
    }

    static string Motivo(ResultadoPartida r)
    {
        switch (r.Motivo)
        {
            case MotivoFin.Tranca:
                return r.EsEmpate
                    ? "Juego trancado: nadie podía jugar y quedaron con las mismas fichas."
                    : "Juego trancado: nadie podía jugar. Gana quien tenía menos fichas.";
            case MotivoFin.Abandono:
                return $"{r.Tabla[r.Tabla.Count - 1].Nombre} abandonó la partida.";
            default:
                return $"{r.Tabla[0].Nombre} se quedó sin fichas.";
        }
    }

    static string Tabla(EstadoVisible e, ResultadoPartida r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<b>Lugar<pos=18%>Jugador<pos=62%>Fichas<pos=82%>Puntos</b>");

        int idAbandono = r.Motivo == MotivoFin.Abandono ? r.Tabla[r.Tabla.Count - 1].JugadorId : -1;

        foreach (Posicion x in r.Tabla)
        {
            string lugar = x.JugadorId == idAbandono
                ? "—"
                : (1 + r.Tabla.Count(o => o.JugadorId != idAbandono && o.FichasRestantes < x.FichasRestantes)).ToString();
            string nombre = PhotonNetwork.InRoom && x.JugadorId == e.MiId ? $"<b>{x.Nombre} (tú)</b>" : x.Nombre;
            int puntos = x.JugadorId == r.GanadorId ? r.Puntos : 0;
            sb.AppendLine($"{lugar}<pos=18%>{nombre}<pos=62%>{x.FichasRestantes}<pos=82%>{puntos}");
        }
        return sb.ToString();
    }
}