using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONCharacterFinder : MonoBehaviour
{

    public TextAsset characterJSON;
    public static JSONCharacterFinder instance;

    public void Awake() {
        instance = this;
    }

    public Ally GetAllyRef(string name) {
        Allies charactersInJson = JsonUtility.FromJson<Allies>(characterJSON.text);
        foreach (Ally character in charactersInJson.allies) {
            if (character.GetName() == name) {
                character.FullRecover();
                character.characterType = CharacterType.Ally;
                return character;
            }
        }
        Debug.Log("CHARACTER SPECIFIED IN GETALLYREF() NOT FOUND. JsonCharacterFinder.cs (" + name + ")");
        return null;
    }

    public Enemy GetEnemyRef(string name) {
        Enemies charactersInJson = JsonUtility.FromJson<Enemies>(characterJSON.text);
        foreach (Enemy character in charactersInJson.enemies) {
            if (character.GetName() == name) {
                character.FullRecover();
                character.characterType = CharacterType.Enemy;
                return character;
            }
        }
        Debug.Log("CHARACTER SPECIFIED IN GETENEMYREF() NOT FOUND. JsonCharacterFinder.cs (" + name + ")");
        return null;
    }
}
