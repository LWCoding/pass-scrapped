using System; // For Int32.Parse
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DialogueManager : MonoBehaviour
{
    
    private List<string> intValueProperties = new List<string> { "money", "partyCount" };
    private List<string> stringValueProperties = new List<string> { };

    /*
        This function takes in a string that may be passed by the Dialogue json
        in the `prereq` statement and evaluates it to true or false.

        If required, an & symbol can be appended to work with multiple conditions.

        Ex: money>50   money=50   partyCount=3

        VALID PROPERTIES: money, partyCount
        VALID OPERATORS: <, >, = 
    */
    private bool EvaluatePrerequisite(string s) {
        // Find which operator we're working with
        if (s.Contains(">")) {
            string[] split = s.Split('>');
            string key = split[0], value = split[1];
            // Assume the key and value are both integers because it's the
            // greater-than operator and I don't care about strings.
            return Int32.Parse(GetPropertyValue(key)) > Int32.Parse(value);
        }
        if (s.Contains("<")) {
            string[] split = s.Split('<');
            string key = split[0], value = split[1];
            // Assume the key and value are both integers because it's the
            // greater-than operator and I don't care about strings.
            return Int32.Parse(GetPropertyValue(key)) < Int32.Parse(value);
        }
        if (s.Contains("=")) {
            string[] split = s.Split('=');
            string key = split[0], value = split[1];
            // Check if it's comparing ints or strings depending on the list
            // declared at the top of this file.
            if (intValueProperties.Contains(key)) {
                return Int32.Parse(GetPropertyValue(key)) == Int32.Parse(value);
            } else {
                return GetPropertyValue(key) == value;
            }
        }
        Debug.Log("EVALUATEPREREQUISITE FOUND INVALID CONDITIONAL STRING (" + s + ") in FREEROAM > DIALOGUEMANAGER_EVAL.CS!");
        return false;
    }

    /*
        Retrieves the value of the provided property in the function above.
    */
    private string GetPropertyValue(string s) {
        switch (s) {
            case "money":
                return Globals.wallet.GetMoney().ToString();
            case "partyCount":
                return Globals.partyMembers.Count.ToString();
        }
        Debug.Log("GETPROPERTYVALUE FOUND INVALID PROPERTY (" + s + ") in FREEROAM > DIALOGUEMANAGER_EVAL.CS!");
        return "";
    }

}
