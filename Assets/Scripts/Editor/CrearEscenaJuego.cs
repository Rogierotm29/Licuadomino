using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public static class CrearEscenaJuego
{
    const string RutaPrefab = "Assets/Prefabs/Ficha.prefab";
    const string RutaSprites = "Assets/Data/FrutaSprites.asset";
    const string RutaFondo = "Assets/Art/fondo_patron.png";

    static readonly Color crema = new Color32(0xFF, 0xF4, 0xE0, 0xFF);
    static readonly Color oscuro = new Color32(0x33, 0x33, 0x33, 0xFF);
    static readonly Color verde = new Color32(0x4C, 0xAF, 0x50, 0xFF);
    static readonly Color gris = new Color32(0x9E, 0x9E, 0x9E, 0xFF);
    static readonly Color panel = new Color(1f, 1f, 1f, 0.9f);

    [MenuItem("Licuadomino/Crear UI de juego en esta escena")]
    public static void Crear()
    {
        var prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>(RutaPrefab);
        var sprites = AssetDatabase.LoadAssetAtPath<FrutaSprites>(RutaSprites);
        if (prefabGo == null || sprites == null)
        {
            Debug.LogError("[Licuadomino] Falta Ficha.prefab o FrutaSprites. Corre primero Licuadomino → Crear prefab de ficha.");
            return;
        }
        FichaView prefab = prefabGo.GetComponent<FichaView>();

        AsegurarEventSystem();

        var canvasGo = new GameObject("CanvasJuego", typeof(RectTransform), typeof(Canvas),
                                      typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGo.layer = LayerMask.NameToLayer("UI");
        canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        Transform c = canvasGo.transform;

        var fondo = NuevoUI("Fondo", c);
        Estirar(fondo, 0, 0, 0, 0);
        var imgFondo = fondo.AddComponent<Image>();
        var spFondo = AssetDatabase.LoadAssetAtPath<Sprite>(RutaFondo);
        if (spFondo != null)
        {
            imgFondo.sprite = spFondo;
            imgFondo.type = Image.Type.Tiled;
            imgFondo.color = Color.white;
        }
        else imgFondo.color = crema;
        imgFondo.raycastTarget = false;

        var mesaGo = NuevoUI("Mesa", c);
        Estirar(mesaGo, 230, 230, 240, 330);
        var mesa = mesaGo.AddComponent<MesaView>();
        Asignar(mesa, "prefabFicha", prefab);

        var rivGo = NuevoUI("Rivales", c);
        Colocar(rivGo, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(-600, 150));
        var rivales = rivGo.AddComponent<RivalesView>();
        Asignar(rivales, "prefabFicha", prefab);

        TMP_Text turno = Texto("TextoTurno", c, "Turno", 38, oscuro, true);
        Colocar(turno.gameObject, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -185), new Vector2(1200, 56));

        TMP_Text mensaje = Texto("TextoMensaje", c, "", 30, oscuro, false);
        var aviso = mensaje.gameObject.AddComponent<AvisoTemporal>();
        Colocar(mensaje.gameObject, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 262), new Vector2(1400, 60));

        var manoGo = NuevoUI("Mano", c);
        Colocar(manoGo, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(-640, 220));
        var mano = manoGo.AddComponent<ManoView>();
        Asignar(mano, "prefabFicha", prefab);

        var pozoGo = NuevoUI("Pozo", c);
        Colocar(pozoGo, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 110), new Vector2(220, 190));
        var fichaPozoGo = (GameObject)PrefabUtility.InstantiatePrefab(prefabGo, pozoGo.transform);
        fichaPozoGo.name = "FichaPozo";
        ((RectTransform)fichaPozoGo.transform).anchoredPosition = new Vector2(0, 25);
        TMP_Text textoPozo = Texto("TextoPozo", pozoGo.transform, "Pozo", 28, oscuro, true);
        Colocar(textoPozo.gameObject, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(220, 40));
        var pozo = pozoGo.AddComponent<PozoView>();
        Asignar(pozo, "ficha", fichaPozoGo.GetComponent<FichaView>());
        Asignar(pozo, "texto", textoPozo);

        Button pasar = Boton("BotonPasar", c, "Pasar", gris);
        Colocar(pasar.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 30), new Vector2(220, 64));

        Button nueva = Boton("BotonNueva", c, "Nueva partida", verde);
        Colocar(nueva.gameObject, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20), new Vector2(240, 60));

        BotonLado izq = CrearBotonLado("BotonIzquierda", c, sprites, "< Aquí");
        Colocar(izq.gameObject, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, 40), new Vector2(190, 230));

        BotonLado der = CrearBotonLado("BotonDerecha", c, sprites, "Aquí >");
        Colocar(der.gameObject, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-20, 40), new Vector2(190, 230));

        var pantalla = canvasGo.AddComponent<PantallaJuego>();
        Asignar(pantalla, "mesa", mesa);
        Asignar(pantalla, "mano", mano);
        Asignar(pantalla, "rivales", rivales);
        Asignar(pantalla, "pozo", pozo);
        Asignar(pantalla, "botonIzquierda", izq);
        Asignar(pantalla, "botonDerecha", der);
        Asignar(pantalla, "botonPasar", pasar);
        Asignar(pantalla, "textoTurno", turno);
        Asignar(pantalla, "aviso", aviso);

        var juegoGo = new GameObject("JuegoLocal");
        var juego = juegoGo.AddComponent<JuegoLocal>();
        Asignar(juego, "pantalla", pantalla);
        Asignar(juego, "botonNueva", nueva);

        Undo.RegisterCreatedObjectUndo(canvasGo, "Crear UI de juego");
        Undo.RegisterCreatedObjectUndo(juegoGo, "Crear UI de juego");
        EditorSceneManager.MarkSceneDirty(canvasGo.scene);
        Selection.activeGameObject = canvasGo;
        Debug.Log("[Licuadomino] UI de juego creada. Guarda la escena (Ctrl+S) y dale Play.");
    }


    static void AsegurarEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem", typeof(EventSystem));
        var tipoInputSystem = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (tipoInputSystem != null) go.AddComponent(tipoInputSystem);
        else go.AddComponent<StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(go, "Crear EventSystem");
    }

    static GameObject NuevoUI(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(padre, false);
        return go;
    }

    static void Colocar(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void Estirar(GameObject go, float izquierda, float derecha, float arriba, float abajo)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(izquierda, abajo);
        rt.offsetMax = new Vector2(-derecha, -arriba);
    }

    static TMP_Text Texto(string nombre, Transform padre, string texto, float tam, Color color, bool negritas)
    {
        var go = NuevoUI(nombre, padre);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto;
        t.fontSize = tam;
        t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        t.fontStyle = negritas ? FontStyles.Bold : FontStyles.Normal;
        t.raycastTarget = false;
        return t;
    }

    static Sprite SpriteUI() => AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

    static Button Boton(string nombre, Transform padre, string texto, Color color)
    {
        var go = NuevoUI(nombre, padre);
        var img = go.AddComponent<Image>();
        img.sprite = SpriteUI();
        img.type = Image.Type.Sliced;
        img.color = color;
        var b = go.AddComponent<Button>();

        TMP_Text t = Texto("Texto", go.transform, texto, 30, Color.white, true);
        Estirar(t.gameObject, 0, 0, 0, 0);
        return b;
    }

    static BotonLado CrearBotonLado(string nombre, Transform padre, FrutaSprites sprites, string texto)
    {
        var go = NuevoUI(nombre, padre);
        var img = go.AddComponent<Image>();
        img.sprite = SpriteUI();
        img.type = Image.Type.Sliced;
        img.color = panel;
        var boton = go.AddComponent<Button>();

        var frutaGo = NuevoUI("Fruta", go.transform);
        Colocar(frutaGo, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 25), new Vector2(110, 110));
        var fruta = frutaGo.AddComponent<Image>();
        fruta.preserveAspect = true;
        fruta.raycastTarget = false;

        TMP_Text t = Texto("Texto", go.transform, texto, 28, oscuro, true);
        Colocar(t.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -80), new Vector2(180, 50));

        var bl = go.AddComponent<BotonLado>();
        Asignar(bl, "boton", boton);
        Asignar(bl, "fruta", fruta);
        Asignar(bl, "texto", t);
        Asignar(bl, "sprites", sprites);
        return bl;
    }

    static void Asignar(Object destino, string campo, Object valor)
    {
        var so = new SerializedObject(destino);
        SerializedProperty p = so.FindProperty(campo);
        if (p == null)
        {
            Debug.LogError($"[Licuadomino] {destino.GetType().Name} no tiene el campo '{campo}'.");
            return;
        }
        p.objectReferenceValue = valor;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
