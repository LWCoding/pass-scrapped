using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleItemController : MonoBehaviour
{
    
    [Header("Sprite Assignments")]
    public Sprite healSyringe001Sprite;
    public Sprite stunGrenade002Sprite;
    private Character c;
    private int targetNum;
    private string itemReference;
    private IEnumerator itemEffect;

    public void SetItem(Item item, Character specifiedCharacter, int targetedCharNum) {
        this.c = specifiedCharacter;
        transform.position = c.spriteObject.transform.position + (Vector3)c.relativeItemTransform;
        targetNum = targetedCharNum;
        itemReference = item.GetName();
        switch (item.GetName()) {
            case "Heal Syringe":
                GetComponent<SpriteRenderer>().sprite = healSyringe001Sprite;
                transform.localScale = new Vector2(0.3f, 0.3f);
                itemEffect = SyringeItemEffect();
                break;
            case "Stun Grenade":
                GetComponent<SpriteRenderer>().sprite = stunGrenade002Sprite;
                transform.localScale = new Vector2(0.55f, 0.55f);
                itemEffect = StunGrenadeEffect();
                break;
        }
    }

    private IEnumerator SyringeItemEffect() {
        yield return new WaitForSeconds(0.25f);
        Ally target = Globals.GetAlly(targetNum);
        if (target.GetStatus() == CharacterStatus.Unconscious) {
            WaitForSeconds wfs = new WaitForSeconds(0.01f);
            for (int i = 0; i < 60; i++) {
                transform.localScale -= new Vector3(0.001f, 0.001f, 0);
                transform.Rotate(0, 0, 6);
                yield return wfs;
            }
            target.bcc.Flash(new Color(1, 0.95f, 0.3f, 0.8f)); 
            Globals.inventory.AddItem(itemReference, 1);
            yield break;
        }
        target.bcc.Flash(new Color(0.3f, 1, 0, 0.8f)); 
        target.infoBoxHandler.ChangeHealth(20);
        BattleController.battleController.InstantiateBattleText(transform.position.x, transform.position.y + 0.5f, "20", new Color(0, 1, 0));
        BattleController.battleController.PlayHealingSFX();
    }

    private IEnumerator StunGrenadeEffect() {
        yield return JumpToPosition(0.2f);
        Enemy target = Globals.GetEnemy(targetNum);
        target.bcc.Flash(new Color(1, 0.3f, 0, 0.8f));
        target.bcc.TakeDamageSequence(25, false, false);
        if (Random.Range(0, 100) < 20) {
            target.InflictStatusEffect(new StatusEffect(Effect.Paralyze, 2));
        }
        yield return new WaitForSeconds(0.15f);
        if (targetNum > 0) {
            Globals.GetEnemy(targetNum - 1).bcc.TakeDamageSequence(10, false, false);
            if (Random.Range(0, 100) < 20) {
                Globals.GetEnemy(targetNum - 1).InflictStatusEffect(new StatusEffect(Effect.Paralyze, 2));
            }
        }
        if (targetNum < Globals.battleEnemies.Count - 1) {
            Globals.GetEnemy(targetNum + 1).bcc.TakeDamageSequence(10, false, false);
            if (Random.Range(0, 100) < 20) {
                Globals.GetEnemy(targetNum + 1).InflictStatusEffect(new StatusEffect(Effect.Paralyze, 2));
            }
        }
    }

    private IEnumerator JumpToPosition(float movementSpeed) {
        Enemy targetEnemy = Globals.GetEnemy(targetNum);
        Vector3 targetPosition = targetEnemy.spriteObject.transform.position - new Vector3(targetEnemy.spriteWidth / 2, 0, 0);        
        float yOffset = 0;
        float changeXValue = transform.position.x + (targetPosition.x - transform.position.x) / 2;
        bool goDown = false;
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        while (true) {
            float step = movementSpeed;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (!goDown && transform.position.x > changeXValue) {
                goDown = true;
            }
            if (goDown) {
                yOffset -= 0.01f;
                yOffset = Mathf.Max(yOffset, 0);
            } else {
                yOffset += 0.013f;
                yOffset = Mathf.Min(yOffset, 0.1f);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step) + new Vector3(0, yOffset, 0);
            if (distanceToTarget < 0.01f) {
                break;
            }
            yield return wfs;
        }
    }    

    public IEnumerator AnimateFadeCoroutine() {
        float upwardsScale = 0.05f;
        StartCoroutine(itemEffect);
        WaitForSeconds wfs = new WaitForSeconds(0.05f);
        for (int i = 0; i < 30; i++) {
            transform.position += new Vector3(0, upwardsScale, 0);
            GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
            upwardsScale -= 0.002f;
            yield return wfs;
        }
    }

}
