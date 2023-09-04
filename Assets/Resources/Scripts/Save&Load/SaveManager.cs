using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{

    private static string savePath = Application.persistentDataPath + "/SaveInfo/";

    public static void Save(int fileNumber, SaveObject saveObject)
    {

        string fileName = "Save" + fileNumber.ToString() + ".ass";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            Debug.Log("Save file not found! Creating new file...");
        }

        File.WriteAllText(savePath + fileName, JsonUtility.ToJson(saveObject));
        Debug.Log("Slot saved into current file.");

    }

    public static void Load(int fileNumber)
    {

        string fileName = "Save" + fileNumber.ToString() + ".ass";

        Globals.currentFileNumber = fileNumber;

        if (!File.Exists(savePath + fileName))
        {
            Debug.Log("Save slot not found! Skipping load sequence!");
            return;
        }

        string savedText = File.ReadAllText(savePath + fileName);
        SaveObject so = JsonUtility.FromJson<SaveObject>(savedText);
        Debug.Log("Loading save slot (" + fileName + ").");
        string debugLoadSequence = "";

        if (so.keybinds != null)
        {
            Globals.keybinds = so.keybinds;
        }
        else
        {
            debugLoadSequence += " <Keybinds> ";
        }

        if (so.partyMembers != null)
        {
            Globals.partyMembers = so.partyMembers;
        }
        else
        {
            debugLoadSequence += " <Party Members> ";
        }

        if (so.inventory != null)
        {
            Globals.inventory = so.inventory;
        }
        else
        {
            debugLoadSequence += " <Inventory> ";
        }

        if (debugLoadSequence != "")
        {
            Debug.Log("Skipped loading (null values): [" + debugLoadSequence + "]");
        }

    }

}
