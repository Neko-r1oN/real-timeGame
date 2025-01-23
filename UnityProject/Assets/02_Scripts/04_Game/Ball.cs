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

        GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 1); //�e�̐F�����ɂ���
        objectCollider = GetComponent<CapsuleCollider>();
        objectCollider.isTrigger = false; //Trigger�Ƃ��Ĉ���
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; //�d�͂𖳌��ɂ���
        Invoke("TriggerChange", 0.5f);
    }

    private async void TriggerChange()
    {
        //objectCollider.isTrigger = false; //Trigger�Ƃ��Ĉ���

        rb.useGravity = true; //�d�͂�L���ɂ���

    }

    private void Update()
    {
        //Debug.Log(this.gameObject.tag);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") //�^�O��Enemy�̃I�u�W�F�N�g�ƏՓ˂����ꍇ
        {

            this.gameObject.tag = "EasyBall";
           

        }
    }
}