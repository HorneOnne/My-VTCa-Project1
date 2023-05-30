using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    public delegate void OpenChestDelegate();
    public static event OpenChestDelegate OpenChestEvent;

    public delegate void CloseChestDelegate();
    public static event CloseChestDelegate CloseChestEvent;




    private CircleCollider2D cl;
    public InventoryObject chestInventory;
    public bool isOpen;
    public Animator anim;


    private void Start()
    {
        cl = GetComponent<CircleCollider2D>();
        cl.isTrigger = true;
        cl.enabled = false;

        //chestInventory = new InventoryObject(14);  
        chestInventory = ScriptableObject.CreateInstance<InventoryObject>();
        chestInventory.inventorySize = 14;
    }

    public void OnOpenState()
    {
        anim.SetBool("open", true);
        isOpen = true;
        OpenChestEvent?.Invoke();        
    }

    public void OnCloseState()
    {
        anim.SetBool("open", false);
        isOpen = false;
        CloseChestEvent?.Invoke();
        cl.enabled = false;
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(isOpen)
            {
                OnCloseState();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cl.enabled = true;
        var player = FindObjectOfType<PlayerController>();       
        var distance = (this.transform.position - player.transform.position).magnitude;
       
        if(distance < 2.5f)
        {
            if (isOpen)
            {
                OnCloseState();
                player.isPlayerOpenChest = false;              
            }
            else
            {
                if(!player.isPlayerOpenChest)
                {
                    player.chestInventory = this.chestInventory;
                    OnOpenState();
                }
            }
        }
        
    }
}
