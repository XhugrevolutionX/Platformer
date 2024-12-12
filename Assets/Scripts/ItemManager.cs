using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    
    [SerializeField] private UnityEvent allItemsPickedUp;

    private List<Item> _itemsToPick = new List<Item>();
    void Start()
    {
        LoadItems();
    }
    public void LoadItems()
    {
        Item[] itemsArray = GetComponentsInChildren<Item>();
        _itemsToPick = new List<Item>(itemsArray);

        foreach (Item item in _itemsToPick)
        {
            item.Activate();
            item.OnPicked += RemoveItem;
        }
    }

    private void RemoveItem(Item itemToRemove)
    {
        Debug.Log("Removing item");
        itemToRemove.OnPicked -= RemoveItem;
        _itemsToPick.Remove(itemToRemove);
        
        if (_itemsToPick.Count == 0)
        {
            allItemsPickedUp?.Invoke();
        }
    }
}
