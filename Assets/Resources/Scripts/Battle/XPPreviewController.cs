using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XPPreviewController : MonoBehaviour
{
    
    public LevelUpBarHandler levelUpBarHandler;
    public TextMeshPro previewName;
    [SerializeField] private Sprite jackNeutral;
    [SerializeField] private Sprite jackLevelUp;
    [SerializeField] private Sprite ryanNeutral;
    [SerializeField] private Sprite ryanLevelUp;
    [SerializeField] private Sprite renoNeutral;
    [SerializeField] private Sprite renoLevelUp;

    public void InitializeInfo(string allyName) {
        previewName.text = allyName.ToUpper();
        switch (allyName) {
            case "Jack":
                levelUpBarHandler.playerNeutralIcon = jackNeutral;
                levelUpBarHandler.playerLevelUpIcon = jackLevelUp;
                break;
            case "Ryan":
                levelUpBarHandler.playerNeutralIcon = ryanNeutral;
                levelUpBarHandler.playerLevelUpIcon = ryanLevelUp;
                break;
            case "Reno": 
                levelUpBarHandler.playerNeutralIcon = renoNeutral;
                levelUpBarHandler.playerLevelUpIcon = renoLevelUp;  
                break;
        }
        levelUpBarHandler.playerIconRenderer.sprite = levelUpBarHandler.playerNeutralIcon;
    }

}
