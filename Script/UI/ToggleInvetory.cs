using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInvetory : MonoBehaviour
{
    public GameObject inventoryObj;

    private void OnEnable()
    {
        Chest.OpenChestEvent += OnState;
        Chest.CloseChestEvent += OffState;
    }

    private void OnDisable()
    {
        Chest.OpenChestEvent -= OnState;
        Chest.CloseChestEvent -= OffState;

    }


    private void OnState()
    {
        inventoryObj.SetActive(true);
    }

    private void OffState()
    {
        inventoryObj.SetActive(false);
    }
}
