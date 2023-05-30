using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Sword Object", order = 51)]
public class SwordObject : ItemObject
{
    public int damage;
    public int numOfUseRemaining;

    public void Awake()
    {
        itemType = ItemType.Weapon_Sword;
    }
}
