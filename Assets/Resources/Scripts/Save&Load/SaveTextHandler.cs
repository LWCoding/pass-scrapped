using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveTextHandler : MonoBehaviour
{
    
    [SerializeField] private TextMeshPro saveText;
    private string[] loadingStages = {"Saving   ", "Saving.  ", "Saving.. ", "Saving..." };
    public static SaveTextHandler saveTextHandler = null;

    private void Start() {
        if (GameObject.FindGameObjectsWithTag("SaveText").Length == 2) {
            Destroy(this.gameObject);
            return;
        }
        GetComponent<Canvas>().worldCamera = Camera.main;
        SaveTextHandler.saveTextHandler = GetComponent<SaveTextHandler>();
        StartCoroutine(UpdateSaveText(0));
    }

    private IEnumerator UpdateSaveText(int currentStage) {
        if (currentStage >= loadingStages.Length) {
            currentStage = 0;
        }
        saveText.text = loadingStages[currentStage];
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(UpdateSaveText(currentStage + 1));
    }

    public void Destroy() {
        SaveTextHandler.saveTextHandler = null;
        Destroy(this.gameObject);
    }

}
