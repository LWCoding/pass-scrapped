using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUpBarHandler : MonoBehaviour
{
    
    public float maxTransformValue;
    public bool glowAfterIncrease;
    public TextMeshPro levelCounterText;
    public TextMeshPro xpCounterText;
    public SpriteRenderer playerIconRenderer;
    public Animator playerLevelAnimator;
    [HideInInspector] public Sprite playerNeutralIcon;
    [HideInInspector] public Sprite playerLevelUpIcon;
    public LevelingSystem levelSystem;
    private int level;
    private float currValue;
    private float nextValue;

    ///<summary>Takes in a current value and max value and sets the progress bar depending on maxTransformValue.</summary>
    public void InitializeBarValue(LevelingSystem ls) {
        levelSystem = ls;
        currValue = ls.GetCurrentXP();
        nextValue = ls.GetXPToNextLevel();
        levelCounterText.text = "LVL " + ls.GetLevel();
        xpCounterText.text = currValue + "/" + nextValue;
        Vector3 currScale = transform.localScale;
        float currPercent = (currValue / nextValue) * maxTransformValue;
        transform.localScale = new Vector3(currPercent, currScale.y, currScale.z);
    }

    ///<summary>Takes in an added value and animates an increase the progress bar. Speed is an optional param set to 1 for 100%.</summary>
    public void AddBarValue(float addedValue) {
        StartCoroutine(BarIncrease(addedValue));
    }

    public IEnumerator BarIncrease(float added) {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        bool leveledUp = false;
        float incrementTimes = (added / nextValue) / 0.01f;
        int remainder = 0;
        currValue += added;
        if (currValue > nextValue) {
            remainder = (int)(added - (nextValue - (currValue - added)));
            levelSystem.EarnXP((int)(nextValue - (currValue - added)));
            currValue = nextValue;
        } else {
            levelSystem.EarnXP((int)added);
        }
        while (incrementTimes > 0) {
            incrementTimes--;
            transform.localScale += new Vector3(0.01f * maxTransformValue, 0, 0);
            xpCounterText.text = ((int)(nextValue * (transform.localScale.x / maxTransformValue))) + "/" + nextValue;
            yield return wfs;
            if (transform.localScale.x > maxTransformValue) {
                levelCounterText.text = "LVL " + (GetLevel() + 1);
                StartCoroutine(IconLevelUpCoroutine());
                leveledUp = true;
                break;
            }
        }
        xpCounterText.text = currValue + "/" + nextValue;
        nextValue = levelSystem.GetXPToNextLevel();
        // Bar glow after increase; if level up, change icon too
        wfs = new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().color += new Color(0.4f, 0.4f, 0.4f, 0);
        for (int i = 0; i < 10; i++) {  
            GetComponent<SpriteRenderer>().color -= new Color(0.04f, 0.04f, 0.04f, 0);
            yield return wfs;
        }
        // Handle rest of level up if bar exceeded max
        if (leveledUp) {
            yield return new WaitForSeconds(0.5f);
            Vector3 currScale = transform.localScale;
            transform.localScale = new Vector3(0, currScale.y, currScale.z);
            float amountXpUsed = currValue;
            currValue = 0;
            if (remainder > 0) {
                StartCoroutine(BarIncrease(remainder));
            }
        }
    }

    private IEnumerator IconLevelUpCoroutine() {
        playerIconRenderer.sprite = playerLevelUpIcon;
        playerLevelAnimator.Play("Emphasize");
        yield return new WaitForSeconds(1);
        playerIconRenderer.sprite = playerNeutralIcon;
    }

    private int GetLevel() {
        return int.Parse(levelCounterText.text.Split(' ')[1]);
    }

}
