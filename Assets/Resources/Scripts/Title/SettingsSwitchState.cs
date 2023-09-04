using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSwitchState : MonoBehaviour
{
    
    private Animator anim;
    public bool isDefaultOn = false;
    public bool isOn = false;
    private TitleController titleController;

    public void Awake() {
        titleController = GameObject.Find("Controller").GetComponent<TitleController>();
        anim = GetComponent<Animator>();
        isOn = isDefaultOn;
        if (isOn) {
            anim.Play("SwitchOn");
        } else {
            anim.Play("SwitchOff");
        }
    }

    void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            AnimatorStateInfo a = anim.GetCurrentAnimatorStateInfo(0);
            if (isOn && a.length <= a.normalizedTime) {
                titleController.SettingsToggleOffSound();
                anim.Play("SwitchOff");
            } else if (!isOn && a.length <= a.normalizedTime) {
                titleController.SettingsToggleOnSound();
                anim.Play("SwitchOn");
            }
            isOn = !isOn;
        }
    }

}
