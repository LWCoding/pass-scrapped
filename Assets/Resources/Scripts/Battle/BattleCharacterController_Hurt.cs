using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    /// <summary>
    /// Returns a boolean for whether or not the attack succeeded. If false, it may have been a miss/target is dead.
    /// </summary>
    public bool TakeDamageSequence(int damageTaken, bool canDodge = true, bool bypassDefense = false) {
        if (characterRef.GetStatus() != CharacterStatus.Alive) {
            return false;
        }
        if (canDodge && characterRef.RollDodgeChance()) {
            StartCoroutine(DodgeAnimationCoroutine());
            return false;
        }
        damageTaken = characterRef.CalculateDamage(damageTaken, bypassDefense);
        if (characterRef.characterType == CharacterType.Ally) {
            characterRef.infoBoxHandler.ChangeHealth(-1 * damageTaken);
        } else {
            characterRef.currentHealth -= damageTaken;
        }
        HurtAnimation(damageTaken);
        return true;
    }

    public void HurtAnimation(int damageTaken) {
        if (!characterRef.isDefending) {
            PlayAnimation("Hurt");
        }
        spriteMaskObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 125);
        spriteObject.GetComponent<SpriteMask>().enabled = true;
        spriteMaskObject.GetComponent<SpriteRenderer>().enabled = true;
        spriteMaskObject.GetComponent<SpriteRenderer>().sortingOrder = spriteObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        battleController.InstantiateBattleText(transform.position.x, transform.position.y + 0.5f, damageTaken.ToString(), new Color(1, 0, 0));
        if (damageTaken > characterRef.GetMaxHealth() / 3) {
            CameraShake.Shake(0.5f, 0.3f, 0.04f);
        }
        // Stop the old hurt coroutine so the sprite renderer doesn't disappear
        StartCoroutine(HurtAnimationShakeCoroutine());
        if (hurtFlashCoroutine != null) {
            StopCoroutine(hurtFlashCoroutine);
        }
        hurtFlashCoroutine = HurtAnimationColorCoroutine();
        StartCoroutine(hurtFlashCoroutine);
        if (characterRef.currentHealth <= 0 && characterRef.characterType == CharacterType.Enemy) {
            BecomeUnconscious(1.5f);
        }
    }

    IEnumerator DodgeAnimationCoroutine() {
        PlayAnimation("Dodge");
        battleController.InstantiateBattleText(transform.position.x, transform.position.y + 0.5f, "MISS", new Color(1, 0, 0));
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.1f);
        float xChange = (characterRef.characterType == CharacterType.Ally) ? 0.19f : -0.19f;
        float timeToWait = 0.005f;
        for (int i = 0; i < 8; i++) {
            transform.position += new Vector3(xChange, 0, 0);
            timeToWait += 0.002f;
            yield return new WaitForSeconds(timeToWait);
        }
        timeToWait = 0.01f;
        for (int i = 0; i < 8; i++) {
            transform.position += new Vector3(xChange / 3, 0, 0);
            timeToWait += 0.004f;
            yield return new WaitForSeconds(timeToWait);
        }
        timeToWait = 0.005f;
        for (int i = 0; i < 8; i++) {
            transform.position -= new Vector3(xChange * 4 / 3, 0, 0);
            timeToWait += 0.002f;
            yield return new WaitForSeconds(timeToWait);
        }
        GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.1f);
        PlayIdleAnimation();
    }

    IEnumerator HurtAnimationShakeCoroutine() {
        float waitTime = 0.015f;
        float waitTimeInc = 0.005f;
        float moveDistance = 0.6f;
        if (characterRef.isDefending) {
            moveDistance = 0.5f;
        }
        float moveDistanceInc = moveDistance / 6;
        for (int i = 0; i < 10; i++) {
            spriteObject.transform.position += new Vector3(moveDistance / 10, 0, 0); // 0.6f
            yield return new WaitForSeconds(waitTime / 10);
        }
        moveDistance = moveDistance * -2 + moveDistanceInc;
        waitTime += 0.01f;
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 10; j++) {
                spriteObject.transform.position += new Vector3(moveDistance / 10, 0, 0);
                yield return new WaitForSeconds(waitTime / 10);
            }
            moveDistance = moveDistance * -1 + ((moveDistance > 0) ? moveDistanceInc * 2 : moveDistanceInc * -2);
            waitTime += waitTimeInc;
        }
        yield return new WaitForSeconds(0.2f);
        spriteMaskObject.GetComponent<SpriteRenderer>().sortingOrder = spriteObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        PlayIdleAnimation(false);
    }

    IEnumerator HurtAnimationColorCoroutine() {
        yield return new WaitForSeconds(0.0001f);
        spriteObject.GetComponent<SpriteMask>().sprite = spriteObject.GetComponent<SpriteRenderer>().sprite;
        spriteMaskObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
        WaitForSeconds wfs = new WaitForSeconds(0.08f);
        for (int i = 0; i < 10; i++) {
            yield return wfs;
            spriteMaskObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
        }
        spriteObject.GetComponent<SpriteMask>().enabled = false;
        spriteMaskObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
