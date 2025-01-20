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
        IDLE = 0,                  //�ҋ@��
        HAVE_IDLE = 1,             //�ҋ@��(����)
        DASH = 5,                  //�_�b�V����
        HAVE_DASH = 6,             //�_�b�V����(����)
        JUMP = 10,                 //�W�����v��
        HAVE_JUMP = 11,            //�W�����v��(����)
        CATCH = 15,                //�L���b�`��
        HAVE_CATCH = 16,           //�L���b�`��(����)
        FEINT = 30,                //������(�t�F�C���g)
        THROW = 31,                //������(����)
        AIRTHROW = 35,             //�󒆓�����
        AIRFEINT = 36,             //�󒆓�����(�t�F�C���g)
        DOWN = 80,                 //�_�E����
        DEAD = 90,                 //���S��
    }

    //�A�j���X�e�[�g�������l�ɐݒ�
    ANIM_STATE anim_State = ANIM_STATE.IDLE;

    //�R���|�[�l���g�t�^����
    public void Init()
    {
        animator = this.gameObject.GetComponent<Animator>();

        isLeft = false;
    }

    void Update()
    {
        //�L�����N�^�[�̌����؂�ւ�
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

    //�G�A�j���[�V�������f�֐�
    public void SetEnemyAnim(int id)
    {

        animator.SetInteger("animation", id);
    }

    //�G�̌������f�֐�
    public bool GetAngle()
    {
        return isLeft;
    }
}
