using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuickLootChest : MonoBehaviour
{
    public GameObject chestInventoryObj;
    public InventoryObject playerInventory;
    public InventoryObject chestInventory;


    [SerializeField]
    private bool isSetChestInventory = false;

    private void Start()
    {
        playerInventory = transform.GetChild(0).GetComponent<DisplayInventoryUI>().inventory;
        
    }

    private void Update()
    {
        if (!chestInventoryObj.activeInHierarchy)
        {
            isSetChestInventory = false;
            chestInventory = null;
            return;
        }
        
        if (isSetChestInventory) return;

        isSetChestInventory = true;
        chestInventory = transform.GetChild(1).GetComponent<DisplayInventoryUI>().inventory;

    }

    public void QuickLootEvent()
    {
        if (playerInventory == null || chestInventory == null) return;


        for(int i = 0; i < playerInventory.inventorySize; i++)
        {     
            if(playerInventory.inventory[i].itemObject == null)
            {
                for (int j = 0; j < chestInventory.inventorySize; j++)
                {
                    if (chestInventory.inventory[j].itemObject != null)
                    {
                        playerInventory.AddItem(chestInventory.inventory[j].itemObject, chestInventory.inventory[j].amount);
                        chestInventory.RemoveItem(j);
                    }
                }
            }
        }
    }
}
