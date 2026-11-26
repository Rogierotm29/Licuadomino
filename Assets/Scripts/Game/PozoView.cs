using System;
using TMPro;
using UnityEngine;


public class PozoView : MonoBehaviour
{
    [SerializeField] FichaView ficha;
    [SerializeField] TMP_Text texto;

    public event Action Clic;

    void Awake()
    {
        ficha.MostrarBocaAbajo(true);
        ficha.Clic += _ => Clic?.Invoke();
    }

    public void Mostrar(int cantidad, bool puedeRobar)
    {
        ficha.gameObject.SetActive(cantidad > 0);
        ficha.Interactiva = puedeRobar;
        texto.text = cantidad > 0 ? $"Pozo: {cantidad}" : "Pozo vacío";
    }
}
