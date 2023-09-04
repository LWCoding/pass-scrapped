using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotFileHover : MonoBehaviour
{
    
    private Animator anim;
    private TitleController titleController;
    public int fileNumber; // This corresponds to animation name
    private float desiredScale;
    private WaitForSeconds wfs;

    public void Awake() {
        anim = GetComponent<Animator>();
        titleController = GameObject.Find("Controller").GetComponent<TitleController>();
        desiredScale = titleController.initialFileScale;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, titleController.initialFileOpacity);
    }

    public void FixedUpdate() {
        float difference = Mathf.Abs(transform.localScale.x - desiredScale);
        if (difference > 0.01f) {
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


    void OnMouseOver() {
        if (titleController.filesUninteractable) { return; }
        AnimatorStateInfo a = anim.GetCurrentAnimatorStateInfo(0);
        if (a.IsName("FileShow" + fileNumber) && a.length <= a.normalizedTime) {
            titleController.HoverSound();
            anim.Play("FileHover" + fileNumber);
            desiredScale = titleController.hoverFileScale;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateState() {
        if (titleController.selectedFile != fileNumber) { 
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("FileHover" + fileNumber)) {
                anim.Play("FileShow" + fileNumber, 0, 1);
                desiredScale = titleController.initialFileScale;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, titleController.initialFileOpacity);
            }
        }
    }

    void OnMouseExit() {
        if (titleController.filesUninteractable) { return; }
        UpdateState();
    }

    void OnMouseDown() {
        if (titleController.filesUninteractable) { return; }
        if (Input.GetMouseButtonDown(0)) {
            if (titleController.selectedFile != fileNumber) {
                titleController.selectedFile = fileNumber;
                titleController.UpdateSlots();
                titleController.SetSlotText(titleController.slotSelectText.Replace("#", fileNumber.ToString()));
            } else {
                titleController.filesUninteractable = true;
                anim.Play("Importing");
                titleController.SelectSound();
                SaveManager.Load(fileNumber);
                StartCoroutine(ConstantBump());
                StartCoroutine(StartLoadAfterDelay());
            }
        }
    }

    IEnumerator StartLoadAfterDelay() {
        wfs = new WaitForSeconds(1.5f);
        yield return wfs;
        LoadScene.instance.StartLoadScreen();
    }

    IEnumerator ConstantBump() {
        float waitTime = 0.02f;
        for (int j = 0; j < 8; j++) {
            desiredScale += 0.01f;
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.005f;
        }
        waitTime = 0.02f;
        for (int j = 0; j < 8; j++) {
            desiredScale -= 0.01f;
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.005f;
        }
        desiredScale = titleController.hoverFileScale;
        StartCoroutine(ConstantBump());
    }

    public IEnumerator Disappear() {
        wfs = new WaitForSeconds(0.35f); // ShowFile animation time
        yield return wfs;
        gameObject.SetActive(false);
    }

}
