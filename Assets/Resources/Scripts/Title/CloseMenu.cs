using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    private bool exitingMenu = false;
    private Animator anim;
    private TitleController titleController;
    private WaitForSeconds wfs;

    public void Start() {
        anim = GetComponent<Animator>();
        titleController = GameObject.Find("Controller").GetComponent<TitleController>();
    }

    void OnMouseOver() {
        if (titleController.filesUninteractable || exitingMenu) { return; }
        anim.Play("Hover");
    }

    void OnMouseExit() {
        if (titleController.filesUninteractable || exitingMenu) { return; }
        anim.Play("Idle");
    }

    void OnMouseDown() {
        if (titleController.filesUninteractable || exitingMenu) { return; }
        exitingMenu = true;
        anim.Play("Select");
        titleController.MenuCloseSound();
        GameObject.Find("Controller").GetComponent<TitleController>().HideSlotAssets();
        StartCoroutine(ResetStatus());
    }

    IEnumerator ResetStatus() {
        wfs = new WaitForSeconds(0.5f);
        yield return wfs;
        exitingMenu = false;
        anim.Play("Idle");
    }

}
