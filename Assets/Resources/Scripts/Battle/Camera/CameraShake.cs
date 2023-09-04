using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;
    [HideInInspector] public bool isShaking = false;
    private WaitForSeconds wfs;

    void Awake()
    {
        instance = this;
    }

    void Update() {
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame; 
    }

    public static void Shake (float duration, float amount, float timeBetweenShakes) {
        instance._originalPos = instance.gameObject.transform.localPosition;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(duration, amount, timeBetweenShakes));
    }

    public IEnumerator cShake (float duration, float amount, float timeBetweenShakes) {
        instance.isShaking = true;
        float endTime = Time.time + duration;
        while (duration > 0) {
            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;
            duration -= _fakeDelta + timeBetweenShakes;
            wfs = new WaitForSeconds(timeBetweenShakes);
            yield return wfs;
        }
        transform.localPosition = _originalPos;
        instance.isShaking = false;
    }

}
