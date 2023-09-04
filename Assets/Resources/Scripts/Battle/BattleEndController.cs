using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleEndController : MonoBehaviour
{
    
    public GameObject xpPreviewPrefab;
    public GameObject lootPreviewPrefab;
    public GameObject xpPreviewContainerObject;
    public GameObject lootPreviewContainerObject;
    public Animator lootBGAnimator;
    public TextMeshPro excessLootText;
    public TextMeshPro continueText;
    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Takes in a list of XP values that each character gains. 
    /// Optionally also takes in a list of items earned.
    /// </summary>
    public void StartEndSequence(List<int> xpGained, List<ItemDrop> itemsEarned) {
        StartCoroutine(EndSequenceCoroutine(xpGained, itemsEarned));
    }
    
    private IEnumerator EndSequenceCoroutine(List<int> xpGained, List<ItemDrop> itemsEarned) {
        anim.Play("ShowClear");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(anim, "ShowClear")) {
            yield return null;
        }
        anim.Play("LoadXP");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(anim, "LoadXP")) {
            yield return null;
        }
        WaitForSeconds wfs = new WaitForSeconds(0.2f);
        for (int i = 0; i < Globals.partyMembers.Count; i++) {
            Ally ally = Globals.partyMembers[i];
            // Initializing GameObject from Prefab
            GameObject newPreview = Instantiate(xpPreviewPrefab);
            newPreview.transform.SetParent(xpPreviewContainerObject.transform);
            newPreview.transform.position = xpPreviewContainerObject.transform.position;
            newPreview.transform.position -= new Vector3(0, 1.3f * i, 0);
            // Handling XP preview initialization
            newPreview.GetComponent<XPPreviewController>().InitializeInfo(ally.GetRawName());
            // Handling XP bar initialization and manipulation
            LevelUpBarHandler xpBarHandler = newPreview.GetComponent<XPPreviewController>().levelUpBarHandler;
            xpBarHandler.InitializeBarValue(ally.levelSystem);
            newPreview.GetComponent<Animator>().Play("ShowXPPreview");
            xpBarHandler.AddBarValue(xpGained[i]);
            yield return wfs;
        }
        yield return new WaitForSeconds(2.5f);
        anim.Play("LoadLoot");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(anim, "LoadLoot")) {
            yield return null;
        }
        wfs = new WaitForSeconds(0.4f);
        if (itemsEarned.Count == 0) {
            lootBGAnimator.Play("NoLootFound");
        }
        for (int i = 0; i < Mathf.Min(4, itemsEarned.Count); i++) {
            ItemDrop itemEarned = itemsEarned[i];
            // Initializing GameObject from Prefab
            GameObject newPreview = Instantiate(lootPreviewPrefab);
            newPreview.transform.SetParent(lootPreviewContainerObject.transform);
            newPreview.transform.position = lootPreviewContainerObject.transform.position;
            newPreview.transform.position -= new Vector3(0, 1.15f * i, 0);
            // Setting item preview
            newPreview.GetComponent<LootPreviewController>().SetLootSprite(itemEarned.itemName);
            newPreview.GetComponent<LootPreviewController>().SetLootText(itemEarned.itemName, itemEarned.itemCount);
            // Handling fade-in animation
            newPreview.GetComponent<Animator>().Play("ShowLootPreview");
            yield return wfs;
        }
        if (itemsEarned.Count > 4) {
            excessLootText.text = "and " + (itemsEarned.Count - 4) + " other" + ((itemsEarned.Count > 5) ? "s!" : "!");
            yield return new WaitForSeconds(0.35f);
            lootBGAnimator.Play("ExcessLoot");
        }
        yield return new WaitForSeconds(0.8f);
        // Wait for input to end animation
        continueText.text = "PRESS [" + Globals.keybinds.selectKeycode + "] TO CONTINUE...";
        anim.Play("LoadContinueText");
        StartCoroutine(WaitForConfirm());
    }

    private IEnumerator WaitForConfirm() {
        while (!Input.GetKeyDown(Globals.keybinds.selectKeycode)) {
            yield return null;
        }
        LoadScene.instance.StartLoadScreen();
    }

    private bool isPlaying(Animator anim, string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }
    
}
