using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBarHandler : MonoBehaviour
{

    public float maxTransformValue;
    private float currValue;
    private float nextValue;
    private SpriteRenderer sr;
    private float delayBetweenCharge = 0.6f;
    [SerializeField] private Animator orbAnimator;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        InitializeBarValue();
        AddBarValue(10);
        sr.color = new Color(1, 0, 0);
    }

    /*
        Takes in a current value and max value and sets the progress bar depending on maxTransformValue.
    */
    public void InitializeBarValue()
    {
        currValue = 0;
        nextValue = 10;
        Vector3 currScale = transform.localScale;
        float currPercent = (currValue / nextValue) * maxTransformValue;
        transform.localScale = new Vector3(currPercent, currScale.y, currScale.z);
    }

    /*
        Takes in an added value and animates an increase the progress bar. 
        Speed is an optional param set to 1 for 100%.
    */
    public void AddBarValue(float addedValue)
    {
        StartCoroutine(BarIncrease(addedValue));
    }

    public IEnumerator BarIncrease(float added)
    {
        bool endOfBar = false; // Whether or not the bar became full.
        currValue += added;
        transform.localScale += new Vector3(0.1f * maxTransformValue, 0, 0);
        sr.color += new Color(-0.08f, 0.08f, 0);
        if (transform.localScale.x > maxTransformValue)
        {
            Vector3 currScale = transform.localScale;
            transform.localScale = new Vector3(maxTransformValue, currScale.y, currScale.z);
            endOfBar = true;
        }
        yield return new WaitForSeconds(delayBetweenCharge);
        // Handle if bar exceeded max
        if (endOfBar)
        {
            orbAnimator.Play("Pulse");
            yield return new WaitForSeconds(0.8f);
            orbAnimator.Play("Idle");
            Vector3 currScale = transform.localScale;
            transform.localScale = new Vector3(0, currScale.y, currScale.z);
            currValue = 0;
            sr.color = new Color(1, 0, 0);
            // TODO: Animate orb shoot here instead of waiting!
            yield return new WaitForSeconds(1.5f);
        }
        StartCoroutine(BarIncrease(10));
    }

}
