using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleSelectMode {
    ChoosingMainOptions, ChoosingSubOptions, ChoosingAllyOptions, ChoosingEnemyOptions
}

public enum ButtonType {
    Attack, Special, Item, Defend, Escape, None
}

public enum AttackType {
    DefaultWalkAndHit, RangedStandInPlace
}

public enum RhythmHitterType {
    BarGrowAndShrink
}

public class BattleAction {

    public int targetNum;
    public ButtonType action;
    private Item usedItem;
    private Special usedSpecial;

    public BattleAction(int targetNum, ButtonType action) {
        this.targetNum = targetNum;
        this.action = action;
    }
    public BattleAction() {
        action = ButtonType.None;
    }
    public void SetUsedItem(Item item) {
        usedItem = item;
    }
    public void SetUsedSpecial(Special special) {
        usedSpecial = special;
    }
    public Item GetUsedItem() {
        return usedItem;
    }
    public Special GetUsedSpecial() {
        return usedSpecial;
    }

}