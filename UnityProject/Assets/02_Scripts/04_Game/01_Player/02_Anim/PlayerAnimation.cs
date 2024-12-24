using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator = null;

    private FixedJoystick joyStick;

    //private PlayerManager player;
    //private EnemyManager enemy;
  

    //�A�j�����
    public enum ANIM_STATE
    {
        IDLE = 0,             //��~��
        DASH = 5,                 //�_�b�V����
        JUMP = 10,                 //�W�����v��
        CATCH = 15,                //�L���b�`��
        THROW = 30,                //������
        AIRTHROW = 35,             //�󒆓�����
        DOWN = 80,                 //�_�E����
        DEAD = 90,                 //���S��
    }

    //�A�j���X�e�[�g�������l�ɐݒ�
    ANIM_STATE anim_State = ANIM_STATE.IDLE;

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();

    }

    void Update()
    {
        //���@�������ꍇ
        if (GetComponentInParent<PlayerManager>())
        {

        }
    }

   public int GetAnimId()
    {
        return animator.GetInteger("animation");
    }

    public void SetAnim(int animId)
    {
        animator.SetInteger("animation", animId);
    }
}
