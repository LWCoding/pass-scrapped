using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    // ANIMATE THE PLAYER MOVING BEFORE THE ATTACK
    private IEnumerator StartPursuitCoroutine() {
        yield return MoveToTarget();
        yield return PlayPursuitCoroutine();
        yield return BackToStartCoroutine();
    }
    
    // ANIMATE THE BEGINNING OF THE ATTACK AND AWAIT PLAYER INPUT
    private IEnumerator PlayPursuitCoroutine() {
        if (targetedCharacters.Count == 0) { yield break; }
        GetComponent<SpriteRenderer>().sortingOrder += 10;
        PlayAnimation("AttackStart");
        yield return PromptPursuitInput();
        HideRhythmHitterObject();
        float effectiveness = CalculateEffectiveness();
        int calculatedDamage = (int)(effectiveness * characterRef.GetDamage() * 1.5f);
        PlayAnimation("AttackEnd");
        List<int> toRemove = new List<int>();
        for (int i = 0; i < targetedCharacters.Count; i++) {
            Character targetChar = targetedCharacters[i].GetComponent<BattleCharacterController>().characterRef;
            bool success = targetChar.spriteObject.GetComponent<BattleCharacterController>().TakeDamageSequence(calculatedDamage);
            if (success) {
                targetChar.InflictStatusEffect(new StatusEffect(Effect.Vulnerable, 3));
                targetChar.InflictStatusEffect(new StatusEffect(Effect.Paralyze, 3), 0.4f);
                targetChar.InflictStatusEffect(new StatusEffect(Effect.Poison, 3), 0.8f);
            }
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
    private IEnumerator PromptPursuitInput() {
        yield return ShowRhythmHitterObject(RhythmHitterType.BarGrowAndShrink);
    }

}
