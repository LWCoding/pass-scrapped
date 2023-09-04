using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Special {
    public string specialName;
    public string description;
    public int energyCost;
    public bool targetsAlly;
    public bool targetsEnemy;
    public int levelRequirement = 0;
    public int GetLevelRequirement() {
        return levelRequirement;
    }
    public string GetName() {
        return specialName;
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
    public int GetEnergyCost() {
        return energyCost;
    }
}