using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LootPreviewController : MonoBehaviour
{
    
    [SerializeField] private TextMeshPro lootText;
    [SerializeField] private TextMeshPro lootCount;
    [SerializeField] private SpriteRenderer lootIconRenderer;
    [SerializeField] private Sprite healSyringe001Sprite;
    [SerializeField] private Sprite stunGrenade002Sprite;

    public void SetLootText(string itemName, int itemCount) {
        lootText.text = itemName;
        lootCount.text = "x" + ((itemCount < 10) ? ("0" + itemCount.ToString()) : itemCount.ToString());
    }

    public void SetLootSprite(string itemName) {
        switch (itemName) {
            case "Heal Syringe":
                lootIconRenderer.sprite = healSyringe001Sprite;
                break;
            case "Stun Grenade":
                lootIconRenderer.sprite = stunGrenade002Sprite;
                break;
        }
    }

}
