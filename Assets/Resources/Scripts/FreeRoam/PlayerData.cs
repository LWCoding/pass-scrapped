using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    private BoxCollider2D boxCollider;
    public PlayerController playerController;
    public Animator playerAnimator;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /*  
        The script below runs when the player collides with an
        interactable object. If it identifies as a ChangeScene
        InteractableType, then it runs automatically. Or else,
        the player needs to press the interact key.
    */
    private void OnTriggerEnter2D(Collider2D col)
    {
        playerController.interactableObjects.Add(col.gameObject);
        InteractableController intControl = col.gameObject.GetComponent<InteractableController>();
        intControl.ToggleIcon(true);
        if (intControl.interactableType == InteractableType.ChangeScene || intControl.interactableType == InteractableType.Enemy)
        {
            StartCoroutine(intControl.InteractWith());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        playerController.interactableObjects.Remove(col.gameObject);
        InteractableController intControl = col.gameObject.GetComponent<InteractableController>();
        intControl.ToggleIcon(false);
    }

    public void PlayAnimation(string animName)
    {
        playerAnimator.Play(animName);
    }

}
