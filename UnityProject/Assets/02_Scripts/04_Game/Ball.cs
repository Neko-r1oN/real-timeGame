////////////////////////////////////////////////////////////////////////////
///
///  ボール処理スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private CapsuleCollider objectCollider;
    private Rigidbody rb;

    void Start()
    {
        this.name = "Ball";   //オブジェクト名変更

        GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 1);
        objectCollider = GetComponent<CapsuleCollider>();
        objectCollider.isTrigger = false; //当たり判定有効化
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; //重力を無効にする
        Invoke("GravityChange", 0.5f);
    }

    private async void GravityChange()
    {
        rb.useGravity = true; //重力を有効にする
    }

    void OnCollisionEnter(Collision collision)
    {
        //フィールドに当たった場合
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall") 
        {
            SEManager.Instance.Play(
                audioPath: SEPath.BOUND,      //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 0,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );

            //無害化
            GameObject fire = this.gameObject.transform.GetChild(2).gameObject;
            fire.SetActive(false);
            this.gameObject.tag = "EasyBall";
        }
        //場外判定にあたった場合
        if (collision.gameObject.tag == "Warp") this.gameObject.transform.position = new Vector3(0.0f, 0.7f, -0.6f);     //ステージ中央にワープ

    }
}