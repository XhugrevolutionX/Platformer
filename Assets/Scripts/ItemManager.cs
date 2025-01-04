using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    
    [SerializeField] private UnityEvent allItemsPickedUp;
    [SerializeField] private GameObject diamondCounter;
    private int _diamondPickedUp;
    
    private List<Item> _itemsToPick = new List<Item>();
    void Start()
    {
        _diamondPickedUp = 0;
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
        _diamondPickedUp++;
        
        diamondCounter.GetComponent<Text>().text = "X " + _diamondPickedUp;
        
        if (_itemsToPick.Count == 0)
        {
            allItemsPickedUp?.Invoke();
        }
    }
}
