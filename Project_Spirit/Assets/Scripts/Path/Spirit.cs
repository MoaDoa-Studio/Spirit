 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    public int SpiritID;
    public int SpiritJob;
    public int SpiritElement;
    public float SDefaultLife;
    public float SpiritSpeed;
    string SpiritName;

    private void Start()
    {
        SDefaultLife = 100f;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Spirit")
        {
            
            int element = collision.gameObject.GetComponent<Spirit>().SpiritElement;
            if(element != SpiritElement )
            {
                if(collision.gameObject.GetComponent<Spirit>().SDefaultLife >= SDefaultLife )
                {
                    collision.gameObject.GetComponent<Spirit>().SDefaultLife -= SDefaultLife;
                    if(collision.gameObject.GetComponent<Spirit>().SDefaultLife <= 0 )
                    {
                        Destroy( collision.gameObject );
                    }
                    Destroy(this.gameObject);
                }
                else
                {
                    SDefaultLife -= collision.gameObject.GetComponent<Spirit>().SDefaultLife;
                    Destroy( collision.gameObject );
                }
            }
            
        }
    }
}
