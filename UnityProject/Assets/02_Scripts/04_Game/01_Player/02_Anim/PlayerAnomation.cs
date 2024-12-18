using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnomation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    private string animState;

    private PlayerManager player;
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    float degStop;

    void Start()
    {
        //GameObject parentObject = GameObject.Find("MyPlay");

        anim = this.gameObject.GetComponent<Animator>();

        player = GetComponentInParent<PlayerManager>();//��ԏ�̐e�ɂ��Ă���R���|�[�l���g���擾����


        joyStick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();


       
    }

    void Update()
    {

        //Move();

        switch (player.animState)
        {
            case 0://�A�C�h�����

                //anim.SetBool(animState, false); //�A�j���[�V������~
                break;
            case 1:    //�_�b�V�����
                
                animState = "isDash";
                break;
            case 2:    //�W�����v���
                animState = "isJump";

                break;
            case 3:    //�L���b�`
                animState = "isCatch";
                break;
            case 4:    //������
                animState = "isThrow"; //wait��walk��
                break;
            case 5:   //�_�E��
                animState = "isDown";
                break;
            default:

                animState = "";
                break;
        }

        if(animState != null)
        {
            if (player.animState == 2)
            {
                SetTrigger();
            }
            else
            {
                SetBool();
            }
        }
    }
        
   

    public void SetTrigger()
    {

        anim.SetTrigger(animState); //wait��walk��
    }
    public void SetBool()
    {

        anim.SetBool(animState,true); //wait��walk��
    }
}
