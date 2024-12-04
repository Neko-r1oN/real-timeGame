using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharaImg : MonoBehaviour
{
    GameObject cam;
    GameObject ball;

    [SerializeField] private float carrentPos;
    [SerializeField] private float nowPos;



    private void Start()
    {
        carrentPos = this.transform.position.x;

        cam = GameObject.Find("MenuCamera");
       
        //this.GetComponent<SpriteRenderer>().flipX = true;

    }
    private void FixedUpdate()
    {

       

        carrentPos = this.transform.position.x;


        //カメラに対して平行に画像が表示される処理

        // 対象物と自分自身の座標からベクトルを算出
        Vector3 vector3 = cam.transform.position - this.transform.position;
        // もし上下方向の回転はしない(Baseオブジェクトが床から離れないようにする)ようにしたければ以下のようにする。
        //vector3.y = 0f;

        // Quaternion(回転値)を取得
        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // 算出した回転値をこのゲームオブジェクトのrotationに代入
        this.transform.rotation = quaternion;
    }
}
