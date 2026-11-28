using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonSalir : MonoBehaviour
{
    [SerializeField] RegresoLobby regreso;

    public void Salir()
    {
        if (PhotonNetwork.InRoom) regreso.SalirAhora();
        else SceneManager.LoadScene("Inicio");
    }
}