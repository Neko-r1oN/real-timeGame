using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    private string animState;   //�A�j���X�e�[�g
    private bool animJudge;     //�A�j������

    private PlayerManager player;
    private EnemyManager enemy;
  
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    float degStop;


    void Start()
    {
        //GameObject parentObject = GameObject.Find("MyPlay");

        anim = this.gameObject.GetComponent<Animator>();

        player = GetComponentInParent<PlayerManager>();//��ԏ�̐e�ɂ��Ă���R���|�[�l���g���擾����

        //
        if(player == null)
        {
            enemy = GetComponentInParent<EnemyManager>();//��ԏ�̐e�ɂ��Ă���R���|�[�l���g���擾����

        }


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

      
    }
        
   

    public void SetTrigger()
    {

        anim.SetTrigger(animState); //wait��walk��
    }
    public void SetBool()
    {

        anim.SetBool(animState,true); //wait��walk��
    }

     
    public void ResetJump()
    {
        anim.SetBool("isJump",false);
    }
    public void ResetAnim()
    {
        anim.SetBool(animState,false);
        Debug.Log("�L�����Z���Ă΂ꂽ");

        Debug.Log(animState);

    }

   public int GetAnimId()
    {
        return 0;

    }
    public void SetAnim(int animName)
    {

    }
}
