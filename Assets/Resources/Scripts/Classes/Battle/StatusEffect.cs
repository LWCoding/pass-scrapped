using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect {
    Vulnerable, // Takes more damage from all sources
    Poison, // Takes damage after each move
    Paralyze // 75% of not doing turn
}


public class StatusEffect {
    private Effect effectType;
    private int turnsLeft;
    public bool recentlyAdded = true;
    public StatusEffect(Effect effectType, int turnsLeft) {
        this.effectType = effectType;
        this.turnsLeft = turnsLeft;
    }
    public Effect GetEffectType() {
        return effectType;
    }
    public int GetTurnsLeft() {
        return turnsLeft;
    }
    public void TopOffTurnsRemaining(int newTurnCount) {
        if (newTurnCount <= turnsLeft) { return; }
        recentlyAdded = true;
        turnsLeft = newTurnCount;
    }
    public int DeductTurn(CharacterType type) {
        if (type == CharacterType.Ally && recentlyAdded) {
            recentlyAdded = false;
            return turnsLeft;
        }
        turnsLeft--;
        return turnsLeft;
    }
}