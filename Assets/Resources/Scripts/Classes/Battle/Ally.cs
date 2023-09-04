using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Allies {
    public Ally[] allies;
}


[System.Serializable]
public class Ally : Character {

    public LevelingSystem levelSystem;

    public Ally() {
        this.characterType = CharacterType.Ally;
    }
    public void SetAttackType(AttackType attackType) {
        this.attackType = attackType;
    }
    // Allies may have a level requirement, but enemies do not
    public override List<Special> GetSpecials() {
        List<Special> allowedSpecials = new List<Special>();
        foreach (Special s in specialList) {
            if (levelSystem.GetLevel() >= s.GetLevelRequirement()) {
                allowedSpecials.Add(s);
            }
        }
        return allowedSpecials;
    }
    public override bool CanCast(string specialName) {
        Special s = GetSpecial(specialName);
        if (levelSystem.GetLevel() < s.GetLevelRequirement()) { return false; } 
        return base.CanCast(specialName);
    }
    public void PreviewEnergyCost(string specialName) {
        Special s = GetSpecial(specialName);
        infoBoxHandler.PreviewEnergyLoss(s.GetEnergyCost());
    }
    public void EndPreviewEnergyCost() {
        infoBoxHandler.EndPreviewEnergyLoss();
    }
    public void FullRecover() {
        this.currentHealth = baseStats.GetMaxHealth();
        this.currentEnergy = baseStats.GetMaxEnergy();
    }

}
