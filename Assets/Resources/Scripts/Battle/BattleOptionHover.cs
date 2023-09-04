using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleOptionHover : MonoBehaviour
{
    
    public ButtonType buttonType;
    public Sprite spriteOnHover;
    private Sprite initialSprite;
    public float initialScale;
    public float desiredScale; // Scale that button will continue to go towards
    public TargetIntention targetIntention;
    private Item storedItemRef; // This is for saving the Item for BattleOptionHover_UseItem.cs
    private Special storedSpecialRef; // Ditto as above but for Specials
    private float incrementOnHover = 0.11f;
    private float scalingSpeedMultiplier = 1;
    private float lastButtonClickTime = 0;
    private BattleController battleController;
    public static BattleOptionHover battleOptionHover;

    private void Awake() {
        battleOptionHover = this;
    }

    public void InitializeButton() {
        initialScale = transform.localScale.x;
        desiredScale = initialScale;
        initialSprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void Start() {
        battleController = BattleController.battleController;
    }

    public void ResetButton() {
        desiredScale = initialScale;
        GetComponent<SpriteRenderer>().sprite = initialSprite;
    }

    public void EnlargeButton() {
        desiredScale = initialScale + incrementOnHover;
        GetComponent<SpriteRenderer>().sprite = spriteOnHover;
    }

    public IEnumerator ButtonSquish() {
        desiredScale = initialScale;
        scalingSpeedMultiplier = 2;
        yield return new WaitForSeconds(0.2f);
        scalingSpeedMultiplier = 1;
        desiredScale = initialScale + incrementOnHover;
    }

    public void SelectOption() {
        // If the box animation is currently playing, don't run anything
        if (battleController.isPlaying(battleController.topBoxExtensionObject.GetComponent<Animator>(), "TopBoxExtensionSlideUp") || battleController.isPlaying(battleController.topBoxExtensionObject.GetComponent<Animator>(), "TopBoxExtensionSlideDown")) {
            return;
        }
        // If the user clicked the button within the last 0.2 seconds, don't run it either
        if (Time.time - lastButtonClickTime < 0.2f) {
            return;
        }
        // If the current turn is somehow past the number of party members, don't run anything
        if (battleController.currentAllyTurn >= Globals.partyMembers.Count) {
            return;
        }
        lastButtonClickTime = Time.time;
        battleController.currentlySelectedButtonType = buttonType;
        Ally currentAlly = battleController.GetCurrentAlly(0);
        // If the box extension is not visible, perform an action based on the button
        switch (buttonType) {
            // ATTACK BUTTON
            case ButtonType.Attack:
                if (Globals.GetAliveEnemies().Count != 1) {
                    battleController.AnimateTopBoxExtensionSlideUp(false, false);
                    battleController.PlayOptionSwitchSFX();
                    BattleOptionController.battleOptionController.targetIntention = TargetIntention.FightEnemy;
                    BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingEnemyOptions);
                    break;
                }
                currentAlly.bcc.ShowActionBubbleObject(0);
                // Only one enemy alive -> target that enemy
                SetCurrentAllyAction(ButtonType.Attack, Globals.GetAliveEnemies()[0]);
                break;
            // SPECIALS BUTTON
            case ButtonType.Special:
                List<string> specialOptions = new List<string>();
                foreach (Special s in currentAlly.GetSpecials()) {
                    specialOptions.Add(s.GetName());
                }
                battleController.AnimateTopBoxExtensionSlideDown(specialOptions);
                break;
            // ITEM BUTTON
            case ButtonType.Item:
                List<string> consumableOptions = new List<string>();
                foreach (string itemName in Globals.inventory.GetKeys()) {
                    if (JSONItemFinder.instance.GetItem(itemName).GetItemType() == ItemType.Consumable) {
                        consumableOptions.Add(itemName);
                    }
                }
                battleController.AnimateTopBoxExtensionSlideDown(consumableOptions);
                break;
            // DEFEND BUTTON
            case ButtonType.Defend:
                currentAlly.bcc.ShowActionBubbleObject(3);
                SetCurrentAllyAction(ButtonType.Defend, -1);
                break;
            // ESCAPE BUTTON
            case ButtonType.Escape:
                currentAlly.bcc.ShowActionBubbleObject(4);
                SetCurrentAllyAction(ButtonType.Escape, -1);
                break;
        }
    }

    public void SetCurrentAllyAction(ButtonType buttonType, int targetNum, Item usedItem = null, Special usedSpecial = null) {
        BattleAction battleAction = new BattleAction(targetNum, buttonType);
        if (usedItem != null) {
            battleAction.SetUsedItem(usedItem);
        }
        if (usedSpecial != null) {
            battleAction.SetUsedSpecial(usedSpecial);
        }
        battleController.battleActions[battleController.currentAllyTurn] = battleAction;
        BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingMainOptions);
        CheckForFullActions();
    }

    public void CheckForFullActions(bool playSFX = true) {
        // Battle is currently playing out
        if (battleController.isGameLoopPlaying) { 
            return;
        }
        if (playSFX) {
            battleController.PlayOptionSelectSFX();
        }
        // Check if all battle actions are filled up
        if (battleController.IsBattleActionsFull()) {
            battleController.currentAllyTurn++;
            battleController.RunGameLoop();
        } else {
            battleController.currentAllyTurn++;
            while (battleController.currentAllyTurn < Globals.partyMembers.Count && Globals.GetAlly(battleController.currentAllyTurn).GetStatus() != CharacterStatus.Alive) {
                battleController.currentAllyTurn++;
            }
            // If we've skipped to the last available ally, run the game loop
            if (battleController.currentAllyTurn == Globals.partyMembers.Count) {
                battleController.RunGameLoop();
            }
        }
    }

    public void FixedUpdate() {
        float difference = Mathf.Abs(transform.localScale.x - desiredScale);
        if (difference > 0.01f) {
            if (transform.localScale.x > desiredScale) {
                if (difference < 0.05f) {
                    transform.localScale -= new Vector3(0.01f * scalingSpeedMultiplier, 0.01f * scalingSpeedMultiplier, 0);
                } else {
                    transform.localScale -= new Vector3(0.03f * scalingSpeedMultiplier, 0.03f * scalingSpeedMultiplier, 0);
                }
            } else {
                if (difference < 0.05f) {
                    transform.localScale += new Vector3(0.01f * scalingSpeedMultiplier, 0.01f * scalingSpeedMultiplier, 0);
                } else {
                    transform.localScale += new Vector3(0.03f * scalingSpeedMultiplier, 0.03f * scalingSpeedMultiplier, 0);
                }
            }
        }
    }

}
