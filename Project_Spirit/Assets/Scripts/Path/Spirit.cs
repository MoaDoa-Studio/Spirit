 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    public int SpiritID;
    public int SpiritJob;
    public int SpiritElement;
    public float SDefaultLife;
    public float HP;
    public float SpiritSpeed;
    public float Work_Efficienty;
    string SpiritName;

    private void Start()
    {
        SDefaultLife = 100f;
        HP = SDefaultLife;
    }

    public void TakeBuildingExpense()
    {

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
                    Destroy(collision.gameObject);
                }
                // 기사 정령이 아닌 상황.
                else
                {
                    if (collision.gameObject.GetComponent<Spirit>().HP >= HP)
                    {
                        collision.gameObject.GetComponent<Spirit>().HP -= HP;
                        if (collision.gameObject.GetComponent<Spirit>().HP <= 0)
                        {
                            Destroy(collision.gameObject);
                        }
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        HP -= collision.gameObject.GetComponent<Spirit>().HP;
                        Destroy(collision.gameObject);
                    }
                }
            }
            
        }
    }
    #endregion
}
