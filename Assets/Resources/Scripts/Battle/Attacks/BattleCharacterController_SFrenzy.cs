using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    // ANIMATE THE PLAYER MOVING BEFORE THE ATTACK
    private IEnumerator StartFrenzyCoroutine() {
        yield return MoveToTarget();
        yield return PlayFrenzyCoroutine();
        yield return PlayFrenzyCoroutine();
        yield return PlayFrenzyCoroutine();
        yield return PlayFrenzyCoroutine();
        yield return BackToStartCoroutine();
    }
    
    // ANIMATE THE BEGINNING OF THE ATTACK AND AWAIT PLAYER INPUT
    private IEnumerator PlayFrenzyCoroutine() {
        if (targetedCharacters.Count == 0) { yield break; }
        GetComponent<SpriteRenderer>().sortingOrder += 10;
        PlayAnimation("AttackStart");
        yield return PromptFrenzyInput();
        HideRhythmHitterObject();
        float effectiveness = CalculateEffectiveness();
        int calculatedDamage = (int)(effectiveness * characterRef.GetDamage() * 2 / 3);
        PlayAnimation("AttackEnd");
        List<int> toRemove = new List<int>();
        for (int i = 0; i < targetedCharacters.Count; i++) {
            Character targetChar = targetedCharacters[i].GetComponent<BattleCharacterController>().characterRef;
            targetChar.spriteObject.GetComponent<BattleCharacterController>().TakeDamageSequence(calculatedDamage);
            if (targetChar.GetStatus() != CharacterStatus.Alive) {
                toRemove.Add(i);
            }
        }
        foreach (int remove in toRemove) {
            targetedCharacters.RemoveAt(remove);
        }
        yield return new WaitForSeconds(0.3f);
        PlayIdleAnimation();
        GetComponent<SpriteRenderer>().sortingOrder -= 10;
    }

    // PLAYER INPUT CODE GOES HERE
    private IEnumerator PromptFrenzyInput() {
        yield return ShowRhythmHitterObject(RhythmHitterType.BarGrowAndShrink);
    }

}
