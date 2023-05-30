using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    public bool isPlayerOpenChest;
    public InventoryObject inventory;
    public InventoryObject chestInventory;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            var item = collision.GetComponent<Item>();
            if(item)
            {
                if(inventory.CheckFullInventory() == false)
                {
                    inventory.AddItem(item.item, 1);
                    Destroy(item.gameObject);
                }
            }          
        }

        if(collision.gameObject.tag == "Chest")
        {
            isPlayerOpenChest = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Chest")
        {
            chestInventory = null;
            isPlayerOpenChest = false;
        }
    }

}
