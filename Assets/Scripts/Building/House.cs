using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour, IInteractable
{
    PlayerConditions playerConditions;

    private void Start()
    {
        playerConditions = GameManager.instance.playerObject.GetComponent<PlayerConditions>();
    }

    public string GetInteractPrompt()
    {
        return string.Format("휴식하기");
    }

    public void OnInteract()
    {
        // 낮일 경우 휴식 불가
        if (playerConditions.dayNightCycle.time > 0.2f && playerConditions.dayNightCycle.time < 0.8f)
        {
            return;
        }

        playerConditions.dayNightCycle.Sleep();
    }
}
