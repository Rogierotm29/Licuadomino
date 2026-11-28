using UnityEngine;

public class MenuInicio : MonoBehaviour
{
    [SerializeField] GameObject panelMenu;
    [SerializeField] GameObject panelReglas;
    [SerializeField] GameObject panelCreditos;
    [SerializeField] GuiaLicuados guia;

    void Start() => Mostrar(panelMenu);

    public void AbrirReglas() => Mostrar(panelReglas);
    public void AbrirCreditos() => Mostrar(panelCreditos);
    public void Volver() => Mostrar(panelMenu);
    public void VerCombinaciones() => guia.Mostrar(null, false);

    void Mostrar(GameObject panel)
    {
        panelMenu.SetActive(panel == panelMenu);
        panelReglas.SetActive(panel == panelReglas);
        panelCreditos.SetActive(panel == panelCreditos);
    }
}