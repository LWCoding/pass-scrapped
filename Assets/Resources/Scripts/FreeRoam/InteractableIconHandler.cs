using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableIconHandler : MonoBehaviour
{
    
    public Sprite questionSprite;
    public Sprite exclamationSprite;
    public GameObject iconObject;
    private Animator iconAnimator;

    private void Awake() {
        iconAnimator = GetComponent<Animator>();
    }

    public void Initialize(InteractableState state) {
        switch (state) {
            case InteractableState.Question:
                iconObject.GetComponent<SpriteRenderer>().sprite = questionSprite;
                break;
            case InteractableState.Exclamation:
                iconObject.GetComponent<SpriteRenderer>().sprite = exclamationSprite;
                break;
        }
    }

    public void Show() {
        iconAnimator.Play("IconShow");
    }

    public void Hide() {
        iconAnimator.Play("IconHide");
    }

}
