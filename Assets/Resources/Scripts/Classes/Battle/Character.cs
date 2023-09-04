using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterType
{
    Ally, Enemy
}

public enum CharacterStatus
{
    Alive, Unconscious, Dead, Escaped
}

[System.Serializable]
public class Stats
{
    public int health;
    public int damage;
    public int defense;
    public int utility;
    public int energy;
    public float dodgeChance;
    public int GetMaxHealth()
    {
        return health;
    }
    public int GetDamage()
    {
        return damage;
    }
    public int GetDefense()
    {
        return defense;
    }
    public int GetUtility()
    {
        return utility;
    }
    public int GetMaxEnergy()
    {
        return energy;
    }
    public float GetDodgeChance()
    { // 0 (never dodge) - 1 (always dodge)
        return dodgeChance;
    }
}

public class Character
{

    public string name = "UNNAMED"; // Must be public or else JSONCharacterFinder.cs won't set properly
    protected CharacterStatus status = CharacterStatus.Alive;
    public Stats baseStats;
    protected int id;
    public bool isDefending = false;
    public string nameSuffix = "";
    public int currentHealth;
    public int currentEnergy;
    public int energyCharge;
    public Vector3 spriteScale = new Vector3(1, 1, 1); // How should the sprite be scaled?
    public Vector3 spriteTransform = new Vector3(0, 0, 0); // Should the sprite be moved on init?
    public Vector3 relativeStatusTransform = new Vector3(0, 0, 0); // Where to show debuff icons (rel pos)?
    public Vector3 relativeItemTransform = new Vector3(0, 0, 0); // Where should items spawn when used (rel pos)?
    public GameObject spriteObject;
    public InfoBoxHandler infoBoxHandler;
    public BattleCharacterController bcc;
    public CharacterType characterType;
    public AttackType attackType;
    public List<Special> specialList = new List<Special>();
    private List<StatusEffect> statusEffects = new List<StatusEffect>();
    public float spriteWidth = 3;

