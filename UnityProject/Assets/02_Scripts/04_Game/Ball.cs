////////////////////////////////////////////////////////////////////////////
///
///  �{�[�������X�N���v�g
///  Author : ������C  2025.01/28
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
        this.name = "Ball";   //�I�u�W�F�N�g���ύX

        GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 1);
        objectCollider = GetComponent<CapsuleCollider>();
        objectCollider.isTrigger = false; //�����蔻��L����
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; //�d�͂𖳌��ɂ���
        Invoke("GravityChange", 0.5f);
    }

    private async void GravityChange()
    {
        rb.useGravity = true; //�d�͂�L���ɂ���
    }

    void OnCollisionEnter(Collision collision)
    {
        //�t�B�[���h�ɓ��������ꍇ
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall") 
        {
            SEManager.Instance.Play(
                audioPath: SEPath.BOUND,      //�Đ��������I�[�f�B�I�̃p�X
                volumeRate: 1,                //���ʂ̔{��
                delay: 0,                     //�Đ������܂ł̒x������
                pitch: 1,                     //�s�b�`
                isLoop: false,                 //���[�v�Đ����邩
                callback: null                //�Đ��I����̏���
            );

            //���Q��
            GameObject fire = this.gameObject.transform.GetChild(2).gameObject;
            fire.SetActive(false);
            this.gameObject.tag = "EasyBall";
        }
        //��O����ɂ��������ꍇ
        if (collision.gameObject.tag == "Warp") this.gameObject.transform.position = new Vector3(0.0f, 0.7f, -0.6f);     //�X�e�[�W�����Ƀ��[�v

    }
}