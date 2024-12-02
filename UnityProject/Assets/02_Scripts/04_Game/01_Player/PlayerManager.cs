using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float speed;
    private GameObject[] targets;
    private bool isSwitch = false;

    private float[] PlayersPos;

    //�A�j���[�V����
    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    [SerializeField] private Vector3 velocity;              // �ړ�����
    [SerializeField] private float moveSpeed = 6.0f;        // �ړ����x


    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;

    private GameObject closeEnemy;

    private void Start()
    {
        // �^�O���g���ĉ�ʏ�̑S�Ă̓G�̏����擾
        targets = GameObject.FindGameObjectsWithTag("Player");

        rigidbody = GetComponent<Rigidbody>();
        fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        // �u�����l�v�̐ݒ�
        float closeDist = 1000;

        //�X�e�[�W��ɑ��݂���v���C���[�̏����擾
        foreach (GameObject t in targets)
        {
            // �R���\�[����ʂł̊m�F�p�R�[�h
            //print(Vector3.Distance(transform.position, t.transform.position));

            // ���̃I�u�W�F�N�g�i�C�e�j�ƓG�܂ł̋������v��
            float tDist = Vector3.Distance(transform.position, t.transform.position);

            // �������u�����l�v�����u�v�������G�܂ł̋����v�̕����߂��Ȃ�΁A
            if (closeDist > tDist)
            {
                // �ucloseDist�v���utDist�i���̓G�܂ł̋����j�v�ɒu��������B
                // ������J��Ԃ����ƂŁA��ԋ߂��G�������o�����Ƃ��ł���B
                closeDist = tDist;

                // ��ԋ߂��G�̏���closeEnemy�Ƃ����ϐ��Ɋi�[����i���j
                closeEnemy = t;
            }
        }

        // �C�e�����������0.5�b��ɁA��ԋ߂��G�Ɍ������Ĉړ����J�n����B
        Invoke("SwitchOn", 0.5f);
    }

    void Update()
    {
        if (isSwitch)
        {
            float step = speed * Time.deltaTime;

            // ���œ���ꂽcloseEnemy��ړI�n�Ƃ��Đݒ肷��B
            //transform.position = Vector3.MoveTowards(transform.position, closeEnemy.transform.position, step);
        }

        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

        /*float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x == 0)
        {
            direction = DIRECTION_TYPE.STOP;
        }else if(x > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;
        }else if(x< 0)
        {
            direction= DIRECTION_TYPE.LEFT;
        }*/

        // ���x�x�N�g���̒�����1�b��moveSpeed�����i�ނ悤�ɒ������܂�
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;

        // �����ꂩ�̕����Ɉړ����Ă���ꍇ
        if (velocity.magnitude > 0)
        {
            // �v���C���[�̈ʒu(transform.position)�̍X�V
            // �ړ������x�N�g��(velocity)�𑫂����݂܂�
            transform.position += velocity;
        }
    }

    private void FixedUpdate()
    {
        /*switch (direction)
        {
            case DIRECTION_TYPE.STOP:

                break;

            case DIRECTION_TYPE.RIGHT:

                break;

            case DIRECTION_TYPE.LEFT:

                break;
        }
        rigidbody.velocity = new Vector3(speed,rigidbody.velocity.y,rigidbody.velocity.x);*/
    }

    void SwitchOn()
    {
        isSwitch = true;
    }
}
