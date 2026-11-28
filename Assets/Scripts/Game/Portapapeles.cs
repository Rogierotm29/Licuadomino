using System.Runtime.InteropServices;
using UnityEngine;

public static class Portapapeles
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void CopiarAlPortapapeles(string texto);

    public static void Copiar(string texto) => CopiarAlPortapapeles(texto);
#else
    public static void Copiar(string texto) => GUIUtility.systemCopyBuffer = texto;
#endif
}