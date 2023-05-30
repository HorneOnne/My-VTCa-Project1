using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "ScriptableObject/Inventory", order = 51)]
public class InventoryObject : ScriptableObject
{
    public delegate void OnInventoryChanged();
    public static event OnInventoryChanged OnInventoryChangedEvent;

    public delegate void OnOneSlotChanged(int id);
    public static event OnOneSlotChanged OnOneSlotChangedEvent;

    public int inventorySize;
    public List<InventorySlot> inventory = new List<InventorySlot>();


    public InventoryObject() { }

    public InventoryObject(int size)
    {
        this.inventorySize = size;
    }


   

    public void InitialEmptyInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new InventorySlot(i, null, 0));
        }
    }


    public bool CheckFullInventory()
    {
        int count = 0;
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].itemObject != null)
                if (inventory[i].isFull)
                    count++;
        }
        return count == inventorySize;
    }

    public void AddItem(ItemObject _itemObject, int _amount)
    {
        // Add amount of item when its exist and don't exceed the max_quantity.
        UpdateOldSlot(_itemObject, _amount, out bool hasItem);
        
        // Create new item in inventory
        if (!hasItem)
        {
            AddNewSlot(_itemObject, _amount);
        }

        
        OnInventoryChangedEvent?.Invoke();
    }

    public void AddItemDontStack(ItemObject _itemObject, int _amount, int id)
    {
        inventory[id] = new InventorySlot(id, _itemObject, _amount);
        OnInventoryChangedEvent?.Invoke();
    }

    /// <summary>
    /// Add new item into inventory.
    /// </summary>
    /// <param name="_itemObject"></param>
    /// <param name="_amount"></param>
    private bool AddNewSlot(ItemObject _itemObject, int _amount)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory[i].itemObject)
            {
                inventory[i] = new InventorySlot(i, _itemObject, _amount);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Update amount of item and out bool result for check whether the inventory has this item.
    /// </summary>
    /// <param name="_itemObject"></param>
    /// <param name="_amount"></param>
    /// <param name="hasItem"></param>
    public void UpdateOldSlot(ItemObject _itemObject, int _amount, out bool hasItem)
    {
        hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemObject == _itemObject)
            {
                if (inventory[i].amount < _itemObject.max_quantity)
                {
                    int surplus = inventory[i].AddItem(_amount);
                    if (surplus > 0)
                        AddNewSlot(inventory[i].itemObject, surplus);

                    hasItem = true;
                    break;
                }
            }
        }



        OnInventoryChangedEvent?.Invoke();
    }


    public void SwapItem(int oldID, int newID)
    {
        var temp = inventory[oldID];
        inventory[oldID] = inventory[newID];
        inventory[newID] = temp;

        OnInventoryChangedEvent?.Invoke();
    }

    public void SwapItemThroughId(int id, ref InventorySlot itemToSwap)
    {
        itemToSwap.id = id;
        var temp = new InventorySlot(id, inventory[id].itemObject, inventory[id].amount);
        inventory[id] = itemToSwap;
        itemToSwap = temp;
        
        OnInventoryChangedEvent?.Invoke();
    }


    public void RemoveItem(int id)
    {
        this.inventory[id].itemObject = null;
        this.inventory[id].amount = 0;

        OnInventoryChangedEvent?.Invoke();
    }

    public void AddAmountItem(int id, int amount)
    {
        inventory[id].AddItem(amount);
        OnOneSlotChangedEvent?.Invoke(id);
    }


    public void SubstractAmountItem(int id, int _amount)
    {
        bool result = inventory[id].SubstractItem(_amount);

        if (!result)
            throw new Exception();


        OnInventoryChangedEvent?.Invoke();

    }

    public ItemObject GetItemObjectById(int id)
    {
        return inventory[id].itemObject;
    }

    public void SetAmount(int id, int value)
    {
        inventory[id].SetAmount(value);
        OnOneSlotChangedEvent?.Invoke(id);
    }

    public void OnOneSlotChangedTrigger(int id)
    {
        OnOneSlotChangedEvent?.Invoke(id);
    }
}


[System.Serializable]
public class InventorySlot
{
    public delegate void OnReset();
    public static event OnReset OnResetEvent;


    public int id;
    public ItemObject itemObject;
    public int amount;
    public bool isFull;

    public InventorySlot(int id, ItemObject _itemObject, int _amount)
    {
        this.id = id;
        this.itemObject = _itemObject;
        this.amount = _amount;
        if (_itemObject != null)
            this.isFull = this.amount == itemObject.max_quantity;
        else
            this.isFull = false;
    }

    private void CheckFullSlot()
    {
        isFull = this.amount == itemObject.max_quantity;
    }

    /// <summary>
    /// Add the amount of item when it exist in inventory.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int AddItem(int value)
    {
        int surplus = 0;
        if (this.amount + value <= itemObject.max_quantity)
        {
            this.amount += value;
            CheckFullSlot();
            return 0;
        }
        else
        {
            surplus = this.amount + value - itemObject.max_quantity;
            this.amount = itemObject.max_quantity;
            return surplus;
        }
    }

    public bool SubstractItem(int value)
    {
        if (this.amount - value >= 0)
        {
            this.amount -= value;
            CheckFullSlot();
            return true;
        }
        return false;
    }

    public void SetAmount(int value)
    {
        this.amount = value;
        CheckFullSlot();

        if (value < 0 || value > itemObject.max_quantity)
            throw new Exception("amount of item exceed the maximum quantity.");
    }

    public void UpdateSlot(int _id, ItemObject _item, int _amount)
    {
        this.id = _id;
        this.itemObject = _item;
        this.amount = _amount;
        CheckFullSlot();
    }

    public void Reset()
    {
        id = -1;
        itemObject = null;
        amount = -1;
        isFull = false;
        OnResetEvent?.Invoke();
    }   
}

