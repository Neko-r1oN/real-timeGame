////////////////////////////////////////////////////////////////////////////
///
///  画像移行投影スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 画像投影クラス
/// </summary>
public class NumberImg : MonoBehaviour
{
    GameObject cam;

    public bool isY;
    private void Start()
    {
        cam = GameObject.Find("MainCamera");
    }
    private void Update()
    {
        //カメラに対して平行に画像が表示される処理

        // 対象物と自分自身の座標からベクトルを算出
        Vector3 vector3 = cam.transform.position - this.transform.position;
        // もし上下方向の回転はしない(Baseオブジェクトが床から離れないようにする)ようにしたければ以下のようにする。
        if(isY)vector3.y = 0f;

        // Quaternion(回転値)を取得
        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // 算出した回転値をこのゲームオブジェクトのrotationに代入
        this.transform.rotation = quaternion;
    }
}
