using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDeathCounterHandler : MonoBehaviour
{
    
    public GameObject fillObject;
    public GameObject healthIconObject;
    public GameObject baseObject;
    public GameObject bgObject;

    public void Awake() {
        baseObject.SetActive(false);
        healthIconObject.SetActive(false);
        fillObject.SetActive(false);
        bgObject.SetActive(false);
    }

    public IEnumerator Show() {
        baseObject.SetActive(true);
        Animator baseAnim = baseObject.GetComponent<Animator>();
        baseObject.GetComponent<Animator>().Play("DeathFrameShow");
        yield return new WaitForSeconds(0.2f);
        bgObject.SetActive(true);
        healthIconObject.SetActive(true);
        fillObject.SetActive(true);
        fillObject.GetComponent<Image>().fillAmount = 100;
        healthIconObject.transform.localScale = new Vector3(0, 0, 0);
        fillObject.transform.localScale = new Vector3(0, 0, 0);
        WaitForSeconds wfs = new WaitForSeconds(0.008f);
        for (int i = 0; i < 25; i++) {
            healthIconObject.transform.localScale += new Vector3(0.04f, 0.04f, 0);
            fillObject.transform.localScale += new Vector3(0.04f, 0.04f, 0);
            yield return wfs;
        }
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator StartCountdown(float timeInSeconds) {
        float waitTime = timeInSeconds / 100;
        WaitForSeconds wfs = new WaitForSeconds(waitTime);
        for (int i = 0; i < 100; i++) {
            fillObject.GetComponent<Image>().fillAmount -= 0.01f;
            yield return wfs;
        }
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    public bool isPlaying(Animator anim, string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

}
