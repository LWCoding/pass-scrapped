using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Consumable, Damageable, Weapon
}

[System.Serializable]
public class ItemDrop {
    public string itemName;
    public int itemCount;
    public float dropChance; // 0 to 1
    public bool isUniqueDrop; // if True, only given if no other unique drop is given
}

[System.Serializable]
public class Items {
    public Item[] items;
}

[System.Serializable]
public class Item {

    public ItemType itemType;
    public string itemName;
    public bool targetsAlly;
    public string description;
    public bool targetsEnemy;
    public Item(string itemType, string itemName) {
        switch (itemType.ToLower()) {
            case "consumable":
                this.itemType = ItemType.Consumable;
                break;
            case "damageable":
                this.itemType = ItemType.Damageable;
                break;
            case "weapon":
                this.itemType = ItemType.Weapon;
                break;
        }
        this.itemName = itemName;
    }
    public ItemType GetItemType() {
        return itemType;
    }
    public string GetName() {
        return itemName;
    }
    public string GetDescription() {
        return description;
    }
    public bool GetIfTargetsAlly() {
        return targetsAlly;
    }
    public bool GetIfTargetsEnemy() {
        return targetsEnemy;
    }

}