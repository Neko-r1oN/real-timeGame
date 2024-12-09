using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnomation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    PlayerManager playerManager;

    Button jumpButton;
    Button catchButton;
    Button throwButton;
    float degStop;

    void Start()
    {
        GameObject parentObject = GameObject.Find("MyPlay");

        anim = parentObject.transform.GetChild(0).GetComponent<Animator>();
        
        joyStick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();


       
    }

    void Update()
    {

        Move();

        /*switch (playerManager.animState)
        {
            case 0:    //�A�C�h�����
                anim.SetBool("isJump", true); //wait��walk��
                break;
            case 1:    //�_�b�V�����
                anim.SetBool("isDash", true); //wait��walk��
                break;
            case 2:

                break;
            default:
            
                break;
        }*/
    }
        
    public  void Move()
    {

        float dx = joyStick.Horizontal; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�
        float dy = joyStick.Vertical; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�

        float rad = Mathf.Atan2(dx - 0, dy - 0); //�@ ���_(0,0)�Ɠ_�idx,dy)�̋�������p�x���Ƃ��Ă����֗��Ȋ֐�

        float deg = rad * Mathf.Rad2Deg; //radian����degreen�ɕϊ����܂�

       

        if (deg != 0) //joystick�̌��_��(dx,dy)�̂Q�_���Ȃ��p�x���O�ł͂Ȃ��Ƃ� = joystick�𓮂����Ă��鎞
        {
            anim.SetBool("isDash", true); //wait��walk��

            degStop = deg; //��~�O�̃v���C���[�̌�����ۑ�
        }
        else //joystick�̌��_��(dx,dy)�̂Q�_���Ȃ��p�x���O�̎� = joystick���~�܂��Ă��鎞
        {
            anim.SetBool("isDash", false); //walk��wait��

        }
    }
}
