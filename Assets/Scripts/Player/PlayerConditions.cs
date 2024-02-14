using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

[System.Serializable]
public class Condition
{
    [HideInInspector] // condition 들이 갖어야 하는 값들에 대해 미리 준비 
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)//값들을 수정하는 매서드
    {
        curValue = Mathf.Min(curValue + amount, maxValue); //더한값과 맥스값중 작은것씀 
    }

    public void Substract(float amount)//값들을 수정하는 매서드
    {
        curValue = Mathf.Max(curValue - amount, 0.0f); // 0과 뺀값중 큰값 
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}

public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;
    public Condition temperature;

    public float noHungerHealthDecay;

    public UnityEvent onTakeDamage;
    public DayNightCycle dayNightCycle; // DayNightCycle 클래스 참조

    // Start is called before the first frame update
    void Start()
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
        temperature.curValue = temperature.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Substract(hunger.decayRate * Time.deltaTime);
        stamina.Add(stamina.regenRate * Time.deltaTime);

        if (hunger.curValue == 0.0f)
            health.Substract(noHungerHealthDecay * Time.deltaTime);

        if (temperature.curValue == 0.0f)
            health.Substract(noHungerHealthDecay * Time.deltaTime);

        if (health.curValue == 0.0f)
            Die();

        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        temperature.uiBar.fillAmount = temperature.GetPercentage();

        UpdatePlayerConditions();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
        temperature.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue - amount < 0)
        {
            return false;
        }
        stamina.Substract(amount);
        return true;
    }

    public void Die()
    {
        //Debug.Log("플레이어 사망");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Substract(damageAmount);
        onTakeDamage?.Invoke();
    }

    public void UpdatePlayerConditions()
    {
        if (dayNightCycle != null)
        {
            float timeOfNight = dayNightCycle.time;

            if (timeOfNight <= 0.2f || timeOfNight >= 0.8f)
            {
                //Debug.Log(timeOfNight);
                temperature.Substract(0.01f);
            }
        }
    }
}
