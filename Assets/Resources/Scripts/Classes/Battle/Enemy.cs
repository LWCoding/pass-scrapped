using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemies {
    public Enemy[] enemies;
}


[System.Serializable]
public class Enemy : Character {

    public int xpWorth; // How much XP is gained from killing this enemy
    public int dangerLevel; // How dangerous this enemy is from 1-10
    public bool isBoss; // Whether or not this enemy is a boss enemy
    public List<ItemDrop> potentialItemsGained = new List<ItemDrop>();
    public List<string> encounterMessages = new List<string>();
    public List<string> groupEncounterMessages = new List<string>();
    public List<string> idleMessages = new List<string>();
    public int messagePriority; // Determines which encounter message / idle message is played

    public Enemy() {
        this.characterType = CharacterType.Enemy;
    }
    public void SetEnergyCharge(int energyCharge) {
        this.energyCharge = energyCharge;
    }
    public string GetRandomEncounterMessage() {
        if (encounterMessages.Count == 0) {
            return "Look who decided to pop in!";
        }
        return encounterMessages[Random.Range(0, encounterMessages.Count)].Replace("@s", name);
    }
    public string GetRandomGroupEncounterMessage(bool addEtAl = false, string otherName = "") {
        if (groupEncounterMessages.Count == 0) {
            return "It's a real party up in here now!";
        }
        string additional = "";
        if (otherName != "") {
            additional = " and " + otherName;
        }
        if (addEtAl) {
            string[] random = { " and friends", " and co.", " et al.", " and others" };
            additional += random[Random.Range(0, random.Length)];
        }
        return groupEncounterMessages[Random.Range(0, groupEncounterMessages.Count)].Replace("@t", name + additional);
    }
    public string GetRandomIdleMessage() {
        if (idleMessages.Count == 0) {
            return "A silence fills the room.";
        }
        return idleMessages[Random.Range(0, idleMessages.Count)].Replace("@s", name);
    }
    public int GetMessagePriority() {
        return messagePriority;
    }
    /// <summary>
    /// Returns null if no special can be cast, or else returns the special (after random chance).
    /// </summary>
    public Special CastRandomSpecial() {
        // TODO: Make it random chance if a special is cast or not
        List<Special> castableSpecials = new List<Special>();
        foreach (Special s in GetSpecials()) {
            if (CanCast(s.GetName())) {
                castableSpecials.Add(s);
                break;
            }
        }
        if (castableSpecials.Count == 0) {
            return null;
        }
        return castableSpecials[Random.Range(0, castableSpecials.Count)];
    }
    public void FullRecover() {
        this.currentHealth = baseStats.GetMaxHealth();
        this.currentEnergy = 0; // Enemies start with 0 energy
    }
}