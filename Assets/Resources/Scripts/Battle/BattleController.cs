using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public partial class BattleController : MonoBehaviour
{

    [Header("Prefab Assignments")]
    public GameObject pfCharacterInfoBoxObject;
    public GameObject pfCharacterSpriteObject;
    public GameObject pfBattleTextObject;
    [Header("Object Assignments")]
    public GameObject topBoxObject;
    public GameObject topBoxExtensionObject;
    public GameObject characterInfoBoxParentObject;
    public GameObject alliedCharactersParentObject;
    public GameObject enemyCharactersParentObject;
    public List<GameObject> dropdownSubOptions = new List<GameObject>(); // 6 objects
    public GameObject subOptionsUpArrow;
    public GameObject subOptionsDownArrow;
    public Sprite subOptionArrowDefaultSprite;
    public Sprite subOptionArrowSelectedSprite;
    public Sprite subOptionDefaultSprite;
    public Sprite subOptionSelectedSprite;
    public GameObject fadeOverlayObject;
    public GameObject deathTimerParentObject;
    public TextMeshPro narratorText;
    public BattleEndController battleEndController;
    public Animator introAnimator;
    public GameObject introCoverObject;
    public EnemyEncounterController enemyEncounterController;
    public List<GameObject> topBoxButtons = new List<GameObject>();
    [Header("Audio Assignments")]
    public AudioClip battleMusic;
    public AudioClip optionSwitchSFX;
    public AudioClip optionSelectSFX;
    public AudioClip panelOpenSFX;
    public AudioClip panelCloseSFX;
    public AudioClip notAllowedSFX;
    public AudioClip criticalHitSFX;
    public AudioClip healingSFX;
    public AudioClip dialogueBlipSFX;
    public AudioClip bossEncounterSFX;
    public AudioClip encounterSFX;
    // Game-related variables
    public static BattleController battleController;
    public List<BattleAction> battleActions = new List<BattleAction>();
    public List<Sprite> actionBubbleIcons = new List<Sprite>();
    public Sprite allySelectArrow;
    public Sprite enemySelectArrow;
    public bool isGameLoopPlaying = false;
    private bool isGameOver = false;
    public int currentAllyTurn = 0;
    public int currentDropdownPage = 0;
    public int currentSubOptionSelected = 0;
    public List<string> buttonOptions = new List<string>(); // For dropdown choices
    public ButtonType currentlySelectedButtonType = ButtonType.None;
    public bool subOptionsPrevPageEnabled = false;
    public bool subOptionsNextPageEnabled = false;
    private int currentBattleIteration = 0; // How many back-and-forths have happened
    public string backToMainNarratorText;
    private IEnumerator narratorCoroutine;
    private AudioSource musicPlayer;

    private void Awake()
    {
        battleController = this;
        Globals.audioSource = GetComponent<AudioSource>();
        musicPlayer = GetComponent<AudioSource>();
        narratorCoroutine = LoadNarratorTextCoroutine("", 1); // So StopCoroutine doesn't crash my program
    }

    private void Start()
    {
        // TODO: Remove init functions below
        Globals.LoadGame(0); // Contains Jack & inventory items
        Globals.inventory.AddItem("Stun Grenade", 1);
        Ally a = (Ally)JSONCharacterFinder.instance.GetAllyRef("Reno");
        Globals.partyMembers.Add(a);
        Enemy e = (Enemy)JSONCharacterFinder.instance.GetEnemyRef("Jack");
        Globals.battleEnemies.Add(e);
        Enemy f = (Enemy)JSONCharacterFinder.instance.GetEnemyRef("Jack");
        Globals.battleEnemies.Add(f);
        // Remove init functions above
        StartCoroutine(IntroCutsceneCoroutine());
    }

    private IEnumerator IntroCutsceneCoroutine()
    {
        InitializePartyMembers();
        InitializeEnemies();
        ResetGameLoopData();
        LoadNarratorText("");
        Cursor.visible = false;
        topBoxObject.SetActive(false);
        topBoxExtensionObject.SetActive(false);
        enemyEncounterController.SetSprites(Globals.battleEnemies);
        yield return new WaitForSeconds(0.8f);
        if (Globals.IsBossBattle())
        {
            PlayBossEncounterSFX();
            introAnimator.Play("BossDisplay");
            yield return new WaitForSeconds(0.001f);
            while (isPlayingLayer(introAnimator, "BossDisplay", 3))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
        PlayEncounterSFX();
        yield return new WaitForSeconds(0.6f);
        RunEnterAnimations();
        yield return new WaitForSeconds(1.1f);
        introAnimator.Play("WarningEnter");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(introAnimator, "EnemyEnter"))
        {
            yield return null;
        }
        RunExitAnimations();
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(introAnimator, "EnemyExit"))
        {
            yield return null;
        }
        StartCoroutine(FadeIntroBackground());
        InitializeBattleSprites();
    }

    private void RunEnterAnimations()
    {
        introAnimator.Play("EnemyEnter");
        if (Globals.IsInParty("Jack"))
        {
            introAnimator.Play("JackEnter");
        }
        if (Globals.IsInParty("Reno"))
        {
            introAnimator.Play("RenoEnter");
        }
    }

    private void RunExitAnimations()
    {
        introAnimator.Play("EnemyExit");
        introAnimator.Play("WarningExit");
        if (Globals.IsInParty("Jack"))
        {
            introAnimator.Play("JackExit");
        }
        if (Globals.IsInParty("Reno"))
        {
            introAnimator.Play("RenoExit");
        }
    }

    private IEnumerator FadeIntroBackground()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        SpriteRenderer introCoverRenderer = introCoverObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 25; i++)
        {
            introCoverRenderer.color -= new Color(0, 0, 0, 0.04f);
            yield return wfs;
        }
        introCoverObject.SetActive(false);
    }

    private void InitializeBattleSprites()
    {
        StartMusicLoop();
        AnimateTopBoxSlideIn();
    }

    private void StartMusicLoop()
    {
        musicPlayer.loop = true;
        musicPlayer.clip = battleMusic;
        musicPlayer.Play();
    }

    public void PlayOptionSwitchSFX()
    {
        musicPlayer.PlayOneShot(optionSwitchSFX, 1.6f);
    }

    public void PlayOptionSelectSFX()
    {
        musicPlayer.PlayOneShot(optionSelectSFX, 1.6f);
    }

    public void PlayPanelOpenSFX()
    {
        musicPlayer.PlayOneShot(panelOpenSFX, 0.8f);
    }

    public void PlayPanelCloseSFX()
    {
        musicPlayer.PlayOneShot(panelCloseSFX, 0.7f);
    }

    public void PlayNotAllowedSFX()
    {
        musicPlayer.PlayOneShot(notAllowedSFX, 1.9f);
    }

    public void PlayCriticalHitSFX()
    {
        musicPlayer.PlayOneShot(criticalHitSFX, 1.3f);
    }

    public void PlayHealingSFX()
    {
        musicPlayer.PlayOneShot(healingSFX, 1.3f);
    }

    public void PlayDialogueBlipSFX()
    {
        musicPlayer.PlayOneShot(dialogueBlipSFX, 1.6f);
    }

    public void PlayEncounterSFX()
    {
        musicPlayer.PlayOneShot(encounterSFX, 1.1f);
    }

    public void PlayBossEncounterSFX()
    {
        musicPlayer.PlayOneShot(bossEncounterSFX, 1.1f);
    }

    private GameObject InstantiateInfoBox(Ally a)
    {
        GameObject newInfoBox = Instantiate(pfCharacterInfoBoxObject);
        newInfoBox.GetComponent<InfoBoxHandler>().characterRef = a;
        a.infoBoxHandler = newInfoBox.GetComponent<InfoBoxHandler>();
        newInfoBox.GetComponent<InfoBoxHandler>().SetCharacterReference(a);
        newInfoBox.transform.SetParent(characterInfoBoxParentObject.transform);
        return newInfoBox;
    }

    private void InitializePartyMembers()
    {
        int numPartyMembers = Globals.partyMembers.Count;
        int partyMembersCreated = 0;
        float infoBoxDistanceApart = 2.7f - (partyMembersCreated * 0.15f);
        foreach (Ally partyMember in Globals.partyMembers)
        {
            // Set Unique ID
            partyMember.SetId(partyMembersCreated);
            // Create physical sprite and align properly
            GameObject newCharacterContainer = Instantiate(pfCharacterSpriteObject);
            GameObject newCharacterSprite = newCharacterContainer.transform.GetChild(0).gameObject;
            newCharacterContainer.transform.SetParent(alliedCharactersParentObject.transform);
            newCharacterContainer.transform.position -= new Vector3(0.7f * partyMembersCreated, 0.4f * (4 - numPartyMembers) + 0.8f * partyMembersCreated, 0.1f * partyMembersCreated);
            newCharacterContainer.transform.position += partyMember.spriteTransform;
            newCharacterContainer.transform.localScale = partyMember.spriteScale;
            newCharacterSprite.GetComponent<SpriteRenderer>().sortingOrder = partyMembersCreated * 3 + 1;
            // Create info box and link to sprite (inside InstantiateInfoBox)
            GameObject newInfoBox = InstantiateInfoBox(partyMember);
            newInfoBox.transform.position -= new Vector3(infoBoxDistanceApart * (numPartyMembers - 1) - infoBoxDistanceApart * 2 * partyMembersCreated, 0, 0);
            // Initialize values for Ally and BattleCharacterController
            partyMember.spriteObject = newCharacterSprite;
            partyMember.bcc = newCharacterSprite.GetComponent<BattleCharacterController>();
            partyMember.bcc.characterRef = partyMember;
            partyMember.bcc.startPosition = newCharacterSprite.transform.position;
            partyMember.bcc.spriteObject = partyMember.spriteObject;
            partyMember.bcc.spriteMaskObject = newCharacterSprite.transform.GetChild(0).gameObject;
            partyMember.bcc.actionBubbleObject = newCharacterSprite.transform.GetChild(1).gameObject;
            partyMember.bcc.rhythmHitterObject = newCharacterSprite.transform.GetChild(2).gameObject;
            partyMember.bcc.characterSelectObject = newCharacterSprite.transform.GetChild(3).gameObject;
            partyMember.bcc.characterSelectObject.GetComponent<SpriteRenderer>().sprite = allySelectArrow;
            partyMember.bcc.blinkMaskObject = newCharacterSprite.transform.GetChild(4).gameObject;
            partyMember.bcc.blinkMaskObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            partyMember.bcc.statusIconsParentObject = newCharacterSprite.transform.GetChild(5).gameObject;
            partyMembersCreated++;
        }
    }

    private void InitializeEnemies()
    {
        int numEnemies = Globals.battleEnemies.Count;
        int enemiesCreated = 0;
        foreach (Enemy enemy in Globals.battleEnemies)
        {
            // Set Unique ID
            enemy.SetId(enemiesCreated);
            // Create physical sprite and align properly
            GameObject newCharacterContainer = Instantiate(pfCharacterSpriteObject);
            GameObject newCharacterSprite = newCharacterContainer.transform.GetChild(0).gameObject; // Because the current is a container object
            newCharacterContainer.transform.SetParent(enemyCharactersParentObject.transform);
            newCharacterContainer.transform.localScale *= new Vector2(-1, 1); // Enemies need to be flipped
            newCharacterContainer.transform.position -= new Vector3(-0.7f * enemiesCreated, 0.4f * (4 - numEnemies) + 0.8f * enemiesCreated, 0.1f * enemiesCreated);
            newCharacterSprite.GetComponent<SpriteRenderer>().sortingOrder = enemiesCreated * 3 + 1;
            // Initialize values for Enemy and BattleCharacterController
            enemy.spriteObject = newCharacterSprite;
            enemy.bcc = newCharacterSprite.GetComponent<BattleCharacterController>();
            enemy.bcc.startPosition = newCharacterSprite.transform.position;
            enemy.bcc.characterRef = enemy;
            enemy.bcc.spriteObject = enemy.spriteObject;
            enemy.bcc.spriteMaskObject = newCharacterSprite.transform.GetChild(0).gameObject;
            enemy.bcc.rhythmHitterObject = newCharacterSprite.transform.GetChild(2).gameObject;
            enemy.bcc.characterSelectObject = newCharacterSprite.transform.GetChild(3).gameObject;
            enemy.bcc.characterSelectObject.GetComponent<SpriteRenderer>().sprite = enemySelectArrow;
            enemy.bcc.characterSelectObject.transform.rotation *= Quaternion.Euler(1, 1, -1);
            enemy.bcc.blinkMaskObject = newCharacterSprite.transform.GetChild(4).gameObject;
            enemy.bcc.blinkMaskObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
            enemy.bcc.statusIconsParentObject = newCharacterSprite.transform.GetChild(5).gameObject;
            enemiesCreated++;
        }
    }

    public void InstantiateBattleText(float x, float y, string text, Color c, int fontSize = -1)
    {
        GameObject battleText = Instantiate(pfBattleTextObject);
        battleText.GetComponent<TextMeshPro>().text = text;
        battleText.GetComponent<TextMeshPro>().color = c;
        battleText.transform.position = new Vector3(x, y, 0);
        if (fontSize != -1)
        {
            battleText.GetComponent<TextMeshPro>().fontSize = fontSize;
        }
    }

    private void ResetGameLoopData()
    {
        currentBattleIteration++;
        LoadNarratorText("");
        isGameLoopPlaying = false;
        currentAllyTurn = 0;
        BattleOptionController.battleOptionController.energyExpended.Clear();
        for (int i = 0; i < Globals.partyMembers.Count; i++)
        {
            BattleOptionController.battleOptionController.energyExpended.Add(0);
            Globals.GetAlly(i).UpdateStatusEffectTurns();
        }
        for (int i = 0; i < Globals.battleEnemies.Count; i++)
        {
            Globals.GetEnemy(i).UpdateStatusEffectTurns();
        }
        while (battleController.currentAllyTurn < Globals.partyMembers.Count && Globals.GetAlly(battleController.currentAllyTurn).GetStatus() != CharacterStatus.Alive)
        {
            battleController.currentAllyTurn++;
        }
        battleActions = new List<BattleAction>();
        foreach (Ally partyMember in Globals.partyMembers)
        {
            battleActions.Add(new BattleAction());
            if (partyMember.GetStatus() == CharacterStatus.Alive)
            {
                partyMember.bcc.PlayIdleAnimation();
                partyMember.isDefending = false;
            }
        }
    }

    public Ally GetCurrentAlly(int charOffset)
    {
        return Globals.GetAlly(currentAllyTurn + charOffset);
    }

    public bool IsBattleActionsFull()
    {
        return currentAllyTurn == Globals.partyMembers.Count - 1;
    }

    public void RunGameLoop()
    {
        StartCoroutine(GameLoopCoroutine());
    }

    IEnumerator GameLoopCoroutine()
    {
        isGameLoopPlaying = true;
        // Hide all action bubbles
        int charInst = 1;
        foreach (int a in Globals.GetAliveAllies())
        {
            Globals.GetAlly(a).bcc.HideActionBubbleObject(0.25f * charInst);
            charInst++;
        }
        yield return new WaitForSeconds(0.4f);
        // Hide box extension
        yield return AnimateAndHideBoxExtensionCoroutine(true);
        yield return new WaitForSeconds(0.4f);
        yield return AllyLogicCoroutine();
        yield return EnemyLogicCoroutine();
        // If there are no enemies alive, you win
        if (Globals.GetAliveEnemies().Count == 0)
        {
            StartCoroutine(VictoryCoroutine());
            yield break;
        }
        if (!isGameOver)
        {
            ResetGameLoopData();
            AnimateTopBoxSlideIn();
        }
    }

    private IEnumerator AllyLogicCoroutine()
    {
        int currentAlly = 0;
        foreach (BattleAction battleAction in battleActions)
        {
            BattleAction action = battleAction;
            Ally ally = Globals.GetAlly(currentAlly);
            if (isGameOver)
            {
                yield break;
            }
            if (ally.GetStatus() != CharacterStatus.Alive)
            { // If ally dies before animation
                currentAlly++;
                continue;
            }
            // PARALYZE STATUS EFFECT
            if (ally.HasStatusEffect(Effect.Paralyze))
            {
                if (Random.Range(0, 100) > 25)
                {
                    yield return ally.ParalyzeSuccessEffect();
                    action = new BattleAction(-1, ButtonType.None);
                }
                else
                {
                    yield return ally.ParalyzeFailEffect();
                }
            }
            if (action.action == ButtonType.Attack)
            {
                if (Globals.GetEnemy(action.targetNum).GetStatus() != CharacterStatus.Alive)
                { // If enemy dies before action
                    if (Globals.GetAliveEnemies().Count == 0)
                    {
                        StartCoroutine(VictoryCoroutine());
                        yield break;
                    }
                    else
                    {
                        action.targetNum = Globals.PickRandomAliveEnemy();
                    }
                }
                yield return ally.bcc.StartAttackCoroutine(action.targetNum);
            }
            else if (action.action == ButtonType.Special)
            {
                yield return ally.bcc.UseSpecialCoroutine(action.targetNum, action.GetUsedSpecial());
            }
            else if (action.action == ButtonType.Item)
            {
                yield return ally.bcc.UseItemCoroutine(action.targetNum, action.GetUsedItem());
            }
            else if (action.action == ButtonType.Defend)
            {
                ally.isDefending = true;
                yield return ally.bcc.StartDefendCoroutine();
            }
            else if (action.action == ButtonType.Escape)
            {
                yield return ally.bcc.EscapeAnimation();
                ally.SetStatus(CharacterStatus.Escaped);
                yield return new WaitForSeconds(0.3f);
                if (Globals.GetAliveAllies().Count == 0)
                {
                    StartCoroutine(EscapedCoroutine());
                }
            }
            // POISON STATUS EFFECT
            if (ally.HasStatusEffect(Effect.Poison))
            {
                yield return ally.PoisonEffect();
            }
            currentAlly++;
        }
    }

    private IEnumerator EnemyLogicCoroutine()
    {
        foreach (int enemyNum in Globals.GetAliveEnemies())
        {
            bool isTurnSkipped = false;
            if (isGameOver)
            {
                yield break;
            }
            if (Globals.PickRandomAliveAlly() == -1)
            {
                StartCoroutine(GameOverCoroutine());
                yield break;
            }
            Enemy enemy = Globals.GetEnemy(enemyNum);
            // PARALYZE STATUS EFFECT
            if (enemy.HasStatusEffect(Effect.Paralyze))
            {
                if (Random.Range(0, 100) > 25)
                {
                    yield return enemy.ParalyzeSuccessEffect();
                    isTurnSkipped = true;
                }
                else
                {
                    yield return enemy.ParalyzeFailEffect();
                }
            }
            if (!isTurnSkipped)
            {
                Special s = enemy.CastRandomSpecial();
                if (s == null)
                {
                    yield return enemy.bcc.StartAttackCoroutine(Globals.PickRandomAliveAlly());
                }
                else
                {
                    enemy.UseSpecial(s.GetName()); // Reduce EP
                    yield return enemy.bcc.UseSpecialCoroutine(Globals.PickRandomAliveAlly(), s);
                }
            }
            // POISON STATUS EFFECT
            if (enemy.HasStatusEffect(Effect.Poison))
            {
                yield return enemy.PoisonEffect();
            }
        }
    }

    private IEnumerator EscapedCoroutine()
    {
        isGameOver = true;
        yield return new WaitForSeconds(1.6f);
        GameOver();
    }

    private IEnumerator GameOverCoroutine()
    {
        isGameOver = true;
        yield return new WaitForSeconds(1.6f);
        GameOver();
    }

    private IEnumerator VictoryCoroutine()
    {
        isGameOver = true;
        yield return new WaitForSeconds(1);
        deathTimerParentObject.SetActive(false);

        // Calculate XP Earned per ally
        float xpEarned = CalculateXPEarned() / Globals.partyMembers.Count;
        List<int> xpGained = new List<int>();
        for (int i = 0; i < Globals.partyMembers.Count; i++)
        {
            xpGained.Add((int)(xpEarned * Random.Range(0.9f, 1.1f)));
        }

        // Calculate overall pool of items earned
        List<ItemDrop> itemsEarned = new List<ItemDrop>();
        List<ItemDrop> uniqueBox = new List<ItemDrop>(); // contains all unique items rolled
        foreach (Enemy e in Globals.battleEnemies)
        {
            foreach (ItemDrop drop in e.potentialItemsGained)
            {
                // If the below conditional passes, the drop has been achieved
                if (Random.Range(0f, 1f) < drop.dropChance)
                {
                    if (drop.isUniqueDrop)
                    {
                        uniqueBox.Add(drop);
                    }
                    else
                    {
                        itemsEarned.Add(drop);
                    }
                }
            }
            // If there were unique drops, randomize which one the player got
            if (uniqueBox.Count > 0)
            {
                itemsEarned.Add(uniqueBox[Random.Range(0, uniqueBox.Count)]);
            }
        }

        // Consolidate items earned for items with identical names
        for (int i = 0; i < itemsEarned.Count; i++)
        {
            for (int j = i + 1; j < itemsEarned.Count; j++)
            {
                if (itemsEarned[i].itemName == itemsEarned[j].itemName)
                {
                    itemsEarned[i].itemCount += itemsEarned[j].itemCount;
                    itemsEarned[j].itemCount = 0;
                }
            }
        }
        for (int i = itemsEarned.Count - 1; i >= 0; i--)
        {
            if (itemsEarned[i].itemCount == 0)
            {
                itemsEarned.RemoveAt(i);
            }
        }

        // Add items to inventory
        foreach (ItemDrop drop in itemsEarned)
        {
            Globals.inventory.AddItem(drop.itemName, drop.itemCount);
        }

        // Start end sequence using battleEndController
        battleEndController.StartEndSequence(xpGained, itemsEarned);
    }

    private float CalculateXPEarned()
    {
        int totalXP = 0;
        foreach (Enemy e in Globals.battleEnemies)
        {
            totalXP += e.xpWorth;
        }
        return totalXP;
    }

    // Call this if all party members are gone (downed or escaped)
    public void GameOver()
    {
        deathTimerParentObject.SetActive(false);
        LoadScene.instance.StartLoadScreen();
    }

}
