using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cook : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return string.Format("요리하기");
    }

    //요리하기
    public void OnInteract()
    {
        if (Inventory.instance.CheckHaveItem(330))
        {
            SoundManager.instacne.PlayEffectSound(EffectSound.Cook);
            Inventory.instance.UseItme(330);
            Inventory.instance.AddItem(Resources.Load<ItemData>("Scriptable/Steak"));
        }
        else Debug.Log("고기가 없음");
    }
}
