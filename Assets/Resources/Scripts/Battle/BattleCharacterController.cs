using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleCharacterController : MonoBehaviour
{

    public Character characterRef;
    [Header("Prefab Assignments")]
    public GameObject battleItemPrefab;
    public GameObject statusEffectPrefab;
    [Header("Attack SFX Assignments")]
    public AudioClip swipeChargeSFX;
    public AudioClip swipeHitSFX;
    [Header("Other SFX Assignments")]
    public AudioClip basicHurtSFX;
    [Header("Other Assignments")]
    public List<GameObject> targetedCharacters = new List<GameObject>();
    private Animator anim;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    private float movementSpeed = 0.32f;
    [HideInInspector] public GameObject spriteMaskObject;
    [HideInInspector] public GameObject actionBubbleObject;
    [HideInInspector] public GameObject spriteObject;
    [HideInInspector] public GameObject rhythmHitterObject;
    [HideInInspector] public GameObject characterSelectObject;
    [HideInInspector] public GameObject blinkMaskObject;
    [HideInInspector] public GameObject statusIconsParentObject;
    private IEnumerator hideArrowCoroutine;
    private IEnumerator hurtFlashCoroutine;
    private IEnumerator blinkCoroutine; // On player/enemy select
    private bool currentlyAnimating = false; // So we can't die while animating an attack
    public float damageEffectiveness = 0;
    private AudioSource musicPlayer;
    private BattleController battleController;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        musicPlayer = GetComponent<AudioSource>();
        battleController = GameObject.Find("Controller").GetComponent<BattleController>();
    }

    public void Start()
    {
        PlayAnimation("Idle");
        hideArrowCoroutine = HideArrowObjectCoroutine(true);
        blinkCoroutine = BlinkCoroutine();
    }

    public void ShowActionBubbleObject(int idx)
    {
        if (characterRef.characterType == CharacterType.Enemy) { return; }
        StartCoroutine(ShowActionBubbleObjectCoroutine(idx));
    }

    public IEnumerator ShowActionBubbleObjectCoroutine(int idx)
    {
        actionBubbleObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        actionBubbleObject.GetComponent<SpriteRenderer>().sprite = battleController.actionBubbleIcons[idx];
        actionBubbleObject.GetComponent<SpriteRenderer>().sortingOrder = spriteObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        actionBubbleObject.SetActive(true);
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        for (int i = 0; i < 25; i++)
        {
            actionBubbleObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.04f);
            yield return wfs;
        }
    }

    public void HideActionBubbleObject(float delayBefore)
    {
        if (characterRef.characterType == CharacterType.Enemy) { return; }
        StartCoroutine(HideActionBubbleObjectCoroutine(delayBefore));
    }

    public IEnumerator HideActionBubbleObjectCoroutine(float delayBefore)
    {
        WaitForSeconds wfs = new WaitForSeconds(delayBefore);
        yield return wfs;
        actionBubbleObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        wfs = new WaitForSeconds(0.01f);
        for (int i = 0; i < 20; i++)
        {
            actionBubbleObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
            yield return wfs;
        }
        actionBubbleObject.SetActive(false);
    }

    public IEnumerator ShowRhythmHitterObject(RhythmHitterType type)
    {
        if (characterRef.characterType != CharacterType.Enemy) { rhythmHitterObject.SetActive(true); }
        yield return rhythmHitterObject.GetComponent<HitterController>().Initialize(type, GetComponent<BattleCharacterController>());
    }

    public void HideRhythmHitterObject()
    {
        if (characterRef.characterType == CharacterType.Enemy) { return; }
        rhythmHitterObject.SetActive(false);
    }

    public void ShowArrowObject(bool instantTransition)
    {
        StopCoroutine(hideArrowCoroutine);
        StartCoroutine(ShowArrowObjectCoroutine(instantTransition));
        StopCoroutine(blinkCoroutine);
        StartCoroutine(blinkCoroutine);
        blinkMaskObject.GetComponent<SpriteRenderer>().enabled = true;
        blinkMaskObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        Color currColor = blinkMaskObject.GetComponent<SpriteRenderer>().color;
        blinkMaskObject.GetComponent<SpriteRenderer>().color = new Color(currColor.r, currColor.g, currColor.b, 0.1f);
        GetComponent<SpriteMask>().enabled = true;
    }

    public IEnumerator ShowArrowObjectCoroutine(bool instantTransition)
    {
        if (instantTransition)
        {
            characterSelectObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            characterSelectObject.SetActive(true);
            yield break;
        }
        characterSelectObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        characterSelectObject.SetActive(true);
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        for (int i = 0; i < 25; i++)
        {
            characterSelectObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.04f);
            yield return wfs;
        }
    }

    public void HideArrowObject(bool instantTransition)
    {
        StopCoroutine(hideArrowCoroutine);
        hideArrowCoroutine = HideArrowObjectCoroutine(instantTransition);
        StartCoroutine(hideArrowCoroutine);
        StopCoroutine(blinkCoroutine);
        blinkMaskObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteMask>().enabled = false;
    }

    public IEnumerator HideArrowObjectCoroutine(bool instantTransition)
    {
        if (instantTransition)
        {
            characterSelectObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            characterSelectObject.SetActive(false);
            yield break;
        }
        characterSelectObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        for (int i = 0; i < 20; i++)
        {
            characterSelectObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
            yield return wfs;
        }
        characterSelectObject.SetActive(false);
    }

    public IEnumerator BlinkCoroutine()
    {
        bool losingOpacity = false;
        SpriteRenderer r = blinkMaskObject.GetComponent<SpriteRenderer>();
        WaitForSeconds wfs = new WaitForSeconds(0.03f);
        while (true)
        {
            GetComponent<SpriteMask>().sprite = GetComponent<SpriteRenderer>().sprite;
            if (losingOpacity)
            {
                if (r.color.a < 0.1f)
                {
                    losingOpacity = false;
                }
                r.color -= new Color(0, 0, 0, 0.02f);
            }
            else
            {
                if (r.color.a > 0.3f)
                {
                    losingOpacity = true;
                }
                r.color += new Color(0, 0, 0, 0.02f);
            }
            yield return wfs;
        }
    }

    public void Flash(Color flashColor)
    {
        StartCoroutine(FlashCoroutine(flashColor));
    }

    public IEnumerator FlashCoroutine(Color flashColor)
    {
        SpriteRenderer characterSR = characterRef.bcc.spriteMaskObject.GetComponent<SpriteRenderer>();
        SpriteMask characterSM = characterRef.spriteObject.GetComponent<SpriteMask>();
        characterSR.enabled = true;
        characterSM.enabled = true;
        characterSR.color = flashColor;
        characterSM.sprite = characterRef.spriteObject.GetComponent<SpriteRenderer>().sprite;
        characterSR.sortingOrder = characterRef.spriteObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        float waitTime = 0.05f;
        for (int i = 0; i < 10; i++)
        {
            characterSR.color -= new Color(0, 0, 0, 0.08f);
            WaitForSeconds wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.005f;
        }
        characterSR.enabled = false;
        characterSM.enabled = false;
    }

    public void BecomeUnconscious(float delay = 0)
    {
        // For enemies, we don't care if they're unconscious or not (because skill issue)
        if (characterRef.characterType == CharacterType.Enemy)
        {
            StartCoroutine(DeadCoroutine());
            return;
        }
        HideActionBubbleObject(0);
        // If the current ally goes unconscious while they're making a move
        if (characterRef.GetId() == battleController.currentAllyTurn)
        {
            BattleOptionController.battleOptionController.SetSelectArrow(CharacterType.Ally, false, true);
            BattleOptionController.battleOptionController.SetSelectArrow(CharacterType.Enemy, false, true);
            if (characterRef.GetId() != Globals.partyMembers.Count - 1)
            {
                // Character is not last character
                BattleOptionController.battleOptionController.SetBattleSelectMode(BattleSelectMode.ChoosingMainOptions);
            }
        }
        StartCoroutine(UnconsciousCoroutine(delay));
    }

    private IEnumerator UnconsciousCoroutine(float delay)
    {
        WaitForSeconds wfs = new WaitForSeconds(delay);
        if (delay != 0) { yield return wfs; }
        while (currentlyAnimating)
        {
            yield return null;
        }
        if (battleController.currentAllyTurn < Globals.partyMembers.Count &&
            Globals.GetAlly(battleController.currentAllyTurn).GetId() == characterRef.GetId())
        {
            BattleOptionHover.battleOptionHover.CheckForFullActions(false);
        }
        characterRef.SetStatus(CharacterStatus.Unconscious);
        PlayAnimation("Down", true);
        Shake(0.6f, 0.8f);
        yield return characterRef.infoBoxHandler.DeathTimerCoroutine();
        if (characterRef.GetStatus() != CharacterStatus.Alive)
        {
            StartCoroutine(DeadCoroutine());
        }
    }

    private IEnumerator DeadCoroutine()
    {
        while (currentlyAnimating)
        {
            yield return null;
        }
        characterRef.SetStatus(CharacterStatus.Dead);
        if (characterSelectObject.activeSelf)
        {
            BattleOptionController.battleOptionController.SetSelectArrowToNextValidPlayer(characterRef.GetId());
        }
        PlayAnimation("Dead", true);
        Shake(0.6f, 1.2f);
        if (characterRef.characterType != CharacterType.Enemy)
        {
            WaitForSeconds wfs = new WaitForSeconds(1);
            yield return wfs;
            yield return characterRef.infoBoxHandler.BoxBreakCoroutine();
        }
    }

    public void Shake(float amplitude = 1, float waitMultiplier = 1, bool playDownAnimation = false)
    {
        StartCoroutine(ShakeCoroutine(amplitude, waitMultiplier, playDownAnimation));
    }

    public IEnumerator ShakeCoroutine(float amplitude = 1, float waitMultiplier = 1, bool playDownAnimation = false)
    {
        float waitTime = 0.1f * waitMultiplier;
        float waitTimeInc = 0.005f * waitMultiplier;
        float moveDistance = 0.8f * amplitude;
        float moveDistanceInc = 0.05f * amplitude;
        if (playDownAnimation && characterRef.characterType != CharacterType.Enemy)
        {
            PlayAnimation("Down", true);
        }
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                spriteObject.transform.position += new Vector3(moveDistance / 10, 0, 0);
                WaitForSeconds wfs = new WaitForSeconds(waitTime / 10);
                yield return wfs;
            }
            moveDistance = moveDistance * -1 + ((moveDistance > 0) ? moveDistanceInc * 2 : moveDistanceInc * -2);
            waitTime += waitTimeInc;
        }
        if (playDownAnimation && characterRef.characterType != CharacterType.Enemy)
        {
            PlayAnimation("Idle");
        }
    }

    private float CalculateEffectiveness()
    {
        // Enemy always has 80% - 100% damage mult
        if (characterRef.characterType == CharacterType.Enemy)
        {
            damageEffectiveness = Random.Range(0.8f, 1);
        }
        // For players we simply calculate damage up to 100% of max potential
        damageEffectiveness = Mathf.Min(damageEffectiveness, 1);
        return damageEffectiveness;
    }

    public Character SetTargets(int targetNum)
    {
        Character targetedEnemy;
        if (characterRef.characterType == CharacterType.Ally)
        {
            targetedEnemy = Globals.GetEnemy(targetNum);
        }
        else
        {
            targetedEnemy = Globals.GetAlly(targetNum);
        }
        targetedCharacters.Clear();
        targetedCharacters.Add(targetedEnemy.spriteObject);
        return targetedEnemy;
    }

    public IEnumerator StartAttackCoroutine(int targetNum)
    {
        Character targetedEnemy = SetTargets(targetNum);
        if (characterRef.characterType == CharacterType.Ally)
        {
            targetPosition = targetedEnemy.bcc.startPosition - new Vector3(targetedEnemy.spriteWidth, 0, 0);
        }
        else
        {
            targetPosition = targetedEnemy.bcc.startPosition + new Vector3(targetedEnemy.spriteWidth, 0, 0);
        }
        currentlyAnimating = true;
        switch (characterRef.attackType)
        {
            case AttackType.DefaultWalkAndHit:
                yield return StartBasicSwipeCoroutine();
                break;
            case AttackType.RangedStandInPlace:
                yield return StartRangedAttackCoroutine();
                break;
        }
        currentlyAnimating = false;
    }

    public IEnumerator UseSpecialCoroutine(int targetNum, Special special)
    {
        Character targetedEnemy = SetTargets(targetNum);
        if (characterRef.characterType == CharacterType.Ally)
        {
            targetPosition = targetedEnemy.bcc.startPosition - new Vector3(targetedEnemy.spriteWidth, 0, 0);
        }
        else
        {
            targetPosition = targetedEnemy.bcc.startPosition + new Vector3(targetedEnemy.spriteWidth, 0, 0);
        }
        currentlyAnimating = true;
        switch (special.GetName())
        {
            case "Frenzy":
                yield return StartFrenzyCoroutine();
                break;
            case "Pursuit":
                yield return StartPursuitCoroutine();
                break;
        }
        currentlyAnimating = false;
    }

    public void PlayIdleAnimation(bool turnOver = true)
    {
        if (characterRef.GetStatus() != CharacterStatus.Alive) { return; }
        if (!turnOver && characterRef.isDefending)
        {
            PlayAnimation("DefendLoop");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }

    public IEnumerator UseItemCoroutine(int targetedCharNum, Item item)
    {
        PlayAnimation("ItemUse");
        yield return SpawnItemPrefabCoroutine(item, targetedCharNum);
        PlayIdleAnimation();
    }

    public IEnumerator SpawnItemPrefabCoroutine(Item item, int targetedCharNum)
    {
        switch (item)
        {
            default:
                yield return new WaitForSeconds(0.6f);
                GameObject battleItem = Instantiate(battleItemPrefab);
                battleItem.GetComponent<BattleItemController>().SetItem(item, characterRef, targetedCharNum);
                yield return battleItem.GetComponent<BattleItemController>().AnimateFadeCoroutine();
                Destroy(battleItem);
                break;
        }
    }

    public IEnumerator StartDefendCoroutine()
    {
        PlayAnimation("DefendStart");
        yield return new WaitForSeconds(0.01f);
        while (isPlaying(anim, "DefendStart"))
        {
            yield return null;
        }
        PlayAnimation("DefendLoop");
        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator EscapeAnimation()
    {
        PlayAnimation("Escape");
        yield return new WaitForSeconds(0.01f);
        while (isPlaying(anim, "Escape"))
        {
            yield return null;
        }
        GetComponent<Animator>().enabled = false;
        WaitForSeconds wfs = new WaitForSeconds(0.05f);
        for (int i = 0; i < 10; i++)
        {
            GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.1f);
            yield return wfs;
        }
    }

    public bool isPlaying(Animator anim, string stateName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(characterRef.GetName() + stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    private IEnumerator MoveToTarget()
    {
        float yOffset = 0;
        float changeXValue = transform.position.x + (targetPosition.x - transform.position.x) / 2;
        bool goDown = false;
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        while (true)
        {
            float step = movementSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step) + new Vector3(0, yOffset, 0);
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (characterRef.characterType == CharacterType.Ally && !goDown && transform.position.x > changeXValue)
            {
                goDown = true;
            }
            if (characterRef.characterType == CharacterType.Enemy && !goDown && transform.position.x < changeXValue)
            {
                goDown = true;
            }
            if (goDown)
            {
                yOffset -= 0.006f;
                yOffset = Mathf.Max(0, yOffset);
            }
            else
            {
                yOffset += 0.006f;
                yOffset = Mathf.Min(0.1f, yOffset);
            }
            if (yOffset == 0)
            {
                break;
            }
            yield return wfs;
        }
    }

    private IEnumerator BackToStartCoroutine()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        while (true)
        {
            float step = movementSpeed;
            transform.position = Vector3.MoveTowards(transform.position, startPosition, step);
            if (Vector3.Distance(transform.position, startPosition) < 0.001f)
            {
                break;
            }
            yield return wfs;
        }
    }

    private void PlayAnimation(string animName, bool bypassAlive = false)
    {
        if (!bypassAlive && characterRef.GetStatus() != CharacterStatus.Alive) { return; }
        anim.Play(characterRef.GetName() + animName);
    }

}
