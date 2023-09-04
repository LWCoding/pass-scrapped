using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueContainer {
    
    // Stores all dialogue over multiple trials of interacting with the character.
    public List<DialogueList> dialogues = new List<DialogueList>();

    public string dialogueName; // For DialogueManager.cs to locate current save data

}
