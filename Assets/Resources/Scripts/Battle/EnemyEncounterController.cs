using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This script handles the encounter animations that happen at the beginning
// of a battle.

public class EnemyEncounterController : MonoBehaviour
{
    
    [SerializeField] private GameObject enemyContainerObject;
    [SerializeField] private GameObject warningObject;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Sprite dummyIntro;

    public void SetSprites(List<Enemy> enemies) {
        float accumDangerLevel = 0;
        for (int i = 0; i < enemies.Count; i++) {
            Enemy enemy = enemies[i];
            accumDangerLevel += enemy.dangerLevel;
            GameObject newEnemy = Instantiate(enemyPrefab);
            SpriteRenderer encounterRenderer = newEnemy.GetComponent<SpriteRenderer>();
            newEnemy.transform.SetParent(enemyContainerObject.transform);
            newEnemy.transform.position = new Vector3(-12, 2.4f, 0);
            newEnemy.transform.position += new Vector3(enemies.Count * 0.2f - i, 0, 0);
            encounterRenderer.color -= new Color(i * 0.2f, i * 0.2f, i * 0.2f, 0);
            encounterRenderer.sortingOrder = 2 + enemies.Count - i;
            switch (enemy.GetName()) {
                case "Dummy":
                    encounterRenderer.sprite = dummyIntro;
                    break;
            }
        }
        int dangerLevel = (int)(accumDangerLevel / enemies.Count);
        UpdateWarning(dangerLevel);
    }

    public void UpdateWarning(int dangerLevel) {
        SpriteRenderer warningRenderer = warningObject.GetComponent<SpriteRenderer>();
        TextMeshPro warningText = warningObject.transform.GetChild(0).GetComponent<TextMeshPro>();
        Color32 warningColor = new Color32();
        if (dangerLevel <= 5) {
            warningColor = new Color32((byte)(50 + 205 * (dangerLevel / 10f)), 
                                        (byte)(150 + 105 * (1 - (dangerLevel / 10f))), 
                                        0, 255);
        } else {
            warningColor = new Color32((byte)(150 + 105 * (dangerLevel / 10f)), 
                                        (byte)(50 + 205 * (1 - (dangerLevel / 10f))), 
                                        0, 255);
        }
        warningRenderer.color = warningColor;
        warningText.text = dangerLevel.ToString();
        warningText.color = warningColor;
    }

}
