using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueInstance
{

    public int id; // Unique # for each dialogue
    public int next; // # for next dialogue
    public string name; // Name of character speaking
    public string contents; // Text of character speaking
    public string leftSprite = ""; // Sprite name for dialogue image
    public string rightSprite = ""; // Sprite name for dialogue image
    public string active = ""; // If `left` or `right` image is talking
    public string oneShot = ""; // Don't stop animation until done for `left` or `right`

    // If `options` is filled up, do NOT go to next, but wait for player to 
    // select an option
    public List<DialogueOption> options = new List<DialogueOption>();

    public bool HasOptions()
    {
        return options.Count > 0;
    }

    public bool HasLeftSprite()
    {
        return leftSprite != "";
    }

    public bool HasRightSprite()
    {
        return rightSprite != "";
    }

    public bool HasActiveSpecifier()
    {
        return active != "";
    }

    public bool HasOneShot()
    {
        return oneShot != "";
    }

}
