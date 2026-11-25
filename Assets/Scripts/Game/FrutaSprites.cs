using Licuadomino.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Licuadomino/Fruta Sprites", fileName = "FrutaSprites")]
public class FrutaSprites : ScriptableObject
{
    [Tooltip("En el orden del enum Fruta: Plátano, Coco, Limón, Naranja, Mango, Fresa, Sandía, Melón")]
    public Sprite[] frutas = new Sprite[FrutaInfo.Cantidad];
    public Sprite frente;
    public Sprite reverso;

    public Sprite SpriteDe(Fruta f) => frutas[(int)f];
}
