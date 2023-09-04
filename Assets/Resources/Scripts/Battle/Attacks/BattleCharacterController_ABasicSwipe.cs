using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    // ANIMATE THE PLAYER MOVING BEFORE THE ATTACK
    private IEnumerator StartBasicSwipeCoroutine() {
        yield return MoveToTarget();
        yield return PlayBasicSwipeCoroutine();
        yield return BackToStartCoroutine();
    }
    
    // ANIMATE THE BEGINNING OF THE ATTACK AND AWAIT PLAYER INPUT
    private IEnumerator PlayBasicSwipeCoroutine() {
        GetComponent<SpriteRenderer>().sortingOrder += 10;
        PlayAnimation("AttackStart");
        yield return PromptBasicSwipeInput();
        HideRhythmHitterObject();
        float effectiveness = CalculateEffectiveness();
        int calculatedDamage = (int)(effectiveness * characterRef.GetDamage());
        characterRef.ChargeEnergy(effectiveness);
        PlayAnimation("AttackEnd");
        foreach (GameObject target in targetedCharacters) {
            Character targetChar = target.GetComponent<BattleCharacterController>().characterRef;
            targetChar.spriteObject.GetComponent<BattleCharacterController>().TakeDamageSequence(calculatedDamage);
        }
        yield return new WaitForSeconds(0.5f);
        PlayIdleAnimation();
        GetComponent<SpriteRenderer>().sortingOrder -= 10;
    }

    // PLAYER INPUT CODE GOES HERE
    private IEnumerator PromptBasicSwipeInput() {
        if (characterRef.characterType == CharacterType.Ally) {
            musicPlayer.clip = swipeChargeSFX;
            musicPlayer.Play();
        }
        yield return ShowRhythmHitterObject(RhythmHitterType.BarGrowAndShrink);
        musicPlayer.clip = swipeHitSFX;
        musicPlayer.Play();
    }

}
