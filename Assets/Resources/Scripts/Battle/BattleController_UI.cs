using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class BattleController : MonoBehaviour
{

    public void ResetButtonsScale() {
        foreach (GameObject button in topBoxButtons) {
            button.GetComponent<BattleOptionHover>().ResetButton();
        }
    }

    private void AnimateTopBoxSlideIn() {
        StartCoroutine(AnimateTopBoxSlideInCoroutine());
    }

    IEnumerator AnimateTopBoxSlideInCoroutine() {
        Animator topBoxObjectAnimator = topBoxObject.GetComponent<Animator>();
        BattleOptionController.battleOptionController.isInteractable = false;
        topBoxObject.SetActive(true);
        topBoxObject.GetComponent<Animator>().Play("TopBoxSlideIn");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(topBoxObjectAnimator, "TopBoxSlideIn")) {
            yield return null;
        }
        if (currentBattleIteration <= 1) {
            LoadNarratorEncounterText();
        } else {
            LoadNarratorIdleText();
        }
        BattleOptionController.battleOptionController.isInteractable = true;
    }

    private void AnimateTopBoxSlideOut() {
        StartCoroutine(AnimateAndHideTopBoxCoroutine());
    }

    IEnumerator AnimateAndHideTopBoxCoroutine() {
        Animator topBoxObjectAnimator = topBoxObject.GetComponent<Animator>();
        BattleOptionController.battleOptionController.isInteractable = false;
        topBoxObjectAnimator.Play("TopBoxSlideOut");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(topBoxObjectAnimator, "TopBoxSlideOut")) {
            yield return null;
        }
        topBoxObject.SetActive(false);
    }

    public void AnimateTopBoxExtensionSlideDown(List<string> options) {
        buttonOptions = options;
        StartCoroutine(AnimateTopBoxExtensionSlideDownCoroutine());
    }

    IEnumerator AnimateTopBoxExtensionSlideDownCoroutine() {
        battleController.currentDropdownPage = 0;
        battleController.currentSubOptionSelected = 0;
        battleController.PlayPanelOpenSFX();
        SetButtonText();
        SelectSubOption();
        Animator topBoxExtensionAnimator = topBoxExtensionObject.GetComponent<Animator>();
        BattleOptionController.battleOptionController.isInteractable = false;
        topBoxExtensionObject.SetActive(true);
        topBoxExtensionObject.GetComponent<Animator>().Play("TopBoxExtensionSlideDown");
        yield return new WaitForSeconds(0.001f);
        if (currentlySelectedButtonType == ButtonType.Item) {
            battleController.SetNarratorTextToItemDesc();
        } else if (currentlySelectedButtonType == ButtonType.Special) {
            battleController.SetNarratorTextToSpecialDesc();
        }
        bool firstRowHidden = false, secondRowHidden = false;
        dropdownSubOptions[4].SetActive(true);
        dropdownSubOptions[5].SetActive(true);
        while (isPlaying(topBoxExtensionAnimator, "TopBoxExtensionSlideDown")) {
            if (!secondRowHidden && topBoxExtensionObject.transform.position.y <= 4) {
                dropdownSubOptions[2].SetActive(true);
                dropdownSubOptions[3].SetActive(true);
                secondRowHidden = true;
            } else if (!firstRowHidden && topBoxExtensionObject.transform.position.y <= 3) {
                dropdownSubOptions[0].SetActive(true);
                dropdownSubOptions[1].SetActive(true);
                firstRowHidden = true;
                BattleOptionController.battleOptionController.battleSelectMode = BattleSelectMode.ChoosingSubOptions;
                BattleOptionController.battleOptionController.isInteractable = true;
            }
            yield return null;
        }
    }

    public void SetButtonText() {
        int index = battleController.currentDropdownPage;
        if ((index + 1) * 6 <= buttonOptions.Count) {
            subOptionsDownArrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            subOptionsNextPageEnabled = true;
        } else {
            subOptionsDownArrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.38f);
            subOptionsNextPageEnabled = false;
        }
        if (index != 0) {
            subOptionsUpArrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            subOptionsPrevPageEnabled = true;
        } else {
            subOptionsUpArrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.38f);
            subOptionsPrevPageEnabled = false;
        }
        for (int i = index * 6; i < (index + 1) * 6; i++) {
            if (i >= buttonOptions.Count) {
                dropdownSubOptions[i % 6].transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
                dropdownSubOptions[i % 6].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.38f);
                continue;
            }
            string itemExt = "";
            if (currentlySelectedButtonType == ButtonType.Item) {
                itemExt = " [x" + Globals.inventory.GetItemCount(buttonOptions[i]) + "]";
            } else if (currentlySelectedButtonType == ButtonType.Special) {
                itemExt = " (-" + GetCurrentAlly(0).GetSpecial(buttonOptions[i]).GetEnergyCost() + "EP)";
            }
            dropdownSubOptions[i % 6].transform.GetChild(0).GetComponent<TextMeshPro>().text = buttonOptions[i] + itemExt;
            dropdownSubOptions[i % 6].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            if (currentlySelectedButtonType == ButtonType.Special) {
                if (!GetCurrentAlly(0).CanCast(buttonOptions[i])) {
                    dropdownSubOptions[i % 6].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.38f);
                }
            }
        }
    }

    public void SelectSubOption() {
        int selectedNum = battleController.currentSubOptionSelected;
        if (selectedNum == 100 || selectedNum == 101) { // 100 reserved for up, 101 reserved for down
            return;
        }
        foreach (Ally a in Globals.partyMembers) {
            if (currentlySelectedButtonType == ButtonType.Special) {
                if (selectedNum < buttonOptions.Count) {
                    if (GetCurrentAlly(0).CanCast(buttonOptions[selectedNum]) && a.GetId() == currentAllyTurn) {
                        a.PreviewEnergyCost(buttonOptions[selectedNum]);
                    }
                } else {
                    a.EndPreviewEnergyCost();
                }
            }
        }
        subOptionsDownArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowDefaultSprite;
        subOptionsUpArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowDefaultSprite;
        for (int i = 0; i < 6; i++) {
            if (selectedNum == i) {
                dropdownSubOptions[i].GetComponent<SpriteRenderer>().sprite = subOptionSelectedSprite;
                continue;
            }
            dropdownSubOptions[i].GetComponent<SpriteRenderer>().sprite = subOptionDefaultSprite;
        }
    }

    public void SelectUpArrow() {
        subOptionsDownArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowDefaultSprite;
        for (int i = 0; i < 6; i++) {
            dropdownSubOptions[i].GetComponent<SpriteRenderer>().sprite = subOptionDefaultSprite;
        }
        subOptionsUpArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowSelectedSprite;
    }

    public void SelectDownArrow() {
        subOptionsUpArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowDefaultSprite;
        for (int i = 0; i < 6; i++) {
            dropdownSubOptions[i].GetComponent<SpriteRenderer>().sprite = subOptionDefaultSprite;
        }
        subOptionsDownArrow.GetComponent<SpriteRenderer>().sprite = subOptionArrowSelectedSprite;
    }

    public void AnimateTopBoxExtensionSlideUp(bool hideTopBarAfter, bool resetToMainOptionsSelectMode = true) {
        StartCoroutine(AnimateAndHideBoxExtensionCoroutine(hideTopBarAfter, resetToMainOptionsSelectMode));
    }

    IEnumerator AnimateAndHideBoxExtensionCoroutine(bool hideTopBarAfter, bool resetToMainOptionsSelectMode = true) {
        Animator topBoxExtensionAnimator = topBoxExtensionObject.GetComponent<Animator>();
        BattleOptionController.battleOptionController.isInteractable = false;
        if (resetToMainOptionsSelectMode) {
            BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingMainOptions);
        }
        if (topBoxExtensionObject.activeSelf) {
            topBoxExtensionAnimator.Play("TopBoxExtensionSlideUp");
            yield return new WaitForSeconds(0.001f);
            bool firstRowHidden = false, secondRowHidden = false;
            while (isPlaying(topBoxExtensionAnimator, "TopBoxExtensionSlideUp")) {
                if (!firstRowHidden && topBoxExtensionObject.transform.position.y >= 3.5) {
                    dropdownSubOptions[0].SetActive(false);
                    dropdownSubOptions[1].SetActive(false);
                    firstRowHidden = true;
                } else if (!secondRowHidden && topBoxExtensionObject.transform.position.y >= 5) {
                    dropdownSubOptions[2].SetActive(false);
                    dropdownSubOptions[3].SetActive(false);
                    secondRowHidden = true;
                }
                yield return null;
            }
            battleController.PlayPanelCloseSFX();
            topBoxExtensionObject.SetActive(false);
        }
        if (hideTopBarAfter) {
            AnimateTopBoxSlideOut();    
        } else {
            BattleOptionController.battleOptionController.isInteractable = true;
            LoadCurrentIterNarratorText();
        }
    }

    public bool isPlaying(Animator anim, string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    public bool isPlayingLayer(Animator anim, string stateName, int layer) {
        return anim.GetCurrentAnimatorStateInfo(layer).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f;
    }

}
