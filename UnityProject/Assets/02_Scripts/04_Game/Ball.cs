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



    PlayerManager playerManager;

    void Start()
    {
        
        this.name = "Ball";

        GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 1); //弾の色を黒にする
        objectCollider = GetComponent<CapsuleCollider>();
        objectCollider.isTrigger = false; //Triggerとして扱う
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; //重力を無効にする
        Invoke("TriggerChange", 0.5f);
    }

    private async void TriggerChange()
    {
        //objectCollider.isTrigger = false; //Triggerとして扱う

        rb.useGravity = true; //重力を有効にする

    }

    private void Update()
    {
        //Debug.Log(this.gameObject.tag);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall") //タグがEnemyのオブジェクトと衝突した場合
        {
            //cursor.SetActive(false);

            SEManager.Instance.Play(
                audioPath: SEPath.BOUND,      //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 0,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );

            this.gameObject.tag = "EasyBall";


        }
    }
}