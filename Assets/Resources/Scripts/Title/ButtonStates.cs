using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStates : MonoBehaviour
{

    public enum ButtonType {
        Start, Achievements, Settings
    }

    public ButtonType buttonType;
    public Animator anim;
    private TitleController titleController;
    private float desiredScale; // Scale that button will continue to go towards
    private WaitForSeconds wfs;
    
    public void Awake() {
        titleController = GameObject.Find("Controller").GetComponent<TitleController>();
        anim = GetComponent<Animator>();
        desiredScale = titleController.initialButtonScale;
    }

    public void Start() {
        gameObject.SetActive(false);
    }

    public void ResetButton() {
        desiredScale = titleController.initialButtonScale;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, titleController.initialButtonOpacity);
    }

    public void FixedUpdate() {
        float difference = Mathf.Abs(transform.localScale.x - desiredScale);
        if (difference > 0.03f) {
            if (transform.localScale.x > desiredScale) {
                if (difference < 0.05f) {
                    transform.localScale -= new Vector3(0.01f, 0.01f, 1);
                } else {
                    transform.localScale -= new Vector3(0.03f, 0.03f, 1);
                }
            } else {
                if (difference < 0.05f) {
                    transform.localScale += new Vector3(0.01f, 0.01f, 1);
                } else {
                    transform.localScale += new Vector3(0.03f, 0.03f, 1);
                }
            }
        }
    }

    void OnMouseDown() {
        if (titleController.buttonsUninteractable) { return; }
        titleController.buttonsUninteractable = true;
        titleController.SelectSound();
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject button in buttons) {
            if (button.name == this.name) { 
                anim.Play("Select");
                StartCoroutine(button.GetComponent<ButtonStates>().WaitAndDisappear(1.4f));
                StartCoroutine(ButtonAction(1.4f));
            } else {
                button.GetComponent<Animator>().Play("Disappear");
                StartCoroutine(button.GetComponent<ButtonStates>().WaitAndDisappear(0.733f));
            }
        }
    }

    void OnMouseOver() {
        if (titleController.buttonsUninteractable) { return; }
        AnimatorStateInfo a = anim.GetCurrentAnimatorStateInfo(0);
        if (a.IsName("Show") && a.length <= a.normalizedTime) {
            titleController.HoverSound();
            anim.Play("Glow");
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
            foreach (GameObject button in buttons) {
                if (button.name == this.name) { 
                    desiredScale = titleController.hoverButtonScale;
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                } else {
                    button.GetComponent<ButtonStates>().desiredScale = titleController.initialButtonScale;
                    button.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, titleController.initialButtonOpacity);
                }
            }
        }
    }

    void OnMouseExit() {
        if (titleController.buttonsUninteractable) { return; }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Glow")) {
            anim.Play("Show", 0, 1);
            desiredScale = titleController.initialButtonScale;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, titleController.initialButtonOpacity);
        }
    }

    IEnumerator ButtonAction(float animationTime) {
        wfs = new WaitForSeconds(animationTime);
        yield return wfs;
        titleController.DarkenBG();
        switch (buttonType) {
            case ButtonType.Start:
                titleController.ShowSlotAssets();
                break;
            case ButtonType.Achievements:
                break;
            case ButtonType.Settings:
                titleController.ShowSettingsAssets();
                break;
        }
    }

    IEnumerator WaitAndDisappear(float animationTime) {
        wfs = new WaitForSeconds(animationTime + 0.1f);
        yield return wfs;
        gameObject.SetActive(false);
    }

}
