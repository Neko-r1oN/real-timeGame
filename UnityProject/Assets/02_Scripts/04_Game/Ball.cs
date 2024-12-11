using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Invoke("TriggerChange", 0.7f);
    }

    private async void TriggerChange()
    {
        //objectCollider.isTrigger = false; //Triggerとして扱う

        rb.useGravity = true; //重力を有効にする

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") //タグがEnemyのオブジェクトと衝突した場合
        {

            this.gameObject.tag = "EasyBall";
            //rb.useGravity = true;

            //if (playerManager.isCatch) Destroy(this.gameObject); //弾を消す

            //else if (!playerManager.isCatch) rb.useGravity = false; //重力を無効にする

        }
    }
}