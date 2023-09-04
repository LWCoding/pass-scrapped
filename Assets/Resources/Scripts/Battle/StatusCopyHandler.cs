using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCopyHandler : MonoBehaviour
{

    private void Start() {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        WaitForSeconds wfs = new WaitForSeconds(0.025f);
        for (int i = 0; i < 40; i++) {
            sr.color -= new Color(0, 0, 0, 0.02f);
            transform.localScale += new Vector3(0.01f, 0.01f, 0);
            yield return wfs;
        }
        Destroy(gameObject);
    }

}
