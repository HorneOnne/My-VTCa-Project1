using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ItemObject : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int max_quantity;

}

public enum ItemType
{
    Empty,
    Food,
    Armor,
    Tool,
    Weapon_Gun,
    Weapon_Sword
}
