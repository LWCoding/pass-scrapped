using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Globals {
    
    // Regarding player states & current game save
    public static AudioSource audioSource;
    public static Keybinds keybinds = new Keybinds();
    public static List<Ally> partyMembers = new List<Ally>();
    public static List<Enemy> battleEnemies = new List<Enemy>();
    public static Inventory inventory = new Inventory();
    public static Wallet wallet = new Wallet();
    public static int currentFileNumber; // To identify what file to save to
    public static DictStringInt dialogueSaves = new DictStringInt();
    // Regarding open world status
    public static bool canPlayerMove = true;

    // true -> Frozen player ; false -> Moveable player
    // Also updates animation of players.
    public static void ToggleFreezePlayer(bool shouldFreeze) {
        canPlayerMove = !shouldFreeze;
        PlayerController.instance.ForceStopMoving();
    }

    public static void SaveGame() {
        SaveObject saveObject = new SaveObject();
        saveObject.wallet = wallet;
        saveObject.keybinds = keybinds;
        saveObject.partyMembers = partyMembers;
        saveObject.inventory = inventory;
        saveObject.dialogueSaves = dialogueSaves;
        SaveManager.Save(currentFileNumber, saveObject);
    }

    public static void LoadGame(int fileNumber) {
        AudioListener.volume = 0.3f;
        SaveManager.Load(fileNumber);
        if (partyMembers.Count == 0) {
            LoadNewSaveSlot();
        }
    }

    public static void LoadNewSaveSlot() {
        Ally ally = JSONCharacterFinder.instance.GetAllyRef("Jack");
        partyMembers.Add(ally);
    }

    public static List<int> GetAliveEnemies() {
        List<int> aliveEnemies = new List<int>();
        int x = 0;
        foreach (Enemy enemy in battleEnemies) {
            if (enemy.GetStatus() == CharacterStatus.Alive) {
                aliveEnemies.Add(x);
            }
            x++;
        }
        return aliveEnemies;
    }

    public static List<int> GetAliveAllies() {
        List<int> aliveAllies = new List<int>();
        int x = 0;
        foreach (Ally ally in partyMembers) {
            if (ally.GetStatus() == CharacterStatus.Alive) {
                aliveAllies.Add(x);
            }
            x++;
        }
        return aliveAllies;
    }

    public static Ally GetAlly(int allyNum) {
        if (allyNum < 0 || allyNum >= partyMembers.Count) {
            Debug.Log("INVALID CALL TO GETALLY() in GLOBALS.CS! " + allyNum.ToString());
            return null;
        }
        return partyMembers[allyNum];
    }

    public static Enemy GetEnemy(int enemyNum) {
        if (enemyNum < 0 || enemyNum >= battleEnemies.Count) {
            Debug.Log("INVALID CALL TO GETENEMY() in GLOBALS.CS!");
            return null;
        }
        return battleEnemies[enemyNum];
    }

    public static int PickRandomAliveAlly() {
        List<int> aliveAllies = GetAliveAllies();
        if (aliveAllies.Count == 0) {
            Debug.Log("NO ALLIES TO CHOOSE FROM IN GLOBALS.CS!");
            return -1;
        }
        int randomAllyNum = aliveAllies[Random.Range(0, aliveAllies.Count)];
        return randomAllyNum;
    }

    public static int PickRandomAliveEnemy() {
        List<int> aliveEnemies = GetAliveEnemies();
        if (aliveEnemies.Count == 0) {
            Debug.Log("NO ENEMIES TO CHOOSE FROM IN GLOBALS.CS!");
            return -1;
        }
        int randomEnemyNum = aliveEnemies[Random.Range(0, aliveEnemies.Count)];
        return randomEnemyNum;
    }

    public static bool IsBossBattle() {
        foreach (Enemy e in battleEnemies) {
            if (e.isBoss) {
                return true;
            }
        }
        return false;
    }

    public static bool IsInParty(string name) {
        foreach (Ally a in partyMembers) {
            if (a.GetName() == name) {
                return true;
            }
        }
        return false;
    }

}

[System.Serializable]
public class Keybinds {
    public KeyCode leftMoveKeycode = KeyCode.LeftArrow;
    public KeyCode upMoveKeycode = KeyCode.UpArrow;
    public KeyCode rightMoveKeycode = KeyCode.RightArrow; 
    public KeyCode downMoveKeycode = KeyCode.DownArrow;
    public KeyCode altLeftMoveKeycode = KeyCode.A; // ALT
    public KeyCode altUpMoveKeycode = KeyCode.W; // ALT
    public KeyCode altRightMoveKeycode = KeyCode.D; // ALT
    public KeyCode altDownMoveKeycode = KeyCode.S; // ALT
    public KeyCode runToggleKeycode = KeyCode.LeftShift;
    public KeyCode nextButtonKeycode = KeyCode.RightArrow;
    public KeyCode prevButtonKeycode = KeyCode.LeftArrow;
    public KeyCode downButtonKeycode = KeyCode.DownArrow;
    public KeyCode upButtonKeycode = KeyCode.UpArrow;
    public KeyCode altNextButtonKeycode = KeyCode.D; // ALT
    public KeyCode altPrevButtonKeycode = KeyCode.A; // ALT
    public KeyCode altDownButtonKeycode = KeyCode.S; // ALT
    public KeyCode altUpButtonKeycode = KeyCode.W; // ALT
    public KeyCode escapeButtonKeycode = KeyCode.Escape;
    public KeyCode selectKeycode = KeyCode.Space;
    public KeyCode interactKeycode = KeyCode.E;
}