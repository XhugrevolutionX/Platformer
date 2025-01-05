using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    
    [SerializeField] private UnityEvent allDiamondsPickedUp;
    [SerializeField] private GameObject diamondsCounter;
    [SerializeField] private GameObject cherriesCounter;
    private int _diamondsPickedUp;
    private int _cherriesPickedUp;
    
    private List<Item> _diamondsToPick = new List<Item>();
    private List<Item> _cherriesToPick = new List<Item>();
    void Start()
    {
        _diamondsPickedUp = 0;
        _cherriesPickedUp = 0;
        LoadItems();
    }
    private void LoadItems()
    {
        Item[] itemsArray = GetComponentsInChildren<Item>();

        foreach (Item item in itemsArray)
        {
            if (item.CompareTag("Diamond"))
            {
                _diamondsToPick.Add(item);
            }
            else if (item.CompareTag("Cherry"))
            {
                _cherriesToPick.Add(item);
            }
        }

        foreach (Item item in _diamondsToPick)
        {
            item.Activate();
            item.OnPicked += RemoveItem;
        }
        foreach (Item item in _cherriesToPick)
        {
            item.Activate();
            item.OnPicked += RemoveItem;
        }
    }

    private void RemoveItem(Item itemToRemove)
    {
        Debug.Log("Removing item");
        itemToRemove.OnPicked -= RemoveItem;
        
        if (itemToRemove.CompareTag("Diamond"))
        {
            _diamondsToPick.Remove(itemToRemove);
            _diamondsPickedUp++;
            diamondsCounter.GetComponent<Text>().text = " X " + _diamondsPickedUp;
        }
        else if (itemToRemove.CompareTag("Cherry"))
        {
            _cherriesToPick.Remove(itemToRemove);
            _cherriesPickedUp++;
            cherriesCounter.GetComponent<Text>().text = " X " + _cherriesPickedUp;
        }
        
        if (_diamondsToPick.Count == 0)
        {
            allDiamondsPickedUp?.Invoke();
        }
    }
}
