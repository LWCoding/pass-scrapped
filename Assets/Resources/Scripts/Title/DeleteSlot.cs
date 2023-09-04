using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeleteSlot : MonoBehaviour
{
    
    private TitleController titleController;
    [Header("Sprite Assignments")]
    public Sprite idle;
    public Sprite hover;
    public Sprite ongoing;
    public Sprite complete;
    private bool isBusy = false;
    private float desiredScale = 0.8f;
    private string previousText = ""; // Set to this when deleting
    private WaitForSeconds wfs;

    public void Awake() {
        titleController = GameObject.Find("Controller").GetComponent<TitleController>();
    }

    public void FixedUpdate() {
        float difference = Mathf.Abs(transform.localScale.x - desiredScale);
        if (difference > 0.01f) {
            if (transform.localScale.x > desiredScale) {
                if (difference < 0.05f) {
                    transform.localScale -= new Vector3(0.01f, 0.01f, 1);
                } else {
                    transform.localScale -= new Vector3(0.02f, 0.02f, 1);
                }
            } else {
                if (difference < 0.05f) {
                    transform.localScale += new Vector3(0.01f, 0.01f, 1);
                } else {
                    transform.localScale += new Vector3(0.02f, 0.02f, 1);
                }
            }
        }
    }

    public void OnMouseEnter() {
        if (titleController.filesUninteractable || isBusy) { return; }
        titleController.HoverSound();
        previousText = GameObject.Find("SlotText").GetComponent<TextMeshPro>().text;
        if (titleController.selectedFile == 0) {    
            titleController.SetSlotText(titleController.slotDeleteRemindText);
        } else {
            titleController.SetSlotText(titleController.slotDeleteHoverText.Replace("#", titleController.selectedFile.ToString()));
        }
        GetComponent<SpriteRenderer>().sprite = hover;
        desiredScale = 0.9f;
    }

    public void OnMouseExit() {
        if (titleController.filesUninteractable || isBusy) { return; }
        titleController.SetSlotText(previousText);
        GetComponent<SpriteRenderer>().sprite = idle;
        desiredScale = 0.75f;
    }

    public void OnMouseDown() {
        if (titleController.filesUninteractable || isBusy || titleController.selectedFile == 0) { return; }
        isBusy = true;
        if (Input.GetMouseButtonDown(0)) {
            titleController.filesUninteractable = true;
            desiredScale = 0.75f;
            titleController.SetSlotText(titleController.slotDeleteText.Replace("#", titleController.selectedFile.ToString()));
            GetComponent<SpriteRenderer>().sprite = ongoing;
            StartCoroutine(WaitTillComplete());
        }
    }

    IEnumerator SlowPulse() {
        for (int i = 0; i < 10; i++) {
            float waitTime = 0.02f;
            for (int j = 0; j < 8; j++) {
                desiredScale += 0.015f;
                wfs = new WaitForSeconds(waitTime);
                yield return wfs;
                waitTime += 0.005f;
            }
            waitTime = 0.02f;
            for (int j = 0; j < 8; j++) {
                desiredScale -= 0.015f;
                wfs = new WaitForSeconds(waitTime);
                yield return wfs;
                waitTime += 0.005f;
            }
        }
        desiredScale = titleController.hoverFileScale;
    }

    IEnumerator WaitTillComplete() {
        titleController.DeletingSound();
        Coroutine pulse = StartCoroutine(SlowPulse());
        wfs = new WaitForSeconds(3);
        yield return wfs;
        titleController.SetSlotText(titleController.slotDeleteSuccessText.Replace("#", titleController.selectedFile.ToString()));
        GetComponent<SpriteRenderer>().sprite = complete;
        StopCoroutine(pulse);
        titleController.DeleteSuccessSound();
        wfs = new WaitForSeconds(2);
        yield return wfs;
        titleController.SetSlotText(previousText); // This needs to be changed later because it's the text before deletion
        GetComponent<SpriteRenderer>().sprite = idle;
        titleController.filesUninteractable = false;
        isBusy = false;
    }

}
