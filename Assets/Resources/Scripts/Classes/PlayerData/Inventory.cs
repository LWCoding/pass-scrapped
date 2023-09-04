using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory {
    
    [SerializeField] private DictStringInt items = new DictStringInt();

    public List<string> GetKeys() {
        return items.Keys.ToList<string>();
    }
    public int GetItemCount(string itemName) {
        return items[itemName];
    }
    public void RemoveItem(string itemName, int amount) {
        items[itemName] -= amount;
        if (items[itemName] <= 0) {
            items.Remove(itemName);
        }
    }
    public void AddItem(string itemName, int amount) {
        if (GetKeys().Contains(itemName)) {
            items[itemName] += amount;
        } else {
            items.Add(itemName, amount);
        }
    }
    public override string ToString() {
        string repr = "[Inventory {";
        foreach (string key in items.Keys) {
            repr += " " + key + "[x" + items[key] + "] ";
        }
        return repr + " }]";
    }

}