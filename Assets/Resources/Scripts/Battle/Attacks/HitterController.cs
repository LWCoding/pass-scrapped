using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitterController : MonoBehaviour
{

    private BattleCharacterController bcc;
    public GameObject mainGameObject;
    public Sprite boxSprite;
    
    /// <summary>
    /// If the character is an enemy, returns the wait time before damage calc. Or else, prompts input.
    /// </summary>
    public IEnumerator Initialize(RhythmHitterType hitterType, BattleCharacterController bcc) {
        this.bcc = bcc;
        mainGameObject.transform.position = bcc.spriteObject.transform.position;
        switch (hitterType) {
            case RhythmHitterType.BarGrowAndShrink:
                if (bcc.characterRef.characterType == CharacterType.Enemy) {
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
                yield return BarGrowAndShrinkCoroutine();
                break;
        }
    }

    private IEnumerator BarGrowAndShrinkCoroutine() {
        float beginTime = Time.time;
        float damageEffectiveness = 0.5f;
        float durationBeforeAutoSwipe = 1.5f;
        // Initialize sprites
        // mainGameObject.GetComponent<SpriteRenderer>().sprite = boxSprite;
        // mainGameObject.GetComponent<SpriteRenderer>().color = new Color32(6, 233, 0, 255);
        // mainGameObject.SetActive(true);
        mainGameObject.transform.position += new Vector3(0, 2.3f, 0);
        // Main loop for input
        while (Time.time - beginTime < durationBeforeAutoSwipe) {
            // Modify bar
            // mainGameObject.transform.localScale = new Vector3((0.5f - Mathf.Abs(durationBeforeAutoSwipe / 2 - (Time.time - beginTime))) * 4, 0.6f, 1);
            // Check for player input
            if (Input.GetKey(KeyCode.Space)) {
                damageEffectiveness += 0.5f - Mathf.Abs(durationBeforeAutoSwipe / 2 - (Time.time - beginTime));
                break;
            }
            yield return null;
        }
        bcc.damageEffectiveness = damageEffectiveness;
    }   

}
