using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    private IEnumerator StartRangedAttackCoroutine() {;
        PlayAnimation("AttackStart");
        yield return PromptRangedAttackInput();
        float effectiveness = CalculateEffectiveness();
        int calculatedDamage = (int)(effectiveness * characterRef.GetDamage());
        characterRef.ChargeEnergy(effectiveness);
        PlayAnimation("AttackEnd");
        foreach (GameObject target in targetedCharacters) {
            Character targetChar = target.GetComponent<BattleCharacterController>().characterRef;
            targetChar.spriteObject.GetComponent<BattleCharacterController>().TakeDamageSequence(calculatedDamage);
        }
        yield return new WaitForSeconds(0.8f);
        PlayIdleAnimation();
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PromptRangedAttackInput() {
        yield return ShowRhythmHitterObject(RhythmHitterType.BarGrowAndShrink);
    }

}
