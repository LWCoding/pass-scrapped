using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class BattleController : MonoBehaviour
{
    
    public void LoadNarratorEncounterText() {
        if (Globals.battleEnemies.Count == 1) {
            backToMainNarratorText = Globals.GetEnemy(0).GetRandomEncounterMessage();
        } else if (Globals.battleEnemies.Count == 2) {
            int selectedEnemy = 0;
            if (Globals.GetEnemy(0).GetMessagePriority() == Globals.GetEnemy(1).GetMessagePriority()) {
                selectedEnemy = Random.Range(0, 2);
            } else if (Globals.GetEnemy(0).GetMessagePriority() > Globals.GetEnemy(1).GetMessagePriority()) {
                selectedEnemy = 0;
            } else {
                selectedEnemy = 1;
            }
            backToMainNarratorText = Globals.GetEnemy(selectedEnemy).GetRandomGroupEncounterMessage(false, Globals.GetEnemy((selectedEnemy + 1) % 2).GetRawName());
        } else {
            int highestPriority = -1;
            List<Enemy> highestPriorityEnemies = new List<Enemy>();
            foreach (Enemy e in Globals.battleEnemies) {
                if (e.GetMessagePriority() > highestPriority) {
                    highestPriority = e.GetMessagePriority();
                    highestPriorityEnemies.Clear();
                    highestPriorityEnemies.Add(e);
                }
                if (e.GetMessagePriority() == highestPriority) {
                    highestPriorityEnemies.Add(e);
                }
            }
            backToMainNarratorText = highestPriorityEnemies[Random.Range(0, highestPriorityEnemies.Count)].GetRandomGroupEncounterMessage(true);
        }
        LoadNarratorText(backToMainNarratorText);
    }

    public void LoadNarratorIdleText() {
        int highestPriority = -1;
            List<Enemy> highestPriorityEnemies = new List<Enemy>();
            foreach (int i in Globals.GetAliveEnemies()) {
                Enemy e = Globals.GetEnemy(i);
                if (e.GetMessagePriority() > highestPriority) {
                    highestPriority = e.GetMessagePriority();
                    highestPriorityEnemies.Clear();
                    highestPriorityEnemies.Add(e);
                }
                if (e.GetMessagePriority() == highestPriority) {
                    highestPriorityEnemies.Add(e);
                }
            }
            backToMainNarratorText = highestPriorityEnemies[Random.Range(0, highestPriorityEnemies.Count)].GetRandomIdleMessage();
            LoadNarratorText(backToMainNarratorText, 1, true);
    }

    public void LoadNarratorText(string text, float delayMultiplier = 1, bool overrideSkip = false) {
        int currTextLength = narratorText.text.Length - 1;
        if (!overrideSkip && currTextLength > 0 &&
            text.Length > currTextLength && 
            text.Substring(0, currTextLength) == narratorText.text.Substring(0, currTextLength)) { return; }
        StopCoroutine(narratorCoroutine);
        narratorCoroutine = LoadNarratorTextCoroutine(text, delayMultiplier);
        StartCoroutine(narratorCoroutine);
    }

    public void SetNarratorTextToItemDesc() {
        string desc;
        if (currentSubOptionSelected >= buttonOptions.Count) {
            desc = "No item selected...";
        } else {
            string itemName = battleController.buttonOptions[battleController.currentSubOptionSelected];
            string itemDescription = JSONItemFinder.instance.GetItem(itemName).GetDescription();
            desc = itemDescription;
        }
        LoadNarratorText(desc, 0);
    }

    public void SetNarratorTextToSpecialDesc() {
        string desc;
        if (currentSubOptionSelected >= buttonOptions.Count) {
            desc = "";
        } else {
            string specialName = battleController.buttonOptions[battleController.currentSubOptionSelected];
            string specialDescription = GetCurrentAlly(0).GetSpecial(specialName).GetDescription();
            desc = specialDescription;
        }
        LoadNarratorText(desc, 0);
    }

    private IEnumerator LoadNarratorTextCoroutine(string textToLoad, float delayMultiplier) {
        narratorText.text = "";
        string currText = "";
        while (currText.Length < textToLoad.Length - 1) {
            char nextChar = textToLoad[currText.Length];
            currText += nextChar;
            narratorText.text = currText + "|";
            if (delayMultiplier != 0 && !isGameLoopPlaying) {
                PlayDialogueBlipSFX();
                yield return new WaitForSeconds(0.03f * delayMultiplier + ((nextChar == ',' || nextChar == '.' || 
                                                                    nextChar == '?' || nextChar == '!') ? 0.12f * delayMultiplier : 0));
            }
        }
        narratorText.text = textToLoad;
    }

    /// <summary>
    /// Displays the previously generated narrator text for this current round. 
    /// Perfect for back buttons or returning to original state.
    /// </summary>
    public void LoadCurrentIterNarratorText() {
        LoadNarratorText(backToMainNarratorText);
    }

}
