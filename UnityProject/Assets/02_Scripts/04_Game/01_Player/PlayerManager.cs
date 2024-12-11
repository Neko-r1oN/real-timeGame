using DG.Tweening;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    RoomModel roomModel;

    private string tagName = "Enemy";          //�C���X�y�N�^�[�ŕύX�\

    private GameObject searchNearObj;          //�ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)
    private GameObject cursor;

    public bool isHaveBall { get; set; }       //�{�[�����������Ă��邩

    public bool isGround { get; set; }         //�n�ʂɐG��Ă��邩
    public bool isClickJump;

    public bool isCatch { get; set; }

    public float velosity = 6.0f;

    public int animState { get; set; }

    GameObject ballPrefab;
    public float ballSpeed = 15.0f;
    

    [SerializeField] private Vector3 velocity;              // �ړ�����
    [SerializeField] private float moveSpeed = 6.0f;        // �ړ����x

    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;

    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;

    private GameObject closeEnemy;

    private void Start()
    {
        //�{�[���v���n�u�擾
        ballPrefab = (GameObject)Resources.Load("Ball");

        //�J�[�\���I�u�W�F�N�g���擾
        cursor = GameObject.Find("Cursor");

        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearObj = EnemySerch();

       
        animState = 0;

        //�ʏ�����񏉊���
        isHaveBall = false;

        isGround = false;
        isClickJump = false;

        rigidbody = GetComponent<Rigidbody>();
        fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        //�W�����v�{�^��
        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();
        jumpButton.onClick.AddListener(() => OnClickJump());

        //�L���b�`�{�^��
        catchButton = GameObject.Find("CatchButton").GetComponent<Button>();
        catchButton.onClick.AddListener(() => OnClickCatch());

        //�t�F�C���g�{�^��
        feintButton = GameObject.Find("FeintButton").GetComponent<Button>();
        catchButton.onClick.AddListener(() => OnClickFeint());

        //������{�^��
        throwButton = GameObject.Find("ThrowButton").GetComponent<Button>();
        throwButton.onClick.AddListener(() => OnClickThrow());


        catchbtn = GameObject.Find("CatchButton");

        //���[�����f���̎擾
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
        

    }

    void Update()
    {
        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

        if (isHaveBall)
        {
            catchbtn.SetActive(false);
            
        }else if(!isHaveBall)
        {
            catchbtn.SetActive(true);
          
        }

        //�L���b�`��ԂłȂ��ꍇ�݈̂ړ��ł���
        if (isCatch != true)
        {
            // ���x�x�N�g���̒�����1�b��moveSpeed�����i�ނ悤�ɒ������܂�
            velocity = velocity.normalized * moveSpeed * Time.deltaTime;

            // �����ꂩ�̕����Ɉړ����Ă���ꍇ
            if (velocity.magnitude > 0)
            {
                animState = 1;    //�_�b�V��

                // �v���C���[�̈ʒu(transform.position)�̍X�V
                // �ړ������x�N�g��(velocity)�𑫂����݂܂�
                transform.position += velocity;
            }

        }
        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearObj = EnemySerch();

        if (searchNearObj)
        {
            transform.LookAt(searchNearObj.transform);
        }

        //�t�B�[���h�ɓG�v���C���[�����݂��Ă���ꍇ
        if (searchNearObj != null)
        {
            //�J�[�\�����ł��߂��G�̍��W�Ɉړ�
            cursor.transform.DOMove(searchNearObj.gameObject.transform.position, 0.1f).SetEase(Ease.Linear);
        }
        //�t�B�[���h�ɓG�v���C���[�����݂��Ă��Ȃ��ꍇ
        else
        {
            //�J�[�\���������ʒu�Ɉړ�
            cursor.transform.DOMove(new Vector3(10.714f, -1.94f,12.87f), 0.1f).SetEase(Ease.Linear);
        }

        //���n���Ă��邩�𔻒�
        if (isGround == true)
        {
            //�X�y�[�X�L�[��������Ă��邩�𔻒�
            if (isClickJump == true)
            {
                

                //�W�����v�̕�����������̃x�N�g���ɐݒ�
                Vector3 jump_vector = Vector3.up;
                //�W�����v�̑��x���v�Z
                Vector3 jump_velocity = jump_vector * velosity;

                //������̑��x��ݒ�
                rigidbody.velocity = jump_velocity;
                //�n�ʂ��痣���̂Œ��n��Ԃ���������
                isGround = false;
            }
        }

    }

    public void OnClickJump()
    {
        animState = 2;
        isClickJump = true;
    }

    public void OnClickCatch()
    {
        isCatch = true;
        Debug.Log("�L���b�`");
        animState = 3;
        //yield return new WaitForSeconds(1);

        isCatch = false;
        Debug.Log("�L���b�`����");

    }

    public void OnClickThrow()
    {
        //�W�����v��Ԃ�������
        if (isClickJump)
        {
            //�W�����v�A�j���[�V����
            animState = 4;
        }
        else
        {
            animState = 5;
        }
        Shot();
    }

    //�t�F�C���g(������ӂ�)����
    public void OnClickFeint()
    {
        animState = 2;
    }


    void Shot()
    {

        // shotObj.GetComponent<Rigidbody>().velocity = transform.forward * ballSpeed;

        if (isHaveBall)
        {

            GameObject newbullet = Instantiate(ballPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity); //�e�𐶐�
            Rigidbody bulletRigidbody = newbullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (transform.forward * ballSpeed); //�L�����N�^�[�������Ă�������ɒe�ɗ͂�������
            //Destroy(newbullet, 10); //10�b��ɒe������

            //�{�[��������Ԃ���������
            isHaveBall = false;
        }
        else
        {
            Debug.Log("�ʎ����ĂȂ���");
        }
    }


    void OnCollisionEnter(Collision other)
    {
        animState = 0;
        //���n�����o�����̂Œ��n��Ԃ���������
        isGround = true;
        isClickJump = false;

        if (other.gameObject.tag == "Clear")
        {
            // �S���[�U�[�ɃQ�[���I���ʒm
            FinishGame();
        }

        if (other.gameObject.tag == "Ball")
        {
            //�L���b�`��ԂŃ{�[���ɐG������
            if(isCatch)
            {
                //�{�[��������Ԃɂ���
                isHaveBall = true;
                Destroy(other.gameObject);    //�{�[���폜
                Debug.Log("�L���b�`����");
            }
            //�L���b�`��Ԃ���Ȃ�������
            else
            {
                //�_�E������
            }
        }
        if(other.gameObject.tag == "EasyBall")
        {
            //�{�[��������Ԃɂ���
            isHaveBall = true;
            Destroy(other.gameObject);    //�{�[���폜
            Debug.Log("�Q�b�g");
        }

    }
    
    // �Q�[���I���ʒm���M����
    private async void FinishGame()
    {
        await roomModel.FinishGameAsync();
    }



    /// <summary>
    /// �w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    /// </summary>
    /// <returns></returns>
    private GameObject EnemySerch()
    {

        // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;

        // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
        GameObject searchTargetObj = null;

        // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă�������)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objs����P����obj�ϐ��Ɏ��o��
        foreach (GameObject obj in objs)
        {

            // obj�Ɏ��o�����Q�[���I�u�W�F�N�g�ƁA���̃Q�[���I�u�W�F�N�g�Ƃ̋������v�Z���Ď擾
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistance���X�V
                nearDistance = distance;

                // searchTargetObj���X�V
                searchTargetObj = obj;
            }
        }

        //�ł��߂������I�u�W�F�N�g��Ԃ�
        return searchTargetObj;
    }

}
