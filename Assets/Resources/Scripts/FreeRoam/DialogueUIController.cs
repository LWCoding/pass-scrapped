using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class DialogueUIController : MonoBehaviour
{

    [HideInInspector] public static DialogueUIController instance;
    private List<GameObject> instantiatedOptionButtons = new List<GameObject>();
    private string storedLeftAnim = ""; // Saves left character animation
    private string storedRightAnim = ""; // Saves right character animation
    private string storedOneShot = ""; // Saves one shot direction
    private Animator dialogueBoxAnim;
    private Transform optionsContainerTransform;
    private GameObject leftSpriteObject;
    private GameObject rightSpriteObject;
    private Transform initialLeftSpriteTransform;
    private Transform initialRightSpriteTransform;
    private TextMeshProUGUI dialogueNameText;
    private TextMeshProUGUI dialogueContentsText;
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> dialogueBlips = new Dictionary<string, AudioClip>();
    [Header("Object Assignments")]
    public GameObject optionPrefab;
    public GameObject dialogueContainer;

    private void Awake()
    {
        LoadAudioClips();
        instance = this.GetComponent<DialogueUIController>();
        dialogueContainer.SetActive(true); // Temporarily set to true so we can access children
        dialogueBoxAnim = dialogueContainer.GetComponent<Animator>();
        optionsContainerTransform = GameObject.Find("OptionsBox").transform;
        leftSpriteObject = GameObject.Find("LeftDialogueSprite");
        rightSpriteObject = GameObject.Find("RightDialogueSprite");
        initialLeftSpriteTransform = leftSpriteObject.transform;
        initialRightSpriteTransform = rightSpriteObject.transform;
        dialogueNameText = GameObject.Find("DialogueName").GetComponent<TextMeshProUGUI>();
        dialogueContentsText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        dialogueContainer.SetActive(false); // Set back to false after we're done with it
        audioSource = GetComponent<AudioSource>();
    }

    /*
        Dynamically loads all of the audio blips by name from Resources.Load().
    */
    private void LoadAudioClips()
    {
        dialogueBlips["Jack"] = (AudioClip)Resources.Load("SFX/Dialogue/JackBlip");
        dialogueBlips["Reno"] = (AudioClip)Resources.Load("SFX/Dialogue/RenoBlip");
        dialogueBlips["Ryan"] = (AudioClip)Resources.Load("SFX/Dialogue/RenoBlip");
    }

    /*
        This function takes in a character's name and returns the
        corresponding audio blip noise that should play during
        their dialogue.
    */
    public AudioClip GetBlipSound(string characterName)
    {
        if (dialogueBlips.ContainsKey(characterName))
        {
            return dialogueBlips[characterName];
        }
        else
        {
            Debug.Log("DialogueUIController.cs received an incorrect audio clip name (" + characterName + ")!");
            return null;
        }
    }

    /*
        Show the DialogueContainer parent GameObject. This can be called
        outside of the class in case another script needs to access children
        of the Container.
    */
    public void ShowDialogueContainer()
    {
        dialogueContainer.SetActive(true);
    }

    /*
        This function should show the dialogue UI when called.
    */
    public IEnumerator Show()
    {
        ShowDialogueContainer();
        dialogueBoxAnim.GetComponent<Animator>().Play("Show");
        yield return new WaitForSeconds(0.001f);
        while (IsPlaying(dialogueBoxAnim, "Show"))
        {
            yield return null;
        }
    }

    /*
        This function should hide the dialogue UI when called.
    */
    public IEnumerator Hide()
    {
        dialogueContainer.GetComponent<Animator>().Play("Hide");
        yield return new WaitForSeconds(0.001f);
        while (IsPlaying(dialogueBoxAnim, "Hide"))
        {
            yield return null;
        }
        dialogueContainer.SetActive(false);
    }

    /*
        Takes the direction representing the sprite that should
        be affected by the "one shot" effect and stores it.
    */
    public void SetOneShot(string oneShot)
    {
        storedOneShot = oneShot;
    }

    /*
        This function takes in a string saying "left" or "right"
        or "both" and another string that is the animation that
        the corresponding GameObject should play. 

        Uses the activeSprite string (left, right, both) to
        determine whether or not they should be Idle or Talking.

        Also takes in a potential "oneShot" parameter that determines
        if the left or right character animation should finish playing
        instead of terminating.
        
        Stores the animation but does not play it.
    */
    public void SetDialogueAnimation(string direction, string animName)
    {
        if (direction != "left" && direction != "right" && direction != "both")
        {
            Debug.Log("UNKNOWN DIRECTION IN SETDIALOGUESPRITE (" + direction + ") AT FREEROAM > DIALOGUEUICONTROLLER.CS");
        }
        if (direction == "left" || direction == "both")
        {
            storedLeftAnim = animName;
        }
        if (direction == "right" || direction == "both")
        {
            storedRightAnim = animName;
        }
        SetDialogueContainers(direction);
    }

    /*
        Some animations are offset or scaled incorrectly. Call
        this every time the left or right sprite is updated.

        Additionally, you can manually set the positions/scales
        through this function if necessary for certain characters. :)
    */
    private void SetDialogueContainers(string direction)
    {
        if (direction == "left" || direction == "both")
        {
            leftSpriteObject.transform.position = initialLeftSpriteTransform.position;
            leftSpriteObject.transform.localScale = initialLeftSpriteTransform.localScale;
            if (storedLeftAnim.Length > 6 && storedLeftAnim.Substring(0, 6) == "Violet")
            {
                leftSpriteObject.transform.localPosition = new Vector3(70, 0, 0);
            }
        }
        if (direction == "right" || direction == "both")
        {
            rightSpriteObject.transform.position = initialRightSpriteTransform.position;
            rightSpriteObject.transform.localScale = initialRightSpriteTransform.localScale;
            if (storedRightAnim.Length > 6 && storedRightAnim.Substring(0, 6) == "Violet")
            {
                rightSpriteObject.transform.localPosition = new Vector3(-70, 0, 0);
            }
        }
    }

    /*
        This function takes in a string saying "left" or "right"
        or "both" and and continues playing the pre-set animation
        for the character.
    */
    public void UpdateAnimations(string direction, bool initialAnimationSet = false)
    {
        if (direction == "left")
        {
            leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + ((initialAnimationSet) ? "Idle" : "Talk"));
            rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + "Idle");
            dialogueBoxAnim.Play("LeftTalking" + ((initialAnimationSet) ? "Start" : ""));
        }
        else if (direction == "right")
        {
            rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + ((initialAnimationSet) ? "Idle" : "Talk"));
            leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + "Idle");
            dialogueBoxAnim.Play("RightTalking" + ((initialAnimationSet) ? "Start" : ""));
        }
        else if (direction == "both")
        {
            leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + ((initialAnimationSet) ? "Idle" : "Talk"));
            rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + ((initialAnimationSet) ? "Idle" : "Talk"));
            dialogueBoxAnim.Play("BothTalking" + ((initialAnimationSet) ? "Start" : ""));
        }
        else
        {
            Debug.Log("UNKNOWN DIRECTION IN SETDIALOGUESPRITE (" + direction + ") AT FREEROAM > DIALOGUEUICONTROLLER.CS");
        }
        StartCoroutine(ResizeSprites());
    }

    /*
        Stops the current dialogue sprite's talking animation.
    */
    public void StopDialogueAnimation(string direction)
    {
        if (storedOneShot == direction)
        {
            storedOneShot = "";
            return;
        }
        if (direction == "left")
        {
            leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + "Idle");
        }
        else if (direction == "right")
        {
            rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + "Idle");
        }
        else if (direction == "both")
        {
            if (storedOneShot == "left")
            {
                rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + "Idle");
                storedOneShot = "";
                return;
            }
            if (storedOneShot == "right")
            {
                leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + "Idle");
                storedOneShot = "";
                return;
            }
            leftSpriteObject.GetComponent<Animator>().Play(storedLeftAnim + "Idle");
            rightSpriteObject.GetComponent<Animator>().Play(storedRightAnim + "Idle");
        }
        StartCoroutine(ResizeSprites());
    }

    private IEnumerator ResizeSprites()
    {
        yield return new WaitForSeconds(0.0001f);
        leftSpriteObject.GetComponent<Image>().SetNativeSize();
        rightSpriteObject.GetComponent<Image>().SetNativeSize();
    }

    /*
        Highlight sprites depending on direction, which can be "left",
        "right", or "both". This simply makes them more apparent to
        give the sense of talking.
    */
    public void HighlightSprite(string direction)
    {
        if (direction == "left")
        {
            leftSpriteObject.GetComponent<Animator>().Play("_Active");
            rightSpriteObject.GetComponent<Animator>().Play("_Inactive");
        }
        else if (direction == "right")
        {
            leftSpriteObject.GetComponent<Animator>().Play("_Inactive");
            rightSpriteObject.GetComponent<Animator>().Play("_Active");
        }
        else if (direction == "both")
        {
            leftSpriteObject.GetComponent<Animator>().Play("_Active");
            rightSpriteObject.GetComponent<Animator>().Play("_Active");
        }
        else
        {
            Debug.Log("UNKNOWN DIRECTION IN HIGHLIGHTSPRITE (" + direction + ") AT FREEROAM > DIALOGUEUICONTROLLER.CS");
        }
    }

    /*
        This function instantly sets the `dialogueNameText` text field to
        the inputted value.
    */
    public void SetNameText(string s)
    {
        dialogueNameText.text = s;
    }

    /*
        This function instantly sets the `dialogueContentsText` text field to
        the inputted value.
    */
    public void SetContentsText(string s)
    {
        dialogueContentsText.text = s;
    }

    /*
        This function slowly loads the text for the inputted value into the
        text field. The player can skip the text from loading by clicking the
        interact keycode again.
    */
    public IEnumerator LoadContentsText(string s, string charName)
    {
        string curr = "";
        WaitForSeconds waitTime = new WaitForSeconds(0.02f);
        IEnumerator blipCoroutine = LoopDialogueBlip(GetBlipSound(charName));
        StartCoroutine(blipCoroutine);
        while (curr.Length != s.Length)
        {
            curr = s.Substring(0, curr.Length + 1);
            SetContentsText(curr);
            if (!Input.GetKey(Globals.keybinds.interactKeycode))
            {
                yield return waitTime;
            }
        }
        StopCoroutine(blipCoroutine);
    }

    /*
        This function plays the dialogue blip sound over a time interval.
        It should be stopped with StopCoroutine() after the dialogue is
        finished.
    */
    public IEnumerator LoopDialogueBlip(AudioClip blipSFX)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.07f);
        while (true)
        {
            audioSource.PlayOneShot(blipSFX, 0.04f);
            yield return wfs;
        }
    }

    /*
        Given a list of DialogueOptions, this function instantiates and positions
        all required assets.
    */
    public void CreateOptions(List<DialogueOption> options)
    {
        float buttonGap = optionPrefab.GetComponent<RectTransform>().rect.width + 20;
        for (int i = 0; i < options.Count; i++)
        {
            DialogueOption option = options[i];
            // Instantiate the option and set initial values.
            GameObject optionObject = Instantiate(optionPrefab, optionsContainerTransform);
            optionObject.GetComponent<OptionButtonController>().SetText(option.optionName);
            optionObject.transform.position = optionsContainerTransform.position - new Vector3(buttonGap * (options.Count - i - 1), 0, 0);
            // Add option to the List<GameObject> to delete after selection.
            instantiatedOptionButtons.Add(optionObject);
        }
    }

    /*
        This function makes a visual indicator of what option is currently being 
        selected.
    */
    public void UpdateOptionSelection(int buttonIdx)
    {
        // Loop through all of the buttons.
        // If the option is the selected optionObject, change its properties.
        for (int i = 0; i < instantiatedOptionButtons.Count; i++)
        {
            GameObject optionObject = instantiatedOptionButtons[i];
            if (i != buttonIdx)
            {
                optionObject.GetComponent<OptionButtonController>().SetOpacity(0.3f);
            }
            else
            {
                optionObject.GetComponent<OptionButtonController>().SetOpacity(1);
            }
        }
    }

    /*
        Deletes all GameObjects that are created from the `CreateOptions` function.
    */
    public void DeleteOptions()
    {
        for (int i = instantiatedOptionButtons.Count - 1; i >= 0; i--)
        {
            Destroy(instantiatedOptionButtons[i]);
        }
        instantiatedOptionButtons.Clear();
    }

    /*
        Starts the screen battle animation and loads the battle scene.
    */
    public void StartBattleAnimation()
    {
        Debug.Log("Starting battle animation!");
    }

    /*
        Returns a boolean representing whether or not the specified animator is
        playing an animation clip with the specified name.
    */
    public bool IsPlaying(Animator anim, string stateName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

}
