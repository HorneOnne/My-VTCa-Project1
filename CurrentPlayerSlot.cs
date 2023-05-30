using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrentPlayerSlot : MonoBehaviour
{
    public Sprite defaultSprite;
    OnMouseClickItem onMouseClickItem = new OnMouseClickItem();
    public InventorySlot onHandSlot;
    public Transform UIparent;


    private void OnEnable()
    {
        InventoryObject.OnInventoryChangedEvent += UpdateUILeftClick;
        InventoryObject.OnInventoryChangedEvent += UpdateUIRightClick;

        InventorySlot.OnResetEvent += OnResetOnHandSlot;
    }

    private void OnDisable()
    {
        InventoryObject.OnInventoryChangedEvent -= UpdateUILeftClick;
        InventoryObject.OnInventoryChangedEvent -= UpdateUIRightClick;

        InventorySlot.OnResetEvent -= OnResetOnHandSlot;
    }


    private void Update()
    {
        if(onHandSlot.itemObject != null && onMouseClickItem.currentItem != null)
        {
            onMouseClickItem.currentItem.transform.position = Input.mousePosition;

        }
    }

    private void OnResetOnHandSlot()
    {
        Destroy(onMouseClickItem.currentItem);
    }

    public void UpdateUILeftClick()
    {
        if (onHandSlot.itemObject != null)
        {
            if(onMouseClickItem.currentItem != null || onHandSlot.amount <= 0)
                Destroy(onMouseClickItem.currentItem);

            onMouseClickItem.currentItem = new GameObject();
            onMouseClickItem.currentItemID = onHandSlot.id;
            var rt = onMouseClickItem.currentItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            var img = onMouseClickItem.currentItem.AddComponent<Image>();
            img.sprite = onHandSlot.itemObject.icon;
            img.raycastTarget = false;
            onMouseClickItem.currentItem.transform.SetParent(UIparent.parent);


            onMouseClickItem.amountText = new GameObject().AddComponent<TextMeshProUGUI>();
            onMouseClickItem.amountText.text = onHandSlot.amount.ToString();
            onMouseClickItem.amountText.fontSize = 25f;
            onMouseClickItem.amountText.transform.localPosition = new Vector3(10.5f, -32.2f, 0);
            onMouseClickItem.amountText.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 40);
            onMouseClickItem.amountText.transform.SetParent(onMouseClickItem.currentItem.transform);
        }
        else
        {
            Destroy(onMouseClickItem.currentItem);
        }
    }


    public void UpdateUIRightClick()
    {
        if (onHandSlot.itemObject != null)
        {
            if (onMouseClickItem.currentItem != null || onHandSlot.amount <= 0)
            {
                Destroy(onMouseClickItem.currentItem);
            }
                

            // Falke UI Sprite Item
            onMouseClickItem.currentItem = new GameObject();
            onMouseClickItem.currentItemID = onHandSlot.id;
            var rt = onMouseClickItem.currentItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            var img = onMouseClickItem.currentItem.AddComponent<Image>();
            img.sprite = onHandSlot.itemObject.icon;
            img.raycastTarget = false;
            onMouseClickItem.currentItem.transform.SetParent(UIparent.parent);


            // Fake UI Text amount of item.
            onMouseClickItem.amountText = new GameObject().AddComponent<TextMeshProUGUI>();
            onMouseClickItem.amountText.text = onHandSlot.amount.ToString();
            onMouseClickItem.amountText.fontSize = 25f;
            onMouseClickItem.amountText.transform.localPosition = new Vector3(10.5f, -32.2f, 0);
            onMouseClickItem.amountText.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 40);
            onMouseClickItem.amountText.transform.SetParent(onMouseClickItem.currentItem.transform);
        }
        else
        {
            Destroy(onMouseClickItem.currentItem);
        }
    }
}

public class OnMouseClickItem : Object
{
    public GameObject currentItem;
    public TextMeshProUGUI amountText;
    public int currentItemID = -1;

    public OnMouseClickItem() { }
    public OnMouseClickItem(OnMouseClickItem other)
    {
        this.currentItem = other.currentItem;
        this.amountText = other.amountText;
        this.currentItemID = other.currentItemID;
    }

    public void DestroyCurrentMouseItem()
    {
        Destroy(currentItem);
    }

    public void OnChangeMouseItemUI(InventorySlot slot)
    {
        if (currentItem != null)
        {
            currentItem.GetComponent<Image>().sprite = slot.itemObject.icon;
            amountText.GetComponent<TextMeshProUGUI>().text = slot.amount.ToString();
        }
    }
}
