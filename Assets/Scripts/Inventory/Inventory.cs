using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static event Action <List<InventoryItem>> OnInventoryChange;

    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();


    private void OnEnable()
    {
        Object.OnObjectCollected += Add;
    }

    private void OnDisable()
    {
        Object.OnObjectCollected -= Add;
    }

    public void LoadData(GameData data)
    {
        inventory = data.inventory;
    }

    public void SaveData(ref GameData data)
    {
        data.inventory = inventory;
    }

    public static void InvokeInventoryChange(List<InventoryItem> items)
    {
        OnInventoryChange?.Invoke(items);
    }

    public void Add(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddToStack();
            Debug.Log($"total stack is now {item.stackSize}");
            OnInventoryChange?.Invoke(inventory);
        }
        else 
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            itemDictionary.Add(itemData, newItem);
            Debug.Log(message: $"Added to the stack for the first time");
            OnInventoryChange?.Invoke(inventory);
        }
    }

    public void Remove(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveFromStack();
            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }
            OnInventoryChange?.Invoke(inventory);
        }
    }
}
