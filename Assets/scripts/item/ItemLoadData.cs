

using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data")]
public class itemLoadData : ScriptableObject
{
    public string id;
    public string displayName;
    public string description;
    public int maxStackNum;
    public Sprite icon;
    public ItemTypes type;
    public GameObject preFab;
}