using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiyopiyoManager : MonoBehaviour
{
    GameObject rootObject;

    AngleManager angleManager;

    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        //êeéÊìæ
        rootObject = transform.parent.gameObject;

        angleManager = rootObject.transform.GetChild(0).GetComponent<AngleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!angleManager.isLeft)
        {
            pos.x = -1.73f;
            //Debug.Log("êmñÿ");
        }

        else
        {
            pos.x = 1.39f;
            //Debug.Log("ç∂");
        }

        this.gameObject.transform.localPosition = pos;
    }
}
