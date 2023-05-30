using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;



[SelectionBase]
public class DisplayInventoryUI : MonoBehaviour
{
    public InventoryObject inventory;
    public CurrentPlayerSlot currentPlayerSlot;
    
    public GameObject Inventory_Slot_Prefabs;
    public Sprite defaultSprite;
    private int currentIDDragEnter = -1;

    Dictionary<GameObject, InventorySlot> itemsDisplayDict;


    public bool isChest;
    private bool firstTimeInitiate = true;


    private void OnEnable()
    {
        InventoryObject.OnInventoryChangedEvent += UpdateAllSlot;
        InventoryObject.OnOneSlotChangedEvent += UpdateOneSlot;


        if (isChest)
        {
            inventory = currentPlayerSlot.GetComponent<PlayerController>().chestInventory;
            if (inventory != null)
            {
                CreateSlots();
            }
        }
    }

    
    private void OnDisable()
    {
        InventoryObject.OnInventoryChangedEvent -= UpdateAllSlot;
        InventoryObject.OnOneSlotChangedEvent -= UpdateOneSlot;
    }


    void Start()
    {
        if (inventory != null && !isChest)
            CreateSlots();
    }


    /// <summary>
    /// Generate Inventory Slot UI
    /// </summary>
    private void CreateSlots()
    {   
        if (!isChest || firstTimeInitiate)
        {
            inventory.InitialEmptyInventory();

            itemsDisplayDict = new Dictionary<GameObject, InventorySlot>();
            firstTimeInitiate = false;
            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                var obj = Instantiate(Inventory_Slot_Prefabs, transform.position, Quaternion.identity, transform);
                itemsDisplayDict.Add(obj, inventory.inventory[i]);
                AddEvent(obj, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, obj));
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, (baseEvent) => OnExit(baseEvent, obj));
                AddEvent(obj, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, obj));
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            }
        }
        else
        {
            if(!firstTimeInitiate)
            {
                if (inventory.inventory.Count == 0)
                    inventory.InitialEmptyInventory();

                itemsDisplayDict = new Dictionary<GameObject, InventorySlot>();
                for (int i = 0; i < inventory.inventory.Count; i++)
                {                 
                    GameObject obj = gameObject.transform.GetChild(i).gameObject;
                    itemsDisplayDict.Add(obj, inventory.inventory[i]);
                }
                UpdateAllSlot();
            }
            
        }
    }



    #region Update Inventory UI
    /// <summary>
    /// Update all inventory slots over time.
    /// </summary>
    public void UpdateAllSlot()
    {
        int index = 0;
        foreach (var slot in itemsDisplayDict)
        {
            if (inventory.inventory[index].itemObject != null)
            {
                slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite = inventory.inventory[index].itemObject.icon;
                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = inventory.inventory[index].amount.ToString();

            }
            else
            {
                slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite = defaultSprite;
                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            index++;
        }
    }

    public void UpdateOneSlot(int id)
    {
        int index = 0;
        foreach (var slot in itemsDisplayDict)
        {
            if (index == id)
            {
                if (inventory.inventory[index].itemObject != null)
                {
                    slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite = inventory.inventory[index].itemObject.icon;
                    slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = inventory.inventory[index].amount.ToString();

                }
                else
                {
                    slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite = defaultSprite;
                    slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
            index++;
        }
    }

    #endregion 


    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }


    public void OnClick(BaseEventData baseEvent, GameObject obj)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnMouseLeftClick(obj);           
        }

        if (pointerEventData.pointerId == -2)   // Mouse Right Event
        {
            OnMouseRightClick(obj);            
        }
    }


    private void FakeItemUIRightClick(GameObject obj)
    {
        InventorySlot slot = inventory.inventory[itemsDisplayDict[obj].id];
        int objAmount = slot.amount;

        int splitValue = objAmount / 2;
        currentPlayerSlot.onHandSlot = new InventorySlot(slot.id, slot.itemObject, splitValue);
        inventory.SetAmount(inventory.inventory[itemsDisplayDict[obj].id].id, objAmount - splitValue);
        currentPlayerSlot.UpdateUIRightClick();

    }


    private void FakeItemUILeftClick(GameObject obj)
    {
        InventorySlot slot = inventory.inventory[itemsDisplayDict[obj].id];
        currentPlayerSlot.onHandSlot = new InventorySlot(slot.id, slot.itemObject, slot.amount);      
        inventory.RemoveItem(inventory.inventory[itemsDisplayDict[obj].id].id);
    }


    #region On Mouse Left Click
    private void OnMouseLeftClick(GameObject obj)
    {
        if (currentPlayerSlot.onHandSlot.itemObject == null)
        {
            if (inventory.inventory[itemsDisplayDict[obj].id].itemObject != null)
            {
                FakeItemUILeftClick(obj);
            }       
        }
        else
        {
            if (inventory.inventory[itemsDisplayDict[obj].id].itemObject == null)
            {
                inventory.SwapItemThroughId(itemsDisplayDict[obj].id, ref currentPlayerSlot.onHandSlot);
                currentPlayerSlot.onHandSlot.Reset();             
            }
            else
            {
                bool isSameItemType = inventory.GetItemObjectById(itemsDisplayDict[obj].id).itemName == currentPlayerSlot.onHandSlot.itemObject.itemName;
                
                if(isSameItemType)
                {
                    if (inventory.inventory[itemsDisplayDict[obj].id].amount < inventory.GetItemObjectById(itemsDisplayDict[obj].id).max_quantity)
                    {
                        int amountAfterSum = inventory.inventory[itemsDisplayDict[obj].id].amount += currentPlayerSlot.onHandSlot.amount;
                        // amount of item exceed max_quantity of this item -> Fake UI display
                        if (amountAfterSum > inventory.GetItemObjectById(itemsDisplayDict[obj].id).max_quantity)
                        {
                            inventory.SetAmount(itemsDisplayDict[obj].id, inventory.GetItemObjectById(itemsDisplayDict[obj].id).max_quantity);

                            currentPlayerSlot.onHandSlot.amount = amountAfterSum - inventory.inventory[itemsDisplayDict[obj].id].amount;
                        }
                        // amount of item don't exceed max_quantity of this item.
                        else
                        {
                            inventory.SetAmount(itemsDisplayDict[obj].id, amountAfterSum);
                            currentPlayerSlot.onHandSlot.Reset();
                        }
                    }
                }
                else
                {                    
                    inventory.SwapItemThroughId(itemsDisplayDict[obj].id, ref currentPlayerSlot.onHandSlot);
                }
                
            }  
        }
        currentPlayerSlot.UpdateUILeftClick();
    }
    #endregion

    private void OnMouseRightClick(GameObject obj)
    {
        if(currentPlayerSlot.onHandSlot.itemObject == null)
        {
            if (inventory.inventory[itemsDisplayDict[obj].id].itemObject != null)
            {
                // if this slot has only 1 item then treat that as LeftClick event.
                if (inventory.inventory[itemsDisplayDict[obj].id].amount > 1)
                {
                    FakeItemUIRightClick(obj);
                }
                else
                {
                    FakeItemUILeftClick(obj);
                }
            }
        }
        else
        {
            if (inventory.inventory[itemsDisplayDict[obj].id].itemObject == null)
            {
                inventory.AddItemDontStack(currentPlayerSlot.onHandSlot.itemObject, 1, itemsDisplayDict[obj].id);
                currentPlayerSlot.onHandSlot.amount -= 1;
                
                currentPlayerSlot.UpdateUIRightClick();

                if (currentPlayerSlot.onHandSlot.amount <= 0)
                {
                    currentPlayerSlot.onHandSlot.Reset();
                    return;
                }
                
            }
            else
            {
                bool isSameItemType = inventory.GetItemObjectById(itemsDisplayDict[obj].id).itemName == currentPlayerSlot.onHandSlot.itemObject.itemName;
                if(isSameItemType)
                {
                    if(inventory.inventory[itemsDisplayDict[obj].id].amount < inventory.GetItemObjectById(itemsDisplayDict[obj].id).max_quantity)
                    {
                        inventory.AddAmountItem(itemsDisplayDict[obj].id, 1);
                        currentPlayerSlot.onHandSlot.amount -= 1;
                    }
                    
                    if (currentPlayerSlot.onHandSlot.amount <= 0)
                    {
                        currentPlayerSlot.onHandSlot.Reset();
                        return;
                    }
                    currentPlayerSlot.UpdateUIRightClick();
                }
                else
                {
                    inventory.SwapItemThroughId(itemsDisplayDict[obj].id, ref currentPlayerSlot.onHandSlot);
                }

            }
        }
    }

    #region Drag Event
    public void OnEnter(GameObject obj)
    {

        /*if (currentPlayerSlot.onHandSlot.itemObject != null)
        {
            Debug.Log(itemsDisplayDict[obj].id);
        }*/



        if (currentPlayerSlot.onHandSlot.itemObject == null) return;
        currentIDDragEnter = itemsDisplayDict[obj].id;
    }

    public void OnExit(BaseEventData baseEvent, GameObject obj) { }

    public void OnBeginDrag(BaseEventData baseEvent, GameObject obj)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;

        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            if (currentPlayerSlot.onHandSlot.itemObject == null)
            {
                if (inventory.inventory[itemsDisplayDict[obj].id].itemObject != null)
                {
                    FakeItemUILeftClick(obj);
                }
            }
        }
    }


    public void OnDrag(GameObject obj) 
    {
        
    }

    public void OnEndDrag(GameObject obj)
    {
        if (currentPlayerSlot.onHandSlot.itemObject == null)
        {
            if (inventory.inventory[currentIDDragEnter].itemObject != null)
            {
                FakeItemUILeftClick(obj);
            }
        }
        else
        {
            if (inventory.inventory[currentIDDragEnter].itemObject == null)
            {
                inventory.SwapItemThroughId(currentIDDragEnter, ref currentPlayerSlot.onHandSlot);
                currentPlayerSlot.onHandSlot.Reset();
            }
            else
            {
                bool isSameItemType = inventory.GetItemObjectById(currentIDDragEnter).itemName == currentPlayerSlot.onHandSlot.itemObject.itemName;

                if (isSameItemType)
                {
                    if (inventory.inventory[currentIDDragEnter].amount < inventory.GetItemObjectById(currentIDDragEnter).max_quantity)
                    {
                        int amountAfterSum = inventory.inventory[currentIDDragEnter].amount += currentPlayerSlot.onHandSlot.amount;
                        // amount of item exceed max_quantity of this item -> Fake UI display
                        if (amountAfterSum > inventory.GetItemObjectById(currentIDDragEnter).max_quantity)
                        {
                            inventory.SetAmount(currentIDDragEnter, inventory.GetItemObjectById(currentIDDragEnter).max_quantity);

                            currentPlayerSlot.onHandSlot.amount = amountAfterSum - inventory.inventory[currentIDDragEnter].amount;
                        }
                        // amount of item don't exceed max_quantity of this item.
                        else
                        {
                            inventory.SetAmount(currentIDDragEnter, amountAfterSum);
                            currentPlayerSlot.onHandSlot.Reset();
                        }
                    }
                }
                else
                {
                    inventory.SwapItemThroughId(currentIDDragEnter, ref currentPlayerSlot.onHandSlot);
                }

            }
        }
        currentPlayerSlot.UpdateUILeftClick();
    }

    #endregion

    private void OnApplicationQuit()
    {
        inventory?.inventory.Clear();
    }

}



