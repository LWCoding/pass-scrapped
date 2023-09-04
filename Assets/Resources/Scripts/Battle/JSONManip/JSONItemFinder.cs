using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONItemFinder : MonoBehaviour
{

    public TextAsset itemJSON;
    public static JSONItemFinder instance;

    public void Awake() {
        instance = this;
    }

    public Item GetItem(string name) {
        Items itemsInJson = JsonUtility.FromJson<Items>(itemJSON.text);
        foreach (Item item in itemsInJson.items) {
            if (item.GetName() == name) {
                return item;
            }
        }
        Debug.Log("ITEM SPECIFIED IN GETITEM() NOT FOUND. JsonItemFinder.cs (" + name + ")");
        return null;
    }

}