    public override string ToString()
    {
        return "[" + characterType + " " + name + "]";
    }
    public Vector3 GetPosition()
    {
        return spriteObject.transform.position;
    }
    /// <summary>
    /// Returns a boolean based on whether or not the character dodges the attack.
    /// </summary>
    public bool RollDodgeChance()
    {
        return Random.Range(0f, 1f) < baseStats.GetDodgeChance();
    }
    public CharacterStatus GetStatus()
    {
        if (status != CharacterStatus.Escaped && currentHealth > 0)
        {
            return CharacterStatus.Alive;
        }
        return status;
    }
    public void SetStatus(CharacterStatus status)
    {
        this.status = status;
        if (status == CharacterStatus.Dead || status == CharacterStatus.Escaped)
        {
            RemoveAllStatusEffects();
        }
    }
    /// <summary>
    /// Returns a string that should be displayed for the current character.
    /// </summary>
    public string GetRawName()
    {
        return name;
    }
    /// <summary>
    /// Returns a string that will be read to target animators.
    /// </summary>
    public string GetName()
    {
        return name + nameSuffix;
    }
    public int GetMaxHealth()
    {
        return baseStats.GetMaxHealth();
    }
    public int GetMaxEnergy()
    {
        return baseStats.GetMaxEnergy();
    }
    public int GetDamage()
    {
        return baseStats.GetDamage();
    }
    public int GetDefense()
    {
        return baseStats.GetDefense();
    }
    public void SetId(int id)
    {
        this.id = id;
    }
    public int GetId()
    {
        return this.id;
    }
    public int CalculateDamage(int initialDamageTaken, bool bypassDefense)
    {
        int damageTaken = initialDamageTaken;
        // Increase damage taken by 25% if vulnerable stat is active
        if (HasStatusEffect(Effect.Vulnerable))
        {
            damageTaken = (int)(damageTaken * 1.25f);
            bcc.EmphasizeStatusEffect(Effect.Vulnerable);
        }
        // Reduce damage by 40% if currently blocking
        if (isDefending)
        {
            damageTaken = (int)((float)damageTaken * 0.6f);
        }
        if (!bypassDefense)
        {
            // Defend [defense stat] damage from attack, but not more than half initial damage can be removed
            damageTaken = Mathf.Max(damageTaken / 2, damageTaken - GetDefense());
        }
        return damageTaken;
    }
    // <summary>
    // Takes a double from 0 to 1 on how effective the charge was. Increases by energyCharge * effectiveness.
    // </summary>
    public void ChargeEnergy(float effectiveness)
    {
        int changeInEnergy = (int)(energyCharge * effectiveness);
        this.currentEnergy += changeInEnergy;
        if (this.currentEnergy > this.baseStats.GetMaxEnergy())
        {
            changeInEnergy -= this.currentEnergy - this.baseStats.GetMaxEnergy();
            this.currentEnergy = this.baseStats.GetMaxEnergy();
        }
        if (infoBoxHandler != null)
        {
            infoBoxHandler.ChangeEnergy(changeInEnergy);
        }
    }
    public Special GetSpecial(string specialName)
    {
        return specialList.Single(special => special.GetName() == specialName);
    }
    public virtual List<Special> GetSpecials()
    {
        return specialList;
    }
    public int UseSpecial(string specialName)
    {
        int cost = GetSpecial(specialName).GetEnergyCost();
        currentEnergy -= cost;
        if (infoBoxHandler != null)
        {
            infoBoxHandler.ChangeEnergy(-1 * cost);
        }
        return cost;
    }
    public void RefundEnergy(int energyEarned)
    {
        currentEnergy += energyEarned;
        if (infoBoxHandler != null)
        {
            infoBoxHandler.ChangeEnergy(energyEarned);
        }
    }
    public virtual bool CanCast(string special)
    {
        Special s = GetSpecial(special);
        if (s.GetEnergyCost() > currentEnergy)
        {
            return false;
        }
        return true;
    }
    public List<StatusEffect> GetStatusEffects()
    {
        return statusEffects;
    }
    public StatusEffect GetStatusEffect(Effect effect)
    {
        StatusEffect foundStatusEffect = GetStatusEffects().Find(e => e.GetEffectType() == effect);
        return foundStatusEffect;
    }
    public void AddStatusEffect(StatusEffect effect)
    {
        statusEffects.Add(effect);
    }
    public void RemoveStatusEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
        bcc.RemoveStatusEffect(effect);
    }
    public void RemoveAllStatusEffects()
    {
        for (int i = GetStatusEffects().Count - 1; i >= 0; i--)
        {
            RemoveStatusEffect(GetStatusEffects()[i]);
        }
    }
    public bool HasStatusEffect(Effect effect)
    {
        return GetStatusEffect(effect) != null;
    }
    public void InflictStatusEffect(StatusEffect effect, float delay = 0)
    {
        if (GetStatus() == CharacterStatus.Dead) { return; }
        StatusEffect foundStatusEffect = GetStatusEffect(effect.GetEffectType());
        if (foundStatusEffect != null)
        {
            if (characterType == CharacterType.Enemy)
            {
                effect.recentlyAdded = false;
            }
            foundStatusEffect.TopOffTurnsRemaining(effect.GetTurnsLeft());
            bcc.SetStatusEffectTurn(foundStatusEffect, foundStatusEffect.GetTurnsLeft());
            return;
        }
        AddStatusEffect(effect);
        bcc.InflictStatusEffect(effect, delay);
    }

    public void UpdateStatusEffectTurns()
    {
        if (GetStatusEffects().Count == 0)
        {
            return;
        }
        for (int i = GetStatusEffects().Count - 1; i >= 0; i--)
        {
            int turnsLeft = GetStatusEffects()[i].DeductTurn(characterType);
            bcc.SetStatusEffectTurn(GetStatusEffects()[i], turnsLeft);
            if (turnsLeft == 0)
            {
                RemoveStatusEffect(GetStatusEffects()[i]);
            }
        }
    }
    public IEnumerator ParalyzeSuccessEffect()
    {
        bcc.EmphasizeStatusEffect(Effect.Paralyze);
        bcc.Flash(new Color(0.83f, 0.82f, 0, 0.8f));
        bcc.Shake(0.6f, 1f, true);
        yield return new WaitForSeconds(0.8f);
    }
    public IEnumerator ParalyzeFailEffect()
    {
        yield return null;
    }
    public IEnumerator PoisonEffect()
    {
        bcc.EmphasizeStatusEffect(Effect.Poison);
        bcc.TakeDamageSequence((int)(GetMaxHealth() / 8f), false, true);
        yield return new WaitForSeconds(0.3f);
    }

}