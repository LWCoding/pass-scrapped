using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelingSystem {
    public int currLevel;
    public int currXp;
    public int xpToNextLevel;
    public int GetLevel() {
        return currLevel;
    }
    public int GetCurrentXP() {
        return currXp;
    }
    public int GetXPToNextLevel() {
        return xpToNextLevel;
    }
    public float GetPercentageToNextLevel() {
        return (float)System.Math.Round((float)currXp / xpToNextLevel, 2);
    }
    public void EarnXP(int xpEarned) {
        currXp += xpEarned;
        if (currXp >= xpToNextLevel) {
            currLevel++;
            currXp -= xpToNextLevel;
            xpToNextLevel = CalculateXPToNextLevel();
        }
    }
    private int CalculateXPToNextLevel() {
        return (int)(100 * (Mathf.Pow(currLevel, 1.5f)));
    }
}
