using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public enum InteractableType
{
    NPC, // Dialogue options
    ChangeScene, // Character walks off screen
    Enemy // Follows character
}

public enum InteractableState
{
    None, Question, Exclamation
}

public class InteractableController : MonoBehaviour
{

    private SpriteRenderer interactableSR;
    public GameObject interactableObject;
    public GameObject interactableIcon;
    [Header("Type of Interactable Object")]
    public InteractableType interactableType; // Handled in PlayerController.cs
    [Header("ChangeScene Assignments")]
    public string nextSceneName; // Handled in PlayerController.cs
    [Header("Can Interact with Player (NOT const)")]
    public bool isInteractable = true;
    public InteractableState interactableState; // Handled in InteractableIconController.cs
    [Header("NPC Assignments")]
    public bool shouldTurnTowardsPlayer = false;
    public bool makePlayerLookBackwards = false;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upSprite;
    public Sprite downSprite;

#if UNITY_EDITOR
    /*  
        Don't delete this, this is useful for debugging.

        If there is no active DialogueManager, then this will throw
        an assertion error (I think).
    */
    private void Awake()
    {
        if (interactableType == InteractableType.NPC)
        {
            Assert.IsNotNull(GetComponent<DialogueManager>());
        }
    }
#endif

    private void Start()
    {
        interactableSR = interactableObject.GetComponent<SpriteRenderer>();
        float translateY = interactableSR.bounds.size.y / 2;
        interactableIcon.transform.Translate(new Vector3(0, translateY, 0));
        interactableIcon.GetComponent<InteractableIconHandler>().Initialize(interactableState);
    }

    /*
        This is called when the current sprite is interacted with
        by the main player object.

        If it should turn towards the player, it uses the player's
        position to find out where to turn towards.
        
        dm.StartDialogue(); will freeze the player's position, but will 
        end it after the interaction is over.
    */
    public IEnumerator InteractWith(Vector3? playerPos = null)
    {
        if (!isInteractable) { yield break; }
        // If it is an NPC, we wait until the user lets go of the
        // interact keycode.
        if (interactableType == InteractableType.NPC)
        {
            while (Input.GetKey(Globals.keybinds.interactKeycode))
            {
                yield return null;
            }
            if (shouldTurnTowardsPlayer)
            {
                TurnTowardsPlayer(playerPos.Value);
            }
            DialogueManager dm = GetComponent<DialogueManager>();
            StartCoroutine(dm.StartDialogue());
        }
        // If it is a ChangeScene or Enemy, this is run automatically when
        // the player collides with the object.
        if (interactableType == InteractableType.ChangeScene)
        {
            LoadScene.instance.StartLoadScreen();
            SceneManager.LoadScene(nextSceneName);
        }
        if (interactableType == InteractableType.Enemy)
        {
            DialogueUIController.instance.StartBattleAnimation();
        }
    }

    /*
        Uses the player's position and this current interactable's
        position to calculate how this sprite should turn.
    */
    private void TurnTowardsPlayer(Vector3 playerPos)
    {
        // Relative positions from this gameObject to the player.
        // Negative = player is to right, positive = player is to left.
        float xOffset = gameObject.transform.position.x - playerPos.x;
        float yOffset = gameObject.transform.position.y - playerPos.y;
        if (Mathf.Abs(xOffset) > Mathf.Abs(yOffset))
        {
            // X Offset is higher, thus left-right.
            if (xOffset > 0)
            {
                interactableSR.sprite = leftSprite;
            }
            else
            {
                interactableSR.sprite = rightSprite;
            }
        }
        else
        {
            // Y Offset is higher, thus top-down.
            if (yOffset > 0)
            {
                interactableSR.sprite = downSprite;
            }
            else
            {
                interactableSR.sprite = upSprite;
            }
        }
    }

    /*
        Determine whether or not the interactable icon should be
        shown depending on player's distance to this character.
    */
    public void ToggleIcon(bool isActive)
    {
        // If there is no interactable state, don't consider an icon sprite.
        if (interactableState == InteractableState.None || !isInteractable)
        {
            return;
        }
        // Or else, show or hide the icon.
        if (isActive)
        {
            interactableIcon.SetActive(isActive);
            interactableIcon.GetComponent<InteractableIconHandler>().Show();
        }
        else
        {
            interactableIcon.GetComponent<InteractableIconHandler>().Hide();
        }
    }

}
