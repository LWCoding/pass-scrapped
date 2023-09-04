using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoBoxHandler : MonoBehaviour
{
    
    [SerializeField] private GameObject nameTextObject;
    [SerializeField] private GameObject healthBarRealObject;
    [SerializeField] private GameObject energyBarRealObject;
    [SerializeField] private GameObject healthBarCosmeticObject;
    [SerializeField] private GameObject energyBarCosmeticObject;
    [SerializeField] private GameObject healthBarText;
    [SerializeField] private GameObject energyBarText;
    [SerializeField] private GameObject barContentParentObject;
    [SerializeField] private GameObject uiDeathTimerPrefab;
    [SerializeField] private GameObject bgObject;
    [SerializeField] private float maxBarScale;
    private Vector2 originalBarScale;
    private int maxHealth;
    public Character characterRef;
    [SerializeField] private int currentHealth;
    [SerializeField] private int perceivedHealth;
    private GameObject copyOfDeathTimer;
    private int maxEnergy;
    private int currentEnergy;
    private int perceivedEnergy;
    private int healthToHeal = 0;
    private int energyToIncrease = 0;
    private float delayTimeBeforeDeath = 12; // in seconds
    private bool coroutineActive = false;
    private Animator anim;

    public void Awake() {
        originalBarScale = healthBarCosmeticObject.transform.localScale;
        anim = bgObject.GetComponent<Animator>();
    }

    public void SetCharacterReference(Ally a) {
        nameTextObject.GetComponent<TextMeshPro>().text = a.GetRawName();
        maxHealth = a.GetMaxHealth();
        currentHealth = a.currentHealth;
        maxEnergy = a.GetMaxEnergy();
        currentEnergy = a.currentEnergy;
        perceivedEnergy = currentEnergy;
        perceivedHealth = a.currentHealth;
        SetHealth(currentHealth);
        SetEnergy(currentEnergy);
        UpdateVisuals(true, true);
        UpdateHealthCosmeticVisual();
        UpdateEnergyCosmeticVisual();
    }

    private float GetHealthPercentage() {
        if (currentHealth < 0) return 0;
        return ((float)currentHealth / maxHealth);
    }

    private float GetEnergyPercentage() {
        return ((float)currentEnergy / maxEnergy);
    }

    private float GetPerceivedHealthPercentage() {
        return ((float)perceivedHealth / maxHealth);
    }

    private float GetPerceivedEnergyPercentage() {
        return ((float)perceivedEnergy / maxEnergy);
    }

    private void UpdateHealthCosmeticVisual() {
        healthBarCosmeticObject.transform.localScale = new Vector3(GetPerceivedHealthPercentage() * maxBarScale, originalBarScale.y, 1);
    }

    private void UpdateEnergyCosmeticVisual() {
        energyBarCosmeticObject.transform.localScale = new Vector3(GetPerceivedEnergyPercentage() * maxBarScale, originalBarScale.y, 1);
    }

    private void UpdateVisuals(bool updateHealth, bool updateEnergy) {
        characterRef.currentHealth = perceivedHealth;
        if (updateHealth) {
            healthBarRealObject.transform.localScale = new Vector3(GetHealthPercentage() * maxBarScale, originalBarScale.y, 1);
            healthBarText.GetComponent<TextMeshPro>().text = perceivedHealth + "/" + maxHealth; 
        }
        if (updateEnergy) {
            energyBarRealObject.transform.localScale = new Vector3(GetEnergyPercentage() * maxBarScale, originalBarScale.y, 1);
            energyBarText.GetComponent<TextMeshPro>().text = currentEnergy + "/" + maxEnergy; 
        }
    }

    IEnumerator GradualHealthReduceCoroutine() {
        if (perceivedHealth <= currentHealth) {
            coroutineActive = false;
        }
        if (perceivedHealth > currentHealth) {
            perceivedHealth--;
        } else {
            yield break;
        }
        UpdateHealthCosmeticVisual();
        UpdateVisuals(true, false);
        if (perceivedHealth == 0) {
            characterRef.spriteObject.GetComponent<BattleCharacterController>().BecomeUnconscious();
            if (Globals.GetAliveAllies().Count == 0) {
                BattleController.battleController.GameOver();
            }
            yield break;
        }
        float changeLeft = (float)Mathf.Abs(currentHealth - perceivedHealth) / maxHealth;
        if (maxHealth > 100) {
            // 0.106x2−0.238x+0.173
            yield return new WaitForSeconds(0.106f * Mathf.Pow(changeLeft, 2) - 0.238f * changeLeft + 0.223f);
        } else {
            yield return new WaitForSeconds(0.400f * Mathf.Pow(changeLeft, 2) - 0.238f * changeLeft + 0.450f);
        }
        StartCoroutine(GradualHealthReduceCoroutine());
    }

    IEnumerator GradualEnergyReduceCoroutine() {
        if (perceivedEnergy > currentEnergy) {
            perceivedEnergy--;
        } else {
            yield break;
        }
        UpdateEnergyCosmeticVisual();
        yield return new WaitForSeconds(0.008f);
        StartCoroutine(GradualEnergyReduceCoroutine());
    }

    IEnumerator GradualHealthIncreaseCoroutine() {
        if (healthToHeal == 0 || currentHealth >= maxHealth) {
            yield break;
        }
        healthToHeal--;
        currentHealth++;
        if (perceivedHealth < maxHealth) {
            perceivedHealth++;
            UpdateHealthCosmeticVisual();
        }

        UpdateVisuals(true, false);
        float changeLeft = Mathf.Max(0.15f, Mathf.Min(0.25f, ((float)healthToHeal / maxHealth)));
        yield return new WaitForSeconds(0.016f / changeLeft);
        StartCoroutine(GradualHealthIncreaseCoroutine());
    }

    IEnumerator GradualEnergyIncreaseCoroutine() {
        if (energyToIncrease == 0 || currentEnergy > maxEnergy) {
            yield break;
        }
        energyToIncrease--;
        currentEnergy++;
        if (perceivedEnergy < maxEnergy) {
            perceivedEnergy++;
            UpdateEnergyCosmeticVisual();
        }

        UpdateVisuals(false, true);
        yield return new WaitForSeconds(0.016f);
        StartCoroutine(GradualEnergyIncreaseCoroutine());
    }

    public void SetHealth(int value) {
        currentHealth = value;
        if (currentHealth < 0) {
            currentHealth = 0;
        }
        UpdateVisuals(true, false);
    }

    public void SetEnergy(int value) {
        currentEnergy = value;
        UpdateVisuals(false, true);
    }

    public void ChangeHealth(int value) {
        if (value > 0) {
            healthToHeal += value;
            StartCoroutine(GradualHealthIncreaseCoroutine());
        } else if (value < 0) {
            SetHealth(currentHealth + value);
            if (!coroutineActive) {
                coroutineActive = true;
                StartCoroutine(GradualHealthReduceCoroutine());
            }
        }
        // UpdateCosmeticVisuals();
        // UpdateVisuals(true, false);
    }

    public void PreviewEnergyLoss(int value) {
        EndPreviewEnergyLoss();
        perceivedEnergy = currentEnergy;
        currentEnergy = perceivedEnergy - value;
        UpdateEnergyCosmeticVisual();
        UpdateVisuals(false, true);
    }

    public void EndPreviewEnergyLoss() {
        currentEnergy = perceivedEnergy;
        UpdateEnergyCosmeticVisual();
        UpdateVisuals(false, true);
    }

    public void ChangeEnergy(int value) {
        perceivedEnergy = currentEnergy;
        currentEnergy += value;
        UpdateVisuals(false, true);
        if (value < 0) {
            StartCoroutine(GradualEnergyReduceCoroutine());
        } else {
            energyToIncrease += value;
            StartCoroutine(GradualEnergyIncreaseCoroutine());
        }
    }

    public IEnumerator DeathTimerCoroutine() {
        barContentParentObject.SetActive(false);
        yield return SpawnDeathTimer();
    }

    private IEnumerator SpawnDeathTimer() {
        GameObject deathTimer = Instantiate(uiDeathTimerPrefab);
        deathTimer.transform.SetParent(GameObject.Find("DeathTimerParent").transform);
        deathTimer.transform.position = Camera.main.WorldToScreenPoint(transform.position) - new Vector3(0, 10.5f, 0);
        copyOfDeathTimer = deathTimer;
        yield return deathTimer.GetComponent<BattleDeathCounterHandler>().Show();
        yield return deathTimer.GetComponent<BattleDeathCounterHandler>().StartCountdown(delayTimeBeforeDeath);
    }

    public IEnumerator BoxBreakCoroutine() {
        nameTextObject.SetActive(false);
        copyOfDeathTimer.GetComponent<BattleDeathCounterHandler>().Destroy();
        anim.Play("InfoBoxWarble");
        yield return new WaitForSeconds(0.001f);
        while (isPlaying(anim, "InfoBoxWarble")) {
            yield return null;
        }
        anim.Play("InfoBoxBreak");
        yield return Shake(1, 0.4f);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 40; i++) {
            bgObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.01f);
            transform.position -= new Vector3(0, 0.04f, 0);
            yield return new WaitForSeconds(Mathf.Min(0.04f, 0.005f + (i / 1000f)));
        }
    }

    private IEnumerator Shake(float amplitude = 1, float waitMultiplier = 1) {
        float waitTime = 0.1f * waitMultiplier;
        float waitTimeInc = 0.005f  * waitMultiplier;
        float moveDistance = 0.8f * amplitude;
        float moveDistanceInc = 0.05f * amplitude;
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 10; j++) {
                transform.position += new Vector3(moveDistance / 10, 0, 0);
                yield return new WaitForSeconds(waitTime / 10);
            }
            moveDistance = moveDistance * -1 + ((moveDistance > 0) ? moveDistanceInc * 2 : moveDistanceInc * -2);
            waitTime += waitTimeInc;
        }
    }

    public bool isPlaying(Animator anim, string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

}
