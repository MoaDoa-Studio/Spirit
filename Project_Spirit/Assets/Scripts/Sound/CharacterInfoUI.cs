using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
   public void ToggleOffUI()
    {
        SoundManager.instance.UIButtonclick();
        this.gameObject.SetActive(false);
    }
}
