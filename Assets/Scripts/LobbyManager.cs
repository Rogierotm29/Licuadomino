using System.Text;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Panel de conexión")]
    [SerializeField] GameObject panelConexion;
    [SerializeField] TMP_InputField inputNombre;
    [SerializeField] TMP_InputField inputCodigo;
    [SerializeField] Button botonCrear;
    [SerializeField] Button botonUnirse;

    [Header("Panel de sala")]
    [SerializeField] GameObject panelSala;
    [SerializeField] TMP_Text textoCodigo;
    [SerializeField] TMP_Text textoJugadores;
    [SerializeField] Button botonIniciar;
    [SerializeField] Button botonSalir;

    [Header("General")]
    [SerializeField] TMP_Text textoEstado;

    const int MaxJugadores = 4;
    const int MinJugadores = 2;
    const int LargoCodigo = 4;
    const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; 
    const string PropIniciada = "iniciada";
    const string PrefNombre = "nombre";

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        botonCrear.onClick.AddListener(CrearSala);
        botonUnirse.onClick.AddListener(UnirseSala);
        botonIniciar.onClick.AddListener(IniciarPartida);
        botonSalir.onClick.AddListener(SalirSala);

        inputNombre.text = PlayerPrefs.GetString(PrefNombre, "Jugador" + Random.Range(100, 999));
        inputCodigo.characterLimit = LargoCodigo;
        inputCodigo.onValueChanged.AddListener(t => inputCodigo.SetTextWithoutNotify(t.ToUpper()));

        MostrarPanelSala(false);
        BotonesConexion(false);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            OnConnectedToMaster();
        }
        else
        {
            Estado("Conectando a Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }



    void CrearSala()
    {
        if (!AplicarNombre()) return;
        BotonesConexion(false);
        IntentarCrear();
    }

    void IntentarCrear()
    {
        string codigo = GenerarCodigo();
        Estado("Creando sala " + codigo + "...");
        var opciones = new RoomOptions
        {
            MaxPlayers = MaxJugadores,
            IsVisible = false, 
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(codigo, opciones);
    }

    void UnirseSala()
    {
        if (!AplicarNombre()) return;
        string codigo = inputCodigo.text.Trim().ToUpper();
        if (codigo.Length != LargoCodigo)
        {
            Estado("El código debe tener " + LargoCodigo + " caracteres.");
            return;
        }
        BotonesConexion(false);
        Estado("Entrando a " + codigo + "...");
        PhotonNetwork.JoinRoom(codigo);
    }

    void IniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < MinJugadores) return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
 
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { PropIniciada, true } });

    }

    void SalirSala()
    {
        Estado("Saliendo...");
        PhotonNetwork.LeaveRoom();
    }

  

    public override void OnConnectedToMaster()
    {
        Estado("Conectado (región: " + PhotonNetwork.CloudRegion + ")");
        BotonesConexion(true);
    }

    public override void OnDisconnected(DisconnectCause causa)
    {
        Estado("Desconectado: " + causa + ". Reintentando...");
        MostrarPanelSala(false);
        BotonesConexion(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnCreateRoomFailed(short codigo, string mensaje)
    {

        Debug.LogWarning("Crear sala falló (" + codigo + "): " + mensaje);
        IntentarCrear();
    }

    public override void OnJoinRoomFailed(short codigo, string mensaje)
    {
        Estado(codigo switch
        {
            ErrorCode.GameDoesNotExist => "No existe ninguna sala con ese código.",
            ErrorCode.GameFull => "La sala está llena.",
            ErrorCode.GameClosed => "Esa partida ya empezó.",
            _ => "No se pudo entrar: " + mensaje
        });
        BotonesConexion(true);
    }

    public override void OnJoinedRoom()
    {
        Estado("En la sala.");
        MostrarPanelSala(true);
        ActualizarSala();
    }

    public override void OnLeftRoom()
    {
        MostrarPanelSala(false);
     
    }

    public override void OnPlayerEnteredRoom(Player nuevo) => ActualizarSala();
    public override void OnPlayerLeftRoom(Player otro) => ActualizarSala();
    public override void OnMasterClientSwitched(Player nuevoHost) => ActualizarSala();

    public override void OnRoomPropertiesUpdate(Hashtable cambios)
    {
        if (cambios.TryGetValue(PropIniciada, out object valor) && valor is bool b && b)
            Estado("¡Partida iniciada! Todos deberían ver este mensaje.");
    }

 

    void ActualizarSala()
    {
        if (!PhotonNetwork.InRoom) return;
        Room sala = PhotonNetwork.CurrentRoom;

        textoCodigo.text = "Código: " + sala.Name;

        var sb = new StringBuilder();
        sb.AppendLine($"Jugadores ({sala.PlayerCount}/{MaxJugadores}):");
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            sb.Append("• ").Append(p.NickName);
            if (p.IsMasterClient) sb.Append("  [host]");
            if (p.IsLocal) sb.Append("  (tú)");
            sb.AppendLine();
        }
        textoJugadores.text = sb.ToString();

        botonIniciar.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        botonIniciar.interactable = sala.PlayerCount >= MinJugadores;
    }

    bool AplicarNombre()
    {
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            Estado("Escribe tu nombre.");
            return false;
        }
        PhotonNetwork.NickName = nombre;
        PlayerPrefs.SetString(PrefNombre, nombre);
        return true;
    }

    static string GenerarCodigo()
    {
        var sb = new StringBuilder(LargoCodigo);
        for (int i = 0; i < LargoCodigo; i++)
            sb.Append(Caracteres[Random.Range(0, Caracteres.Length)]);
        return sb.ToString();
    }

    void MostrarPanelSala(bool enSala)
    {
        panelConexion.SetActive(!enSala);
        panelSala.SetActive(enSala);
    }

    void BotonesConexion(bool activos)
    {
        botonCrear.interactable = activos;
        botonUnirse.interactable = activos;
    }

    void Estado(string msg)
    {
        textoEstado.text = msg;
        Debug.Log("[Lobby] " + msg);
    }
}