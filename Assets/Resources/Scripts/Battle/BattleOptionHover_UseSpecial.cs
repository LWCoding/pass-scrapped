using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleOptionHover : MonoBehaviour
{

    public void UseSpecial(Special usedSpecial) {
        storedSpecialRef = usedSpecial;
        battleController.AnimateTopBoxExtensionSlideUp(false, false);
        if (usedSpecial.GetIfTargetsAlly()) {
            // If the special targets allies, make the player target an ally
            SpecialPromptAllySelection();
        } else if (usedSpecial.GetIfTargetsEnemy()) {
            // If the special targets enemies, make the player target an enemy
            SpecialPromptEnemySelection();
        } else {
            // If nothing is supplied, just call it on the current ally
            SetItemAction(battleController.currentAllyTurn);
        }
    }

    public void SpecialPromptAllySelection() {
        BattleOptionController.battleOptionController.targetIntention = TargetIntention.UseSpecialOnAlly;
        BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingAllyOptions);
        BattleOptionController.battleOptionController.isInteractable = true;
    }

    public void SpecialPromptEnemySelection() {
        BattleOptionController.battleOptionController.targetIntention = TargetIntention.UseSpecialOnEnemy;
        BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingEnemyOptions);
        BattleOptionController.battleOptionController.isInteractable = true;
    }

    public void SpecialCallAllySelection(int targetNum) {
        SetSpecialAction(targetNum);  
    }

    public void SpecialCallEnemySelection(int targetNum) {
        SetSpecialAction(targetNum);  
    }

    public void SetSpecialAction(int targetNum) {
        battleController.GetCurrentAlly(0).bcc.ShowActionBubbleObject(1);
        SetCurrentAllyAction(ButtonType.Special, targetNum, null, storedSpecialRef);
    }

}
