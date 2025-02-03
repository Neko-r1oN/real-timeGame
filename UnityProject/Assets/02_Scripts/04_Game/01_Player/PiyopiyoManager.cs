////////////////////////////////////////////////////////////////////////////
///
///  ピヨピヨ(ダウン表示)スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

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
        //親取得
        rootObject = transform.parent.gameObject;
        angleManager = rootObject.transform.GetChild(0).GetComponent<AngleManager>();
    }

    void Update()
    {
        if (!angleManager.isLeft) pos.x = -1.73f;  
        else pos.x = 1.39f;
        //座標変更
        this.gameObject.transform.localPosition = pos;
    }
}
