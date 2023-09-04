using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueList {

    public List<DialogueInstance> rawDialogueList;
    public string initialLeftSprite = "";
    public string initialRightSprite = "";
    public string prereq = "";
    public bool loop = false;
    public int elsePrereq = -1;
    public int id;
    public int next;

    public bool HasPrerequisite() {
        return prereq != "";
    }

    public bool HasSprites() {
        return initialLeftSprite != "" && initialRightSprite != "";
    }
    
}