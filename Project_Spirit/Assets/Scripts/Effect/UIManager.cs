using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject researchUI;
    [SerializeField]
    private GameObject CraftModeUI;
    [SerializeField]
    private GameObject SpiritSpawnerUI;
    [SerializeField]
    private GameObject CharacterUI;
    [SerializeField]
    private GameObject BuildingInfo;
  
    // Update is called once per frame
    void Update()
    {
      if(researchUI.activeSelf || CraftModeUI.activeSelf || SpiritSpawnerUI.activeSelf)
        {
            CharacterUI.SetActive(false);
            foreach(Transform info in BuildingInfo.transform)
            {
                info.gameObject.SetActive(false);
            }
        }
            
    }
}
