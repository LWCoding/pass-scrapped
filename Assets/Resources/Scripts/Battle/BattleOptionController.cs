using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetIntention {
    FightEnemy, UseItemOnEnemy, UseItemOnAlly, UseSpecialOnAlly, UseSpecialOnEnemy
}

public partial class BattleOptionController : MonoBehaviour
{
    
    public BattleSelectMode battleSelectMode = BattleSelectMode.ChoosingMainOptions;
    public bool isInteractable = false;
    public TargetIntention targetIntention;
    private int currentlyActiveButton = 0;
    private int currentlyActiveChar = 0;
    private const int numButtons = 5;
    public List<int> energyExpended = new List<int>();
    public static BattleOptionController battleOptionController;
    private BattleController battleController;

    private void Awake() {
        battleOptionController = this;
        for (int i = 0; i < numButtons; i++) {
            transform.GetChild(i).GetComponent<BattleOptionHover>().InitializeButton();
        }
        transform.GetChild(currentlyActiveButton).GetComponent<BattleOptionHover>().EnlargeButton();
    }

    private void Start() {
        battleController = BattleController.battleController;
    }

    private void Update() {
        if (!isInteractable || battleController.isGameLoopPlaying) { return; }
        if (battleSelectMode == BattleSelectMode.ChoosingMainOptions) {
            if (Input.GetKeyDown(Globals.keybinds.nextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.prevButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.altNextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altPrevButtonKeycode)) {
                if (Input.GetKeyDown(Globals.keybinds.nextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altNextButtonKeycode)) {
                    currentlyActiveButton++;
                } else {
                    currentlyActiveButton--;
                }
                if (currentlyActiveButton > 4) { currentlyActiveButton = 0; }
                if (currentlyActiveButton < 0) { currentlyActiveButton = 4; }
                battleController.ResetButtonsScale();
                battleController.PlayOptionSwitchSFX();
                transform.GetChild(currentlyActiveButton).GetComponent<BattleOptionHover>().EnlargeButton();
            }
            if (Input.GetKeyDown(Globals.keybinds.escapeButtonKeycode)) {
                if (battleController.currentAllyTurn < Globals.partyMembers.Count) {
                    battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                }
                if (battleController.currentAllyTurn == 0 || battleController.isGameLoopPlaying) { return; }
                battleController.GetCurrentAlly(-1).bcc.HideActionBubbleObject(0);
                battleController.currentAllyTurn--;
                // Refund battle item used if the player decides to cancel it
                if (battleController.battleActions[battleController.currentAllyTurn].action == ButtonType.Item) {
                    Globals.inventory.AddItem(battleController.battleActions[battleController.currentAllyTurn].GetUsedItem().GetName(), 1);
                }
                if (battleController.battleActions[battleController.currentAllyTurn].action == ButtonType.Special) {
                    battleController.GetCurrentAlly(0).RefundEnergy(energyExpended[battleController.currentAllyTurn]);
                }
                battleController.PlayNotAllowedSFX();
            }
            if (Input.GetKeyDown(Globals.keybinds.selectKeycode)) {
                StartCoroutine(AnimateButtonSquish());
                transform.GetChild(currentlyActiveButton).GetComponent<BattleOptionHover>().SelectOption();
            }
        } else if (battleSelectMode == BattleSelectMode.ChoosingSubOptions) {
            if (Input.GetKeyDown(Globals.keybinds.nextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.prevButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.altNextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altPrevButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                int buttonSel = battleController.currentSubOptionSelected;
                battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                // Clicking A/D or Left/Right Arrow
                if (Input.GetKeyDown(Globals.keybinds.nextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altNextButtonKeycode)) {
                    if (buttonSel == 1 || buttonSel == 3) {
                        buttonSel = 100;
                        battleController.SelectUpArrow();
                    } else if (buttonSel == 5) {
                        buttonSel = 101;
                        battleController.SelectDownArrow();
                    } else if (buttonSel != 100 && buttonSel != 101) {
                        buttonSel++;
                    }
                }
                if (Input.GetKeyDown(Globals.keybinds.prevButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altPrevButtonKeycode)) {
                    if (buttonSel == 100) {
                        buttonSel = 1;
                    } else if (buttonSel == 101) {
                        buttonSel = 5;
                    } else if (buttonSel != 0 && buttonSel != 2 && buttonSel != 4) {
                        buttonSel--;
                    }
                }
                // Clicking W/S or Up/Down Arrow
                if (Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode)) {
                    if (buttonSel == 101) {
                        buttonSel = 100;
                        battleController.SelectUpArrow();
                    } else if (buttonSel != 100) {
                        buttonSel -= 2;
                    }
                } else if (Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                    if (buttonSel == 100) {
                        buttonSel = 101;
                        battleController.SelectDownArrow();
                    } else if (buttonSel != 101) {
                        buttonSel += 2;
                    }
                }
                // Don't let selection go before 0 or after 5, since there are 6 buttons
                if (buttonSel < 0) {
                    buttonSel = 0;
                }
                if (buttonSel > 5 && buttonSel < 100) {
                    buttonSel = 5;
                }
                // Select SubOption and play sound
                battleController.currentSubOptionSelected = buttonSel;
                if (battleController.currentlySelectedButtonType == ButtonType.Item) {
                    battleController.SetNarratorTextToItemDesc();
                } else if (battleController.currentlySelectedButtonType == ButtonType.Special) {
                    battleController.SetNarratorTextToSpecialDesc();
                }
                battleController.SelectSubOption();
                battleController.PlayOptionSwitchSFX();
            }
            if (Input.GetKeyDown(Globals.keybinds.selectKeycode)) {
                switch (battleController.currentSubOptionSelected) {
                    case 100:
                        if (battleController.subOptionsPrevPageEnabled) {
                            battleController.currentDropdownPage--;
                            battleController.SetButtonText();
                        } else {
                            battleController.PlayNotAllowedSFX();
                            return;
                        }
                        break;
                    case 101:
                        if (battleController.subOptionsNextPageEnabled) {
                            battleController.currentDropdownPage++;
                            battleController.SetButtonText();
                        } else {
                            battleController.PlayNotAllowedSFX();
                            return;
                        }
                        break;
                    default:
                        // Button is not in index
                        if (battleController.currentSubOptionSelected >= battleController.buttonOptions.Count) {
                            battleController.PlayNotAllowedSFX();
                            return;
                        }
                        if (battleController.currentlySelectedButtonType == ButtonType.Item) {
                            BattleOptionHover.battleOptionHover.UseItem(battleController.buttonOptions[battleController.currentSubOptionSelected]);
                        } else if (battleController.currentlySelectedButtonType == ButtonType.Special) {
                            Special selected = battleController.GetCurrentAlly(0).GetSpecials()[battleController.currentSubOptionSelected];
                            if (!battleController.GetCurrentAlly(0).CanCast(selected.GetName())) {
                                battleController.PlayNotAllowedSFX();
                                return;
                            }
                            BattleOptionHover.battleOptionHover.UseSpecial(battleController.GetCurrentAlly(0).GetSpecials()[battleController.currentSubOptionSelected]);
                        }
                        break;
                }
            }
            if (Input.GetKeyDown(Globals.keybinds.escapeButtonKeycode)) {
                SetSelectArrow(CharacterType.Ally, false, false);
                SetSelectArrow(CharacterType.Enemy, false, false);
                battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                battleController.AnimateTopBoxExtensionSlideUp(false);
            }
        } else if (battleSelectMode == BattleSelectMode.ChoosingAllyOptions) {
            if (Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                if (Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode)) {
                    if (currentlyActiveChar <= 0) {
                        battleController.PlayNotAllowedSFX();
                        return;
                    }
                    int newIndex = currentlyActiveChar - 1;
                    while (Globals.GetAlly(newIndex).GetStatus() == CharacterStatus.Dead) {
                        newIndex--;
                        if (newIndex == -1) {
                            battleController.PlayNotAllowedSFX();
                            newIndex = -1;
                            break;
                        }
                    }
                    if (newIndex != -1) {
                        currentlyActiveChar = newIndex;
                        battleController.PlayOptionSwitchSFX();
                        LoadCurrentlySelectingNarratorText(Globals.GetAlly(currentlyActiveChar));
                    }
                }
                if (Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                    if (currentlyActiveChar >= Globals.partyMembers.Count - 1) {
                        battleController.PlayNotAllowedSFX();
                        return;
                    }
                    int newIndex = currentlyActiveChar + 1;
                    while (Globals.GetAlly(newIndex).GetStatus() == CharacterStatus.Dead) {
                        newIndex++;
                        if (newIndex == Globals.partyMembers.Count) {
                            battleController.PlayNotAllowedSFX();
                            newIndex = -1;
                            break;
                        }
                    }
                    if (newIndex != -1) {
                        currentlyActiveChar = newIndex;
                        battleController.PlayOptionSwitchSFX();
                        LoadCurrentlySelectingNarratorText(Globals.GetAlly(currentlyActiveChar));
                    }
                }
                // Show arrow only on targeted player
                SetSelectArrow(CharacterType.Ally, false, true, currentlyActiveChar);
            }
            if (Input.GetKeyDown(Globals.keybinds.escapeButtonKeycode)) {
                battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                battleController.LoadCurrentIterNarratorText();
                SetBattleSelectMode(BattleSelectMode.ChoosingMainOptions);
                SetSelectArrow(CharacterType.Ally, false, false);
                battleController.PlayNotAllowedSFX();
            }
            if (Input.GetKeyDown(Globals.keybinds.selectKeycode)) {
                if (targetIntention == TargetIntention.UseItemOnAlly) {
                    BattleOptionHover.battleOptionHover.ItemCallAllySelection(currentlyActiveChar);
                    Globals.inventory.RemoveItem(battleController.buttonOptions[battleController.currentSubOptionSelected], 1);
                }
                if (targetIntention == TargetIntention.UseSpecialOnAlly) {
                    battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                    BattleOptionHover.battleOptionHover.SpecialCallAllySelection(currentlyActiveChar);
                    energyExpended[battleController.currentAllyTurn - 1] = battleController.GetCurrentAlly(-1).UseSpecial(battleController.buttonOptions[battleController.currentSubOptionSelected]);
                }
                SetSelectArrow(CharacterType.Ally, false, false);
            }
        } else if (battleSelectMode == BattleSelectMode.ChoosingEnemyOptions) {
            if (Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode) ||
                Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                if (Input.GetKeyDown(Globals.keybinds.upButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altUpButtonKeycode)) {
                    if (currentlyActiveChar <= 0 ) {
                        battleController.PlayNotAllowedSFX();
                        return;
                    }
                    int newIndex = currentlyActiveChar - 1;
                    while (Globals.GetEnemy(newIndex).GetStatus() == CharacterStatus.Dead) {
                        newIndex--;
                        if (newIndex == -1) {
                            battleController.PlayNotAllowedSFX();
                            newIndex = -1;
                            break;
                        }
                    }
                    if (newIndex != -1) {
                        currentlyActiveChar = newIndex;
                        battleController.PlayOptionSwitchSFX();
                        LoadCurrentlySelectingNarratorText(Globals.GetEnemy(currentlyActiveChar));
                    }
                }
                if (Input.GetKeyDown(Globals.keybinds.downButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altDownButtonKeycode)) {
                    if (currentlyActiveChar >= Globals.battleEnemies.Count - 1) {
                        battleController.PlayNotAllowedSFX();
                        return;
                    }
                    int newIndex = currentlyActiveChar + 1;
                    while (Globals.GetEnemy(newIndex).GetStatus() == CharacterStatus.Dead) {
                        newIndex++;
                        if (newIndex == Globals.battleEnemies.Count) {
                            battleController.PlayNotAllowedSFX();
                            newIndex = -1;
                            break;
                        }
                    }
                    if (newIndex != -1) {
                        currentlyActiveChar = newIndex;
                        battleController.PlayOptionSwitchSFX();
                        LoadCurrentlySelectingNarratorText(Globals.GetEnemy(currentlyActiveChar));
                    }
                }
                // Show arrow only on targeted enemy
                SetSelectArrow(CharacterType.Enemy, false, true, currentlyActiveChar);
            }
            if (Input.GetKeyDown(Globals.keybinds.escapeButtonKeycode)) {
                battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                battleController.LoadCurrentIterNarratorText();
                SetBattleSelectMode(BattleSelectMode.ChoosingMainOptions);
                SetSelectArrow(CharacterType.Enemy, false, false);
                battleController.PlayNotAllowedSFX();
            }
            if (Input.GetKeyDown(Globals.keybinds.selectKeycode)) {
                if (targetIntention == TargetIntention.UseItemOnEnemy) {
                    BattleOptionHover.battleOptionHover.ItemCallEnemySelection(currentlyActiveChar);
                    Globals.inventory.RemoveItem(battleController.buttonOptions[battleController.currentSubOptionSelected], 1);
                }
                if (targetIntention == TargetIntention.UseSpecialOnEnemy) {
                    battleController.GetCurrentAlly(0).EndPreviewEnergyCost();
                    BattleOptionHover.battleOptionHover.SpecialCallEnemySelection(currentlyActiveChar);
                    energyExpended[battleController.currentAllyTurn - 1] = battleController.GetCurrentAlly(-1).UseSpecial(battleController.buttonOptions[battleController.currentSubOptionSelected]);
                }
                if (targetIntention == TargetIntention.FightEnemy) {
                    battleController.GetCurrentAlly(0).bcc.ShowActionBubbleObject(0);
                    BattleOptionHover.battleOptionHover.SetCurrentAllyAction(ButtonType.Attack, currentlyActiveChar);
                }
                SetSelectArrow(CharacterType.Enemy, false, false);
            }
        }
    }

    public void LoadCurrentlySelectingNarratorText(Character c) {
        battleController.LoadNarratorText("Currently selecting " + c.GetName(), 0);
    }

    public void SetSelectArrowToNextValidPlayer(int currSelected) {
        int newIndex = currSelected;
        while (Globals.GetAlly(newIndex).GetStatus() == CharacterStatus.Dead) {
            newIndex++;
            if (newIndex == Globals.partyMembers.Count) {
                newIndex = 0;
            }
            if (Globals.GetAliveAllies().Count == 0) {
                newIndex = -1;
                break;
            }
        }
        if (newIndex != -1) {
            currentlyActiveChar = newIndex;
        }
        SetSelectArrow(CharacterType.Ally, false, true, currentlyActiveChar);
    }

    public void SetSelectArrow(CharacterType targetDemographic, bool arrowStatus, bool instantTransition, int excludeId = -1) {
        if (targetDemographic == CharacterType.Ally) {
            foreach (Ally ally in Globals.partyMembers) {
                if (!arrowStatus) {
                    if (ally.GetId() == excludeId) {
                        ally.bcc.ShowArrowObject(instantTransition);
                    } else {
                        ally.bcc.HideArrowObject(instantTransition);
                    }
                } else {
                    if (ally.GetId() == excludeId) {
                        ally.bcc.HideArrowObject(instantTransition);
                    } else {
                        ally.bcc.ShowArrowObject(instantTransition);
                    }
                }
            }
        }
        if (targetDemographic == CharacterType.Enemy) {
            foreach (Enemy enemy in Globals.battleEnemies) {
                if (!arrowStatus) {
                    if (enemy.GetId() == excludeId) {
                        enemy.bcc.ShowArrowObject(instantTransition);
                    } else {
                        enemy.bcc.HideArrowObject(instantTransition);
                    }
                } else {
                    if (enemy.GetId() == excludeId) {
                        enemy.bcc.HideArrowObject(instantTransition);
                    } else {
                        enemy.bcc.ShowArrowObject(instantTransition);
                    }
                }
            }
        }
    }

    public void SetBattleSelectMode(BattleSelectMode bsm) {
        battleSelectMode = bsm;
        if (bsm == BattleSelectMode.ChoosingMainOptions) {
            if (!battleController.isGameLoopPlaying && !battleController.IsBattleActionsFull()) {
                battleController.LoadNarratorText(battleController.backToMainNarratorText);
            }
        }
        if (bsm == BattleSelectMode.ChoosingAllyOptions) {
            currentlyActiveChar = 0;
            while (Globals.GetAlly(currentlyActiveChar).GetStatus() == CharacterStatus.Dead) {
                currentlyActiveChar++;
                if (currentlyActiveChar == Globals.partyMembers.Count - 1) {
                    break;
                }
            }
            LoadCurrentlySelectingNarratorText(Globals.GetAlly(currentlyActiveChar));
            SetSelectArrow(CharacterType.Ally, false, false, currentlyActiveChar);
        }
        if (bsm == BattleSelectMode.ChoosingEnemyOptions) {
            currentlyActiveChar = 0;
            while (Globals.GetEnemy(currentlyActiveChar).GetStatus() == CharacterStatus.Dead) {
                currentlyActiveChar++;
                if (currentlyActiveChar == Globals.battleEnemies.Count - 1) {
                    break;
                }
            }
            LoadCurrentlySelectingNarratorText(Globals.GetEnemy(currentlyActiveChar));
            SetSelectArrow(CharacterType.Enemy, false, false, currentlyActiveChar);
        }
    }

    IEnumerator AnimateButtonSquish() {
        yield return transform.GetChild(currentlyActiveButton).GetComponent<BattleOptionHover>().ButtonSquish();
    }

}
