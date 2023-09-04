using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class BattleCharacterController : MonoBehaviour
{

    public void InflictStatusEffect(StatusEffect effect, float delay = 0) {
        GameObject effectPrefab = Instantiate(statusEffectPrefab);
        Vector3 statusOffset = new Vector3(((characterRef.characterType == CharacterType.Ally) ? -1 : 1) * 0.8f * ((statusIconsParentObject.transform.childCount - 1) % 3), 0.8f * (statusIconsParentObject.transform.childCount / 3), 0);
        effectPrefab.GetComponent<StatusEffectHandler>().SetEffect(effect, startPosition + new Vector3(0, characterRef.relativeItemTransform.y * 2.5f, 0), delay);
        effectPrefab.transform.SetParent(statusIconsParentObject.transform);
        effectPrefab.transform.position = transform.position + characterRef.relativeStatusTransform + statusOffset;
        effectPrefab.GetComponent<StatusEffectHandler>().imageObject.GetComponent<SpriteRenderer>().sortingOrder = characterRef.spriteObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        effectPrefab.GetComponent<StatusEffectHandler>().turnCounterTextObject.GetComponent<TextMeshPro>().sortingOrder = characterRef.spriteObject.GetComponent<SpriteRenderer>().sortingOrder + 2;
    }

    public void UpdateStatusEffectPositions() {
        for (int i = 0; i < statusIconsParentObject.transform.childCount; i++) {
            GameObject obj = statusIconsParentObject.transform.GetChild(i).gameObject;
            Vector3 statusOffset = new Vector3(((characterRef.characterType == CharacterType.Ally) ? -1 : 1) * 0.8f * ((i - 1) % 3), 0.8f * ((i - 1) / 3), 0);
            obj.transform.position = transform.position + characterRef.relativeStatusTransform + statusOffset;
        }
    }

    public void RemoveStatusEffect(StatusEffect effect) {
        for (int i = 0; i < statusIconsParentObject.transform.childCount; i++) {
            GameObject obj = statusIconsParentObject.transform.GetChild(i).gameObject;
            if (obj.GetComponent<StatusEffectHandler>().GetEffect() == effect.GetEffectType()) {
                DestroyImmediate(obj);
                break;
            }
        }
        UpdateStatusEffectPositions();
    }

    public void SetStatusEffectTurn(StatusEffect effect, int newAmount) {
        for (int i = 0; i < statusIconsParentObject.transform.childCount; i++) {
            GameObject obj = statusIconsParentObject.transform.GetChild(i).gameObject;
            if (obj.GetComponent<StatusEffectHandler>().GetEffect() == effect.GetEffectType()) {
                obj.GetComponent<StatusEffectHandler>().SetTurnCount(newAmount);
                break;
            }
        }
    }

    public void EmphasizeStatusEffect(Effect effect) {
        for (int i = 0; i < statusIconsParentObject.transform.childCount; i++) {
            GameObject obj = statusIconsParentObject.transform.GetChild(i).gameObject;
            if (obj.GetComponent<StatusEffectHandler>().GetEffect() == effect) {
                obj.GetComponent<StatusEffectHandler>().EmphasizeEffect();
                break;
            }
        }
    }

}
