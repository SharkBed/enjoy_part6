using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearChackScript : MonoBehaviour
{
    static public ClearChackScript s_instance;
    public bool ClearFlag;

    // Use this for initialization
    void Start()
    {
        s_instance = this;
        ClearFlag = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {  //見つけたお！
            ClearFlag = true;
        }
    }
    public bool IsClear()
    {
        return ClearFlag;
    }

}
