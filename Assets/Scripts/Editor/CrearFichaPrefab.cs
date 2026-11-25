using Licuadomino.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public static class CrearFichaPrefab
{
    const string CarpetaArte = "Assets/Art";
    const string RutaSprites = "Assets/Data/FrutaSprites.asset";
    const string RutaPrefab = "Assets/Prefabs/Ficha.prefab";
    const string RutaFrente = CarpetaArte + "/ficha_frente.png";
    const string RutaReverso = CarpetaArte + "/ficha_reverso.png";

    // Mismo orden que el enum Fruta.
    static readonly string[] archivos =
        { "Platano", "Coco", "Limon", "Naranja", "Mango", "Fresa", "Sandia", "Melon" };

    static readonly Color colorMarco = new Color(1f, 0.78f, 0.15f);
    static readonly Color colorDivisor = new Color32(0x3A, 0x1A, 0x33, 0xFF);

    [MenuItem("Licuadomino/Crear prefab de ficha")]
    public static void Crear()
    {
        if (!ConfigurarSprite(RutaFrente, true)) return;
        if (!ConfigurarSprite(RutaReverso, false)) return;
        for (int i = 0; i < archivos.Length; i++)
            if (!ConfigurarSprite($"{CarpetaArte}/fruta_{archivos[i]}.png", false)) return;

        FrutaSprites sprites = CrearOActualizarSprites();
        if (sprites == null) return;

        AsegurarCarpeta("Assets/Prefabs");
        GameObject go = Construir(sprites);
        PrefabUtility.SaveAsPrefabAsset(go, RutaPrefab);
        Object.DestroyImmediate(go);
        AssetDatabase.SaveAssets();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(RutaPrefab);
        Debug.Log("[Licuadomino] Listo: " + RutaPrefab + " y " + RutaSprites);
    }

    static bool ConfigurarSprite(string ruta, bool nueveSlice)
    {
        var ti = AssetImporter.GetAtPath(ruta) as TextureImporter;
        if (ti == null)
        {
            Debug.LogError("[Licuadomino] No encontré " + ruta + ". ¿Está en Assets/Art con ese nombre exacto?");
            return false;
        }

        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;

        var s = new TextureImporterSettings();
        ti.ReadTextureSettings(s);
        s.spriteMeshType = SpriteMeshType.FullRect;
        s.spriteBorder = nueveSlice ? new Vector4(40, 40, 40, 40) : Vector4.zero;
        ti.SetTextureSettings(s);

        ti.SaveAndReimport();
        return true;
    }

    static FrutaSprites CrearOActualizarSprites()
    {
        AsegurarCarpeta("Assets/Data");
        var asset = AssetDatabase.LoadAssetAtPath<FrutaSprites>(RutaSprites);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<FrutaSprites>();
            AssetDatabase.CreateAsset(asset, RutaSprites);
        }

        asset.frutas = new Sprite[FrutaInfo.Cantidad];
        for (int i = 0; i < archivos.Length; i++)
        {
            asset.frutas[i] = AssetDatabase.LoadAssetAtPath<Sprite>($"{CarpetaArte}/fruta_{archivos[i]}.png");
            if (asset.frutas[i] == null)
            {
                Debug.LogError($"[Licuadomino] No pude cargar fruta_{archivos[i]}.png como Sprite.");
                return null;
            }
        }
        asset.frente = AssetDatabase.LoadAssetAtPath<Sprite>(RutaFrente);
        asset.reverso = AssetDatabase.LoadAssetAtPath<Sprite>(RutaReverso);
        if (asset.frente == null || asset.reverso == null)
        {
            Debug.LogError("[Licuadomino] No pude cargar ficha_frente o ficha_reverso como Sprite.");
            return null;
        }

        EditorUtility.SetDirty(asset);
        return asset;
    }

    static GameObject Construir(FrutaSprites sprites)
    {
        var root = new GameObject("Ficha", typeof(RectTransform), typeof(CanvasGroup));
        ((RectTransform)root.transform).sizeDelta = new Vector2(FichaView.Ancho, FichaView.Largo);

        Image marco = Hijo(root, "Marco", sprites.frente, colorMarco, true, 7f, false);
        marco.type = Image.Type.Sliced;
        marco.enabled = false;

        Image fondo = Hijo(root, "Fondo", sprites.frente, Color.white, true, 0f, true);
        fondo.type = Image.Type.Sliced;

        Image divisor = Hijo(root, "Divisor", null, colorDivisor, false, 0f, false);

        Image a = Hijo(root, "FrutaA", null, Color.white, false, 0f, false);
        a.preserveAspect = true;
        Image b = Hijo(root, "FrutaB", null, Color.white, false, 0f, false);
        b.preserveAspect = true;

        Image reverso = Hijo(root, "Reverso", sprites.reverso, Color.white, false, 0f, true);
        reverso.enabled = false;

        var view = root.AddComponent<FichaView>();
        var so = new SerializedObject(view);
        so.FindProperty("sprites").objectReferenceValue = sprites;
        so.FindProperty("marco").objectReferenceValue = marco;
        so.FindProperty("fondo").objectReferenceValue = fondo;
        so.FindProperty("divisor").objectReferenceValue = divisor;
        so.FindProperty("frutaA").objectReferenceValue = a;
        so.FindProperty("frutaB").objectReferenceValue = b;
        so.FindProperty("reverso").objectReferenceValue = reverso;
        so.FindProperty("grupo").objectReferenceValue = root.GetComponent<CanvasGroup>();
        so.ApplyModifiedPropertiesWithoutUndo();

        view.Mostrar(GeneradorFichas.PorId(1));
        return root;
    }

    static Image Hijo(GameObject padre, string nombre, Sprite sprite, Color color,
                      bool estirar, float sobresale, bool recibeClics)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(padre.transform, false);

        if (estirar)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(-sobresale, -sobresale);
            rt.offsetMax = new Vector2(sobresale, sobresale);
        }

        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = recibeClics;
        return img;
    }

    static void AsegurarCarpeta(string ruta)
    {
        if (AssetDatabase.IsValidFolder(ruta)) return;
        int i = ruta.LastIndexOf('/');
        AssetDatabase.CreateFolder(ruta.Substring(0, i), ruta.Substring(i + 1));
    }
}
