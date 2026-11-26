using System;
using Licuadomino.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotonLado : MonoBehaviour
{
    [SerializeField] Button boton;
    [SerializeField] Image fruta;
    [SerializeField] TMP_Text texto;
    [SerializeField] FrutaSprites sprites;

    public event Action Clic;

    void Awake() => boton.onClick.AddListener(() => Clic?.Invoke());

    public void Ocultar() => gameObject.SetActive(false);

    /// <param name="extremo"></param>
    public void Mostrar(Fruta? extremo, string etiqueta)
    {
        gameObject.SetActive(true);
        fruta.enabled = extremo.HasValue;
        if (extremo.HasValue) fruta.sprite = sprites.SpriteDe(extremo.Value);
        texto.text = etiqueta;
    }
}
