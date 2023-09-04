using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleOptionHover : MonoBehaviour
{

    public void UseItem(string usedItemName) {
        Item item = JSONItemFinder.instance.GetItem(usedItemName);
        storedItemRef = item;
        battleController.AnimateTopBoxExtensionSlideUp(false, false);
        if (item.GetIfTargetsAlly()) {
            // If the item targets allies, make the player target an ally
            ItemPromptAllySelection();
        } else if (item.GetIfTargetsEnemy()) {
            // If the item targets enemies, make the player target an enemy
            ItemPromptEnemySelection();
        } else {
            // If nothing is supplied, just call it on the current ally
            SetItemAction(battleController.currentAllyTurn);
        }
    }

    public void ItemPromptAllySelection() {
        BattleOptionController.battleOptionController.targetIntention = TargetIntention.UseItemOnAlly;
        BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingAllyOptions);
        BattleOptionController.battleOptionController.isInteractable = true;
    }

    public void ItemPromptEnemySelection() {
        BattleOptionController.battleOptionController.targetIntention = TargetIntention.UseItemOnEnemy;
        BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingEnemyOptions);
        BattleOptionController.battleOptionController.isInteractable = true;
    }

    public void ItemCallAllySelection(int targetNum) {
        SetItemAction(targetNum);  
    }

    public void ItemCallEnemySelection(int targetNum) {
        SetItemAction(targetNum);  
    }

    public void SetItemAction(int targetNum) {
        battleController.GetCurrentAlly(0).bcc.ShowActionBubbleObject(2);
        SetCurrentAllyAction(ButtonType.Item, targetNum, storedItemRef);
    }

}
