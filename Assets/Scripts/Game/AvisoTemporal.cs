using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class AvisoTemporal : MonoBehaviour
{
    [SerializeField] float segundosVisible = 4.5f;
    [SerializeField] float segundosDesvanecer = 0.5f;

    TMP_Text texto;
    Coroutine rutina;

    void Awake()
    {
        texto = GetComponent<TMP_Text>();
        texto.text = "";
    }

    public void Mostrar(string mensaje, Color color)
    {
        if (rutina != null) StopCoroutine(rutina);
        texto.text = mensaje;
        texto.color = color;
        texto.alpha = 1f;
        rutina = StartCoroutine(Desvanecer());
    }

    IEnumerator Desvanecer()
    {
        yield return new WaitForSeconds(segundosVisible);

        for (float t = 0f; t < segundosDesvanecer; t += Time.deltaTime)
        {
            texto.alpha = 1f - t / segundosDesvanecer;
            yield return null;
        }

        texto.text = "";
        rutina = null;
    }
}