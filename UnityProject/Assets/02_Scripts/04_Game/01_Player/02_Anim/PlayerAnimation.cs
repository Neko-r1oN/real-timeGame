using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator = null;

    private FixedJoystick joyStick;

    public bool isLeft;

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

        isLeft = false;

    }

    void Update()
    {
        if (isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = false;

        }
        if (!isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }


    }

    /// <summary>
    /// �A�j���[�V����ID�擾
    /// </summary>
    /// <returns>�A�j���[�V����ID</returns>
   public int GetAnimId()
    {
        return animator.GetInteger("animation");
    }

    public void SetAnim(ANIM_STATE animId)
    {
        animator.SetInteger("animation", (int)animId);
    }

    //�G�A�j���[�V�����֐�
    public void SetEnemyAnim(int id)
    {
        animator.SetInteger("animation", id);
    }

    public bool GetAngle()
    {
        return isLeft;
    }
}
