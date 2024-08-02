
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Spirit : MonoBehaviour
{
    public int SpiritID;
    public int SpiritJob;
    public int SpiritElement;
    public string type;
    public float SDefaultLife;
    public float HP;
    public float SpiritSpeed;
    public float Work_Efficienty;
    string SpiritName;

    GameObject CradleManager;
    GameObject ui_characater_info;

    Slider slider;
    private TMP_InputField title;
    private TMP_InputField text;

    bool characterinfo = false;
    private void Start()
    {
        SDefaultLife = 100f;
        HP = SDefaultLife;
        CradleManager = GameObject.Find("CradleManager");
        check();
    }

    public void TakeBuildingExpense()
    {

    }
    private void Update()
    {
        ToggleCharacterInfoUI();
    }
    // 각종 훈련소, 연구소 사용시 적용되는 효과
    public void TakeDamageOfBuilding()
    {
        SDefaultLife *= (1 - 0.2f);
        HP *= (1 - 0.2f);
    }
    public void TakeDamageOfResourceBuilding()
    {        
        SDefaultLife *= SpiritManager.instance.resourceBuildingDamagePercent; ;
        HP *= SpiritManager.instance.resourceBuildingDamagePercent; ;
    }
    public void TakeAdvantageOfBuilding()
    {
        SDefaultLife *= 4;
        HP *= 4;
    }
    public int GetSpiritID()
    {
        return SpiritID; 
    }
    public void SetSpiritID(int _SpiritID)
    {
        Debug.Log(SpiritID);
        SpiritID = _SpiritID;
    }

    void check()
    {
        switch(SpiritElement)
        {
            case 1:
                type = "Fire";
                SpiritName = " 불 정령";
                break;
            case 2:
                type = "Water";
                SpiritName = " 물 정령";
                break;
            case 3:
                type = "Ground";
                SpiritName = " 땅 정령";
                break;
            case 4:
                type = "Air";
                SpiritName = " 공기 정령";
                break;
        }
    }
    public void DevoteToCradle()
    {
        CradleManager.GetComponent<CradleManager>().AddElement(type, (int)HP);
        Destroy(this.gameObject);
    }

    public void TakeDamage25ByWeather()
    {
        HP  -= 0.041667f; 
    }

    public void TakeDamage25OverByWeather(float temp)
    {
        HP -= (0.041667f * (temp - 25) / 10);
    }

    public void HealInMagicStatueGrid(float temp)
    {
        HP += temp;
        // 최대 체력 초과시
        if(HP > SDefaultLife) HP = SDefaultLife;

        Debug.Log("HP. " + HP);
    }
    #region 정령충돌 감지
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Spirit")
        {
            int element = collision.gameObject.GetComponent<Spirit>().SpiritElement;
            if(element != SpiritElement ) 
            {
                // 기사 정령인 상황.
                if (SpiritID == 4)
                {
                    collision.GetComponent<SpiritAnim>().HitAndDissapear();
                    collision.GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
                    Destroy(collision.gameObject, 2f);
                }
                // 기사 정령이 아닌 상황.
                else
                {
                    if (collision.gameObject.GetComponent<Spirit>().HP >= HP)
                    {
                        collision.gameObject.GetComponent<Spirit>().HP -= HP;
                        if (collision.gameObject.GetComponent<Spirit>().HP <= 0)
                        {
                            collision.gameObject.GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
                            Destroy(collision.gameObject, 1f);

                        }
                            gameObject.GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
                            Destroy(gameObject, 1f);
                        
                    }
                    else
                    {
                        HP -= collision.gameObject.GetComponent<Spirit>().HP;
                        if(HP <= 0)
                        {
                            gameObject.GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
                            Destroy(gameObject, 1f);
                        }
                        collision.GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
                        Destroy(collision.gameObject,1f);
                    }
                }
            }
            
        }

        if(collision.gameObject.tag == "Cradle")
        {
            CradleManager.GetComponent<CradleManager>().AddElement(type, (int)HP);
            GetComponent<DetectMove>().SetDetection(DetectMove.Detect.Dead);
            Destroy(gameObject);
        }
    }
    #endregion

    private void OnMouseDown()
    {
        ui_characater_info = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().characterinfo_UI;
        ui_characater_info.SetActive(true);
        SetUIInfo(ui_characater_info.transform);
        characterinfo = true;
    }


    void SetUIInfo(Transform transform)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == SpiritElement.ToString())
            {
                t.gameObject.SetActive(true);
            }

            if (t.gameObject.name == "title")
            {
                t.GetComponent<TextMeshProUGUI>().text = $"        직업({SpiritJob})  {SpiritName}";
               
            }
            if (t.gameObject.name == "gauge")
            {
                t.GetComponent<Slider>().maxValue = SDefaultLife;
                t.GetComponent <Slider>().value = HP;
            }

            if (t.gameObject.name == SpiritElement.ToString())
            {
                t.gameObject.SetActive(true);
            }
        }


    }

    // UI 정보 초기화
    public void InitializeUIInfo()
    {
        ui_characater_info = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().characterinfo_UI;
        foreach (Transform t in ui_characater_info.transform)
        {
            if (t.gameObject.name == SpiritElement.ToString())
            {
                t.gameObject.SetActive(false);
            }
        }
        ui_characater_info.SetActive(false);
    }

    private void ToggleCharacterInfoUI()
    {
        if (characterinfo)
        {
            if (Input.GetKey(KeyCode.Escape))
            {

                foreach(Transform t in ui_characater_info.transform)
                {

                    if (t.gameObject.name == "1" || t.gameObject.name == "2" || t.gameObject.name == "3" || t.gameObject.name == "4")
                    {
                         t.gameObject.SetActive(false);
                    }
                }
                ui_characater_info.SetActive(false);
                SoundManager.instance.UIButtonclick();
            }
        }
    }
}
