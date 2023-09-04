using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusEffectHandler : MonoBehaviour
{
    
    public Sprite exposedEffectSprite;
    public Sprite paralzyedEffectSprite;
    public Sprite poisonedEffectSprite;
    public GameObject statusCopyPrefab;
    private Effect statusEffect;
    private BattleController battleController;
    [HideInInspector] public GameObject imageObject;
    private TextMeshPro turnCounterText;
    [HideInInspector] public GameObject turnCounterTextObject;
    private string effectMessage = "EFFECTED!";
    private Vector3 characterPos;
    private int turnCount;

    private void Awake() {
        battleController = GameObject.Find("Controller").GetComponent<BattleController>();
        imageObject = transform.GetChild(0).gameObject;
        turnCounterTextObject = transform.GetChild(1).gameObject;
        turnCounterText = turnCounterTextObject.GetComponent<TextMeshPro>();
    }

    public void SetEffect(StatusEffect effect, Vector3 cp, float delay = 0) {
        characterPos = cp;
        turnCount = effect.GetTurnsLeft();
        statusEffect = effect.GetEffectType();
        UpdateTurnCounter();
        switch (effect.GetEffectType()) {
            case Effect.Vulnerable:
                imageObject.GetComponent<SpriteRenderer>().sprite = exposedEffectSprite;
                effectMessage = "VULNERABLE!";
                break;
            case Effect.Paralyze:
                imageObject.GetComponent<SpriteRenderer>().sprite = paralzyedEffectSprite;
                effectMessage = "PARALYZED!";
                break;
            case Effect.Poison:
                imageObject.GetComponent<SpriteRenderer>().sprite = poisonedEffectSprite;
                effectMessage = "POISONED!";
                break;
        }
        StartCoroutine(InstantiateEffectText(delay));
    }

    public Effect GetEffect() {
        return statusEffect;
    }

    private IEnumerator InstantiateEffectText(float delayBefore = 0) {
        yield return new WaitForSeconds(delayBefore);
        battleController.InstantiateBattleText(characterPos.x, characterPos.y, effectMessage, new Color(1, 0.5f, 0), 6);
    }

    public void SetTurnCount(int amt) {
        if (amt > turnCount) {
            EmphasizeEffect();
        }
        turnCount = amt;
        UpdateTurnCounter();
    }

    public void UpdateTurnCounter() {
        turnCounterText.text = turnCount.ToString();
    }

    public void EmphasizeEffect() {
        GameObject copy = Instantiate(statusCopyPrefab);
        copy.transform.position = transform.position;
        copy.GetComponent<SpriteRenderer>().sortingOrder = imageObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        copy.GetComponent<SpriteRenderer>().sprite = imageObject.GetComponent<SpriteRenderer>().sprite;
        copy.transform.SetParent(transform);
    }

}
