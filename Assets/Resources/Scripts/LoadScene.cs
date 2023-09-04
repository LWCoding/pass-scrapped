using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadScene : MonoBehaviour
{

    private GameObject bgMask;
    private GameObject loadingBar;
    private GameObject tipText;
    private GameObject cog;
    private GameObject fadeSprite;
    public static LoadScene instance;

    [Header("Assign Tips File")]
    public TextAsset tipsFile;
    [Header("Assign BG Image (Next Scene)")]
    public Sprite bgImage;
    private float delayBeforeFade = 0.9f;
    private WaitForSeconds wfs;

    private void SetTipText()
    {
        string[] tipsText = tipsFile.text.Split('\n');
        tipText.GetComponent<TextMeshPro>().text = "Tip: " + tipsText[Random.Range(0, tipsText.Length)];
    }

    public void Awake()
    {
        instance = GetComponent<LoadScene>();
        bgMask = GameObject.Find("BGMask");
        loadingBar = GameObject.Find("LoadingBar");
        tipText = GameObject.Find("TipText");
        cog = GameObject.Find("Cog");
        fadeSprite = GameObject.Find("FadeSprite");
    }

    public void Start()
    {
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("LoadingUI"))
        {
            Color c = ui.GetComponent<SpriteRenderer>().color;
            ui.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0);
        }
        GameObject.Find("LoadBG").GetComponent<SpriteRenderer>().sprite = bgImage;
        bgMask.transform.localScale = new Vector3(4.1f, 4.1f, 1);
        tipText.GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 0);
        fadeSprite.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f, 0);
        SetTipText();
        // Set them active so masks don't interfere with each other!
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartLoadScreen()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        StartCoroutine(WidenMask());
    }

    IEnumerator WidenMask()
    {
        float waitTime = 0.01f;
        while (bgMask.transform.localScale.x > 0)
        {
            bgMask.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
            if (bgMask.transform.localScale.x < 0)
            {
                bgMask.transform.localScale = new Vector3(0, 0, 1);
            }
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.0005f;
        }
        GameObject[] uiItems = GameObject.FindGameObjectsWithTag("LoadingUI");
        for (int i = 0; i < 25; i++)
        {
            foreach (GameObject ui in uiItems)
            {
                if (ui.gameObject.name != "BarBG")
                {
                    ui.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.04f);
                }
                else if (ui.GetComponent<SpriteRenderer>().color.a < 0.3f)
                {
                    ui.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.04f);
                }
            }
            tipText.GetComponent<TextMeshPro>().color += new Color(0, 0, 0, 0.04f);
            wfs = new WaitForSeconds(0.02f);
            yield return wfs;
        }
    }

    void FixedUpdate()
    {
        if (loadingBar.GetComponent<SpriteRenderer>().color.a == 0) { return; }
        if (loadingBar.transform.localScale.x < 1)
        {
            loadingBar.transform.localScale += new Vector3(0.005f, 0, 0);
            cog.transform.Rotate(new Vector3(0, 0, -5));
            if (loadingBar.transform.localScale.x > 1)
            {
                loadingBar.transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine(StartFade());
            }
        }
        else
        {
            cog.transform.Rotate(new Vector3(0, 0, -2));
        }
    }

    IEnumerator StartFade()
    {
        wfs = new WaitForSeconds(delayBeforeFade);
        yield return wfs;
        float waitTime = 0.01f;
        while (fadeSprite.GetComponent<SpriteRenderer>().color.a < 1)
        {
            fadeSprite.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.05f);
            wfs = new WaitForSeconds(waitTime);
            yield return wfs;
            waitTime += 0.0002f;
        }
    }

}
