using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DialogueManager : MonoBehaviour
{

    private DialogueContainer dialogueContainer;
    private int currDialogueInstance; // Idx for list of dialogues
    private int currDialogueStep; // Idx for current string in dialogues
    private List<DialogueInstance> currConversation = new List<DialogueInstance>();
    private DialogueUIController dialogueUIController;
    private int currOptionSelection = 0; // Idx for current option button selected
    [Header("Object Assignments")]
    public TextAsset dialogueJson;

    /*
        Check if there is a save state for the current dialogue that
        is running, and then set the current instance # to that.
    */
    private void Awake()
    {
        dialogueContainer = GetDialogue();
        // Navigate save information to see if dialogue has been started
        string dialogueName = dialogueContainer.dialogueName;
        if (Globals.dialogueSaves.ContainsKey(dialogueName))
        {
            currDialogueInstance = Globals.dialogueSaves[dialogueName];
        }
        else
        {
            // Or else start from the first list of dialogue options
            currDialogueInstance = 0;
        }
    }

    // Initialize dialogueUIController to be called in later functions.
    private void Start()
    {
        dialogueUIController = DialogueUIController.instance;
    }

    /*
        Call this in the Awake() function to parse the Json into a
        valid DialogueContainer class object.
    */
    private DialogueContainer GetDialogue()
    {
        DialogueContainer dc = JsonUtility.FromJson<DialogueContainer>(dialogueJson.text);
        return dc;
    }

    /*
        Retrieves the current line of dialogue in the DialogueContainer
        class object.
    */
    private DialogueList GetCurrentConversation()
    {
        return dialogueContainer.dialogues.Find((d) => d.id == currDialogueInstance);
    }

    /*
        This function is called when the player interacts with the GameObject
        holding the Dialogue Manager script and is the first instance of
        it happening.
    */
    public IEnumerator StartDialogue()
    {
        currDialogueStep = 0;
        DialogueList dl = dialogueContainer.dialogues.Find((d) => d.id == currDialogueInstance);
        Globals.ToggleFreezePlayer(true);
        dialogueUIController.SetContentsText("");
        Camera.main.orthographicSize -= 1;
        currConversation = dl.rawDialogueList;
        // Set initial sprites if applicable.
        dialogueUIController.SetDialogueAnimation("left", dl.initialLeftSprite);
        dialogueUIController.SetDialogueAnimation("right", dl.initialRightSprite);
        dialogueUIController.ShowDialogueContainer();
        SetCorrectSprites(true);
        // Animate UI to show on screen.
        yield return dialogueUIController.Show();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(RenderDialogue());
    }

    /*
        This function sets the sprites for the left and right
        characters, as well as highlights the correct one.
    */
    public void SetCorrectSprites(bool forceIdle = false)
    {
        DialogueInstance dialogue = currConversation.Find((d) => d.id == currDialogueStep);
        dialogueUIController.SetNameText(dialogue.name);
        // If a character is active, highlight them.
        if (dialogue.HasOneShot())
        {
            dialogueUIController.SetOneShot(dialogue.oneShot);
        }
        if (dialogue.HasActiveSpecifier())
        {
            dialogueUIController.HighlightSprite(dialogue.active);
        }
        if (dialogue.HasLeftSprite())
        {
            dialogueUIController.SetDialogueAnimation("left", dialogue.leftSprite);
        }
        if (dialogue.HasRightSprite())
        {
            dialogueUIController.SetDialogueAnimation("right", dialogue.rightSprite);
        }
        dialogueUIController.UpdateAnimations(dialogue.active, forceIdle);
    }

    public IEnumerator RenderDialogue()
    {
        // If it's the last dialogue, then just stop it there.
        if (currDialogueStep == -1)
        {
            StartCoroutine(EndDialogue());
            yield break;
        }
        while (Input.GetKey(Globals.keybinds.interactKeycode))
        {
            yield return null;
        }
        // Or else, retrieve current DialogueInstance and display onto screen.
        DialogueInstance dialogue = currConversation.Find((d) => d.id == currDialogueStep);
        dialogueUIController.SetNameText(dialogue.name);
        // Display and highlight correct sprites.
        SetCorrectSprites();
        yield return dialogueUIController.LoadContentsText(dialogue.contents, dialogue.name);
        // Make character shut the heck up.
        dialogueUIController.StopDialogueAnimation(dialogue.active);
        // If there's a choice to be made, stop it here.
        if (dialogue.HasOptions())
        {
            StartCoroutine(WaitForOptionChoice(dialogue));
            yield break;
        }
        // If it's not the last dialogue, wait for the next button press.
        StartCoroutine(WaitForButtonPress());
        // Go to the next dialogue after the button press.
        currDialogueStep = dialogue.next;
    }

    /*
        Pauses until the player presses the interactKeycode button.
        This should update the `currDialogueStep` variable to whatever
        the player has chosen. Each `DialogueOption` has a `next` variable
        to set the id to.
    */
    private IEnumerator WaitForOptionChoice(DialogueInstance dialogue)
    {
        List<DialogueOption> options = dialogue.options;
        // Instantiate button objects and set first as selected button.
        dialogueUIController.CreateOptions(options);
        currOptionSelection = 0;
        dialogueUIController.UpdateOptionSelection(currOptionSelection);
        // Loop while the user hasn't confirmed their selection yet.
        while (!Input.GetKeyDown(Globals.keybinds.selectKeycode))
        {
            // Pressing a previous button will shift the current selection left.
            if (Input.GetKeyDown(Globals.keybinds.prevButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altPrevButtonKeycode))
            {
                currOptionSelection--;
                if (currOptionSelection < 0)
                {
                    currOptionSelection++;
                }
                dialogueUIController.UpdateOptionSelection(currOptionSelection);
            }
            // Pressing a next button will shift the current selection left.
            if (Input.GetKeyDown(Globals.keybinds.nextButtonKeycode) || Input.GetKeyDown(Globals.keybinds.altNextButtonKeycode))
            {
                currOptionSelection++;
                if (currOptionSelection >= options.Count)
                {
                    currOptionSelection--;
                }
                dialogueUIController.UpdateOptionSelection(currOptionSelection);
            }
            yield return null;
        }
        // Destroy all option buttons.
        dialogueUIController.DeleteOptions();
        // Go to the `next` dialogue as stored by the selected option.
        currDialogueStep = options[currOptionSelection].next;
        StartCoroutine(RenderDialogue());
    }

    /*
        Pauses until the player presses the interactKeycode button.
    */
    private IEnumerator WaitForButtonPress()
    {
        while (Input.GetKey(Globals.keybinds.interactKeycode))
        {
            yield return null;
        }
        while (!Input.GetKeyDown(Globals.keybinds.interactKeycode))
        {
            yield return null;
        }
        while (!Input.GetKeyUp(Globals.keybinds.interactKeycode))
        {
            yield return null;
        }
        StartCoroutine(RenderDialogue());
    }

    /*
        This function should be called when the DialogueInstance has `-1` as
        the next variable.
    */
    public IEnumerator EndDialogue()
    {
        yield return dialogueUIController.Hide();
        Camera.main.orthographicSize += 1;
        DialogueList dl = dialogueContainer.dialogues[currDialogueInstance];
        // If it's not a looped dialogue, go to the next currDialogueInstance.
        if (!dl.loop)
        {
            currDialogueInstance = dl.next;
        }
        DialogueList newDl = dialogueContainer.dialogues[currDialogueInstance];
        // If there's a prerequisite at this instance, evaluate that logic instead.
        if (newDl.HasPrerequisite())
        {
            // If you do not satisfy the prerequisite, go to the elsePrereq DialogueList.
            if (!EvaluatePrerequisite(newDl.prereq))
            {
                currDialogueInstance = newDl.elsePrereq;
            }
        }
        Globals.ToggleFreezePlayer(false);
    }

}
