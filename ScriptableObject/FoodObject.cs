using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "ScriptableObject/Item/Food Object", order = 51)]
public class FoodObject : ItemObject
{
    public int restoreHealthValue;
    public int restoreSatietyValue;
    public void Awake()
    {
        itemType = ItemType.Food;
    }
}
