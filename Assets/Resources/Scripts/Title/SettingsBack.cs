using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBack : MonoBehaviour
{
    
    private TitleController titleController;
    [Header("Sprite Assignments")]
    public Sprite idle;
    public Sprite active;
    private bool isBusy = false;
    private float desiredScale = 1;

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
        if (isBusy) { return; }
        titleController.HoverSound();
        desiredScale = 1.1f;
    }

    public void OnMouseExit() {
        if (isBusy) { return; }
        desiredScale = 1;
    }

    public void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            isBusy = true;
            titleController.HideSettingsAssets();
            desiredScale = 0.9f;
            GetComponent<SpriteRenderer>().sprite = active;
        }
    }

    // Should be called before SetActive(false) is called
    public void ResetStats() {
        GetComponent<SpriteRenderer>().sprite = idle;
        desiredScale = 1;
        isBusy = false;
    }

}
