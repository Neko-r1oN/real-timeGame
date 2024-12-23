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
    public bool isDash { get; set; }

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

    //�v���C���[�A�j���[�V����
    Animator playerAnim;

    private GameObject gameDirector;


    private void Start()
    {
        // isCatch = true;
        animState = 0;

        //�ʏ�����񏉊���
        isHaveBall = false;

        isDash = false;

        //�{�[���v���n�u�擾
        ballPrefab = (GameObject)Resources.Load("Ball");
        gameDirector = (GameObject)Resources.Load("GameDirector");

        //�J�[�\���I�u�W�F�N�g���擾
        cursor = GameObject.Find("Cursor");

        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearObj = EnemySerch();

       
        

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

        //playerAnimation�擾
        playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

        Move();

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
               // animState = 1;    //�_�b�V��

                // �v���C���[�̈ʒu(transform.position)�̍X�V
                // �ړ������x�N�g��(velocity)�𑫂����݂܂�
                transform.position += velocity;
            }
            else
            {
               // animState=0;
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


   

        public void Move()
    {

        float dx = fixedJoystick.Horizontal; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�
        float dy = fixedJoystick.Vertical; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�

        float rad = Mathf.Atan2(dx - 0, dy - 0); //�@ ���_(0,0)�Ɠ_�idx,dy)�̋�������p�x���Ƃ��Ă����֗��Ȋ֐�

        float deg = rad * Mathf.Rad2Deg; //radian����degreen�ɕϊ����܂�



        if (deg != 0) //joystick�̌��_��(dx,dy)�̂Q�_���Ȃ��p�x���O�ł͂Ȃ��Ƃ� = joystick�𓮂����Ă��鎞
        {
            animState = 1; //wait��walk��
            isDash = true;
            playerAnim.SetBool("isDash", true);
        }
        else //joystick�̌��_��(dx,dy)�̂Q�_���Ȃ��p�x���O�̎� = joystick���~�܂��Ă��鎞
        {
            if (isDash)
            {
                animState = 0; //walk��wait��
                playerAnim.SetBool("isDash",false);
                isDash = false;
            }
        }
    }

    public void OnClickJump()
    {
        animState = 2;
        playerAnim.SetBool("isJump", true);
        isClickJump = true;
    }

    public void OnClickCatch()
    {
        isCatch = true;
        Debug.Log("�L���b�`");
        animState = 3;
        playerAnim.SetTrigger("isCatch");
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
            playerAnim.SetBool("isThrow", true);
        }
        else
        {
            animState = 4;
            playerAnim.SetBool("isThrow", true);
        }
        Invoke("isThrowOut", 2.0f);
        Shot();
    }

    //�t�F�C���g(������ӂ�)����
    public  void OnClickFeint()
    {
        animState = 4;

        playerAnim.SetBool("isThrow", true);
        Invoke("isThrowOut", 1.0f);
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

            ThrowBall();

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
        //animState = 0;
       

        //���n�����o�����̂Œ��n��Ԃ���������
        if (!isGround)
        {
            Debug.Log(animState);
            animState = 0;
            playerAnim.SetBool("isJump", false);

            isClickJump = false;
            isGround = true;
        }
      
        

        if (other.gameObject.tag == "Clear")
        {
            // �S���[�U�[�ɃQ�[���I���ʒm
            FinishGame();
        }

        if (other.gameObject.tag == "EnemyBall")
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

        //�_���[�W���̂Ȃ���Ԃ�������
        if(other.gameObject.tag == "EasyBall")
        {
            //�{�[��������Ԃɂ���
            isHaveBall = true;
            Destroy(other.gameObject);    //�{�[���폜
            Debug.Log("�Q�b�g");

            //�{�[���l��
            GetBall();
        }

    }

    //�{�[���擾����
    private async void GetBall()
    {
        await roomModel.GetBallAsync();
    }
    //�{�[���擾����
    private async void ThrowBall()
    {
        //�{�[�����
        var throwData = new ThrowData()
        {
            ConnectionId = roomModel.ConnectionId,            //�ڑ�ID
            ThorwPos = this.gameObject.transform.position,    //�������v���C���[�̍��W
            GoalPos = searchNearObj.transform.eulerAngles,    //�ڕW���W

        };

        //�{�[�����˒ʒm
        await roomModel.ThrowBallAsync(throwData);

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

    private void isThrowOut()
    {
        playerAnim.SetBool("isThrow", false);
    }

}
