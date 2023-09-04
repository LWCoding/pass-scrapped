using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleController : MonoBehaviour
{

    [Header("Audio Assignments")]
    public AudioClip buttonSelectSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip trashDeletingSFX;
    public AudioClip trashDeletedSFX;
    public AudioClip fileAppearSFX;
    public AudioClip panelOpenSFX;
    public AudioClip panelCloseSFX;
    public AudioClip closeMenuSFX;
    public AudioClip settingsToggleOn;
    public AudioClip settingsToggleOff;
    public AudioClip titleLoop;
    [HideInInspector] public AudioSource audioSource;

    [Header("Button Assignments")]
    public GameObject playButton;
    public GameObject achievementsButton;
    public GameObject optionsButton;
    public float initialButtonOpacity = 0.6f;
    public float initialButtonScale = 0.85f;
    public float hoverButtonScale = 0.95f;
    [Header("Slot Assignments")]
    public GameObject slotUI; // Just so I can hide it during the inspector
    public GameObject holoFiles;
    public GameObject holoBox;
    public GameObject holoBG;
    public GameObject holoBack;
    public GameObject holoBin;
    public GameObject slotText;
    public float initialFileOpacity = 0.9f;
    public float initialFileScale = 0.65f;
    public float hoverFileScale = 0.75f;
    [Header("Settings Assignments")]
    public GameObject settingsUI; // Just so I can hide it during the inspector
    public GameObject settingsBack;
    [Header("Misc Assignments")]
    public GameObject titleBG;
    public string slotDefaultText = "Click on an empty file to display its information.";
    public string slotSelectText = "Delicious. This is file number #.";
    public string slotDeleteText = "Please wait... Deleting all data for file number #.";
    public string slotDeleteSuccessText = "File number # was successfully wiped!";
    public string slotDeleteHoverText = "Click to permanently delete all data for file number #.";
    public string slotDeleteRemindText = "Select a file and click this button to delete all of its data.";
    [HideInInspector] public bool buttonsUninteractable = true;
    [HideInInspector] public bool filesUninteractable = false;
    [HideInInspector] public int selectedFile = 0; // For the slot-choosing screen
    private float initialBGColor = 0.9f;
    private float holoFileDelay = 0.2f;
    private float targetSettingsPositionY = 0.3f;
    private WaitForSeconds wfs;

    public void Awake() {
        Screen.SetResolution(1280, 720, false); // Set 16:9 on windowed mode 
        slotUI.SetActive(true);
        settingsUI.transform.position = new Vector3(settingsUI.transform.position.x, -100, 0);
        settingsUI.SetActive(true); // To load switch states before setting it to false
        holoBG.SetActive(false);
        holoBack.SetActive(false);
        GameObject.Find("SlotText").GetComponent<TextMeshPro>().text = slotDefaultText;
        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        audioSource.loop = true;
        selectedFile = 0;
    }

    public void Start() {
        audioSource.clip = titleLoop;
        audioSource.Play();
        foreach (Transform t in holoFiles.transform) {
            t.gameObject.SetActive(false);
        }
        slotText.GetComponent<TextMeshPro>().color = new Color(0.9f, 0.9f, 0.9f, 0);
        holoBox.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f, 0);
        holoBin.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f, 0);
        titleBG.GetComponent<SpriteRenderer>().color = new Color(initialBGColor, initialBGColor, initialBGColor, 1);
        playButton.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, initialButtonOpacity);
        achievementsButton.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, initialButtonOpacity);
        optionsButton.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, initialButtonOpacity);
        StartCoroutine(ShowButtons());
    }

    public void SelectSound() {
        audioSource.PlayOneShot(buttonSelectSFX, 0.9f);
    }

    public void HoverSound() {
        audioSource.PlayOneShot(buttonHoverSFX, 0.8f);
    }

    public void FileAppearSound() {
        audioSource.PlayOneShot(fileAppearSFX, 0.5f);
    }

    public void DeletingSound() {
        audioSource.PlayOneShot(trashDeletingSFX, 0.7f);
    }
    
    public void DeleteSuccessSound() {
        audioSource.PlayOneShot(trashDeletedSFX, 0.9f);
    }

    public void SettingsAppearSound() {
        audioSource.PlayOneShot(panelOpenSFX, 0.6f);
    }

    public void SettingsCloseSound() {
        audioSource.PlayOneShot(panelCloseSFX, 0.6f);
    }

    public void MenuCloseSound() {
        audioSource.PlayOneShot(closeMenuSFX, 0.7f);
    }
    public void SettingsToggleOnSound() {
        audioSource.PlayOneShot(settingsToggleOn, 0.7f);
    }
    public void SettingsToggleOffSound() {
        audioSource.PlayOneShot(settingsToggleOff, 0.7f);
    }

    public IEnumerator ShowButtons(float delayBefore = 0) {
        selectedFile = 0;
        buttonsUninteractable = true;
        filesUninteractable = true;
        slotText.GetComponent<TextMeshPro>().text = slotDefaultText;
        playButton.GetComponent<ButtonStates>().ResetButton();
        achievementsButton.GetComponent<ButtonStates>().ResetButton();
        optionsButton.GetComponent<ButtonStates>().ResetButton();
        if (delayBefore != 0) {
            wfs = new WaitForSeconds(delayBefore);
            yield return wfs;
        }
        playButton.SetActive(true);
        playButton.GetComponent<Animator>().Play("Show");
        wfs = new WaitForSeconds(0.5f);
        yield return wfs;
        achievementsButton.SetActive(true);
        achievementsButton.GetComponent<Animator>().Play("Show");
        wfs = new WaitForSeconds(0.5f);
        yield return wfs;
        optionsButton.SetActive(true);
        optionsButton.GetComponent<Animator>().Play("Show");
        wfs = new WaitForSeconds(0.7f);
        yield return wfs;
        buttonsUninteractable = false;
        filesUninteractable = false;
    }

    public void DarkenBG() {
        StartCoroutine(DarkenBGCoroutine());
    }

    IEnumerator DarkenBGCoroutine() {
        wfs = new WaitForSeconds(0.2f);
        yield return wfs;
        float waitTime = 0.03f;
        while (titleBG.GetComponent<SpriteRenderer>().color.r > 0.5f) {
            titleBG.GetComponent<SpriteRenderer>().color -= new Color(0.05f, 0.05f, 0.05f, 0);
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.003f;
        }
    }

    IEnumerator UndarkenBGCoroutine() {
        wfs = new WaitForSeconds(0.6f);
        yield return wfs;
        float waitTime = 0.03f;
        while (titleBG.GetComponent<SpriteRenderer>().color.r < 1) {
            titleBG.GetComponent<SpriteRenderer>().color += new Color(0.05f, 0.05f, 0.05f, 0);
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.003f;
        }
    }

    public void ShowSlotAssets() {
        StartCoroutine(ShowHoloAssetsCoroutine());
        StartCoroutine(ShowSlotFilesCoroutine());
    }

    public void HideSlotAssets() {
        StartCoroutine(HideHoloAssetsCoroutine());
        StartCoroutine(HideSlotFilesCoroutine());
        StartCoroutine(UndarkenBGCoroutine());
        StartCoroutine(ShowButtons(1));
    }

    public void ShowSettingsAssets() {
        StartCoroutine(ShowSettingsAssetsCoroutine());
    }

    public void HideSettingsAssets() {
        StartCoroutine(HideSettingsAssetsCoroutine());
        StartCoroutine(UndarkenBGCoroutine());
        StartCoroutine(ShowButtons(1));
    }

    IEnumerator ShowSettingsAssetsCoroutine() {
        settingsUI.transform.position = new Vector3(settingsUI.transform.position.x, targetSettingsPositionY, 0);
        settingsUI.SetActive(true);
        settingsUI.transform.position -= new Vector3(0, 10, 0);
        SettingsAppearSound();
        wfs = new WaitForSeconds(0.1f);
        yield return wfs;
        float delayTime = 0.008f;
        for (int i = 0; i < 20; i++) {
            settingsUI.transform.position += new Vector3(0, 0.5f, 0);
            wfs = new WaitForSeconds(delayTime);
            yield return wfs;
            delayTime /= (13.0f / 14.0f);
        }
        settingsUI.transform.position = new Vector3(settingsUI.transform.position.x, targetSettingsPositionY, 0);
    }

    IEnumerator HideSettingsAssetsCoroutine() {
        float delayTime = 0.03f;
        wfs = new WaitForSeconds(0.1f);
        yield return wfs;
        for (int i = 0; i < 20; i++) {
            settingsUI.transform.position -= new Vector3(0, 0.5f, 0);
            wfs = new WaitForSeconds(delayTime);
            yield return wfs;
            delayTime *= (13.0f / 14.0f);
        }
        settingsBack.GetComponent<SettingsBack>().ResetStats();
        settingsUI.transform.position = new Vector3(settingsUI.transform.position.x, targetSettingsPositionY, 0);
        settingsUI.SetActive(false);
        SettingsCloseSound();
    }

    IEnumerator ShowHoloAssetsCoroutine() {
        wfs = new WaitForSeconds(0.2f);
        yield return wfs;
        holoBG.SetActive(true);
        holoBG.GetComponent<Animator>().Play("Show");
        wfs = new WaitForSeconds(0.4f);
        yield return wfs;
        for (int i = 0; i < 10; i++) {
            slotText.GetComponent<TextMeshPro>().color += new Color(0, 0, 0, 0.1f);
            holoBox.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.1f);
            holoBin.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.1f);
            wfs = new WaitForSeconds(0.05f);
            yield return wfs;
        }
        wfs = new WaitForSeconds(0.4f);
        yield return wfs;
        holoBack.SetActive(true);
        holoBack.GetComponent<Animator>().Play("Show");
    }

    IEnumerator ShowSlotFilesCoroutine() {
        wfs = new WaitForSeconds(0.6f);
        yield return wfs;
        int currFile = 1;
        foreach (Transform fileTransform in holoFiles.transform) {
            GameObject file = fileTransform.gameObject;
            file.SetActive(true);
            Animator anim = file.GetComponent<Animator>();
            anim.Play("FileShow" + currFile);
            currFile++;
            FileAppearSound();
            wfs = new WaitForSeconds(holoFileDelay);
            yield return wfs;
        }
    }

    IEnumerator HideHoloAssetsCoroutine() {
        wfs = new WaitForSeconds(0.2f);
        yield return wfs;
        for (int i = 0; i < 10; i++) {
            slotText.GetComponent<TextMeshPro>().color -= new Color(0, 0, 0, 0.1f);
            holoBox.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.1f);
            holoBin.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.1f);
            wfs = new WaitForSeconds(0.05f);
            yield return wfs;
        }
        holoBack.GetComponent<Animator>().Play("Hide");
        wfs = new WaitForSeconds(0.35f);
        yield return wfs;
        holoBack.SetActive(false);
        holoBG.GetComponent<Animator>().Play("Hide");
        wfs = new WaitForSeconds(0.35f);
        yield return wfs;
        holoBG.SetActive(false);
    }

    IEnumerator HideSlotFilesCoroutine() {
        wfs = new WaitForSeconds(0.2f);
        yield return wfs;
        int currFile = 5;
        for (int i = holoFiles.transform.childCount - 1; i >= 0; i--) {
            GameObject file = holoFiles.transform.GetChild(i).gameObject;
            Animator anim = file.GetComponent<Animator>();
            anim.Play("FileHide" + currFile);
            StartCoroutine(file.GetComponent<SlotFileHover>().Disappear());
            currFile--;
            wfs = new WaitForSeconds(holoFileDelay / 2);
            yield return wfs;
        }
    }

    public void UpdateSlots() {
        foreach (Transform t in holoFiles.transform) {
            t.gameObject.GetComponent<SlotFileHover>().UpdateState();
        }
    }

    public void SetSlotText(string text) {
        slotText.GetComponent<TextMeshPro>().text = text;
    }

}
