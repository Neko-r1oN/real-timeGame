using DG.Tweening;
using MessagePack;
using MessagePack.Resolvers;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerManager : MonoBehaviour
{

    RoomModel roomModel;
    GameDirector gameDirector;
    UserModel userModel;

    private int MAX_PLAYER = 4;

    //�J�[�\���֘A
    private string enemyTag = "Enemy";           //�擾�^�O��(�G)
    private GameObject searchNearEnemy;          //�ł��߂��G���W
    private GameObject cursor;

    //�J�[�\���֘A
    private string ballTag = "EasyBall";           //�擾�^�O��(�{�[��)
    private string enemyBallTag = "BallPlayer";     //�擾�^�O��(�{�[��������)
    private GameObject searchNearBall;          //�ł��߂��G���W

    //�v���C���[��Ԋ֘A
    public bool isHaveBall;       //�{�[�����������Ă��邩
    public bool isGround;         //�n�ʂɐG��Ă��邩
    public bool isJump;           //�W�����v���Ă��邩
    public bool isCatch;          //�L���b�`��Ԃł��邩
    public bool isDash;           //�_�b�V����Ԃł��邩
    public bool isThrow;          //������Ԃł��邩
    public bool isFeint;

    public float velosity = 13f;              //�W�����v�̋���
    public float ballSpeed = 18.0f;           //�{�[���̑���
    public float knockBack = 4.0f;
    public float catchDelay = 0.6f;
    public float throwDelay = 0.6f;
    GameObject ballPrefab;



    [SerializeField] private Vector3 velocity;              // �ړ�����
    [SerializeField] private float moveSpeed = 6.0f;        // �ړ����x

    //UI�֘A
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;    //�\���ؑ֗p

    FixedJoystick fixedJoystick;    //JoyStick
    Rigidbody rigidbody;


    //�v���C���[�A�j���[�V����
    PlayerAnimation playerAnim;
    private bool isLeft;     //�摜�̌���

    //HP�ϐ�
    public int maxHp = 5;    //HP�ő�l
    public int hp;           //HP���ݒl

    public int damage = 1;   //�_���[�W��(����Œ�)

    //���U���g�X�R�A�p
    private int throwNum;
    public int hitNum;
    private int catchNum;
    private bool isLast;     //�Ō�܂Ő����c
    private bool isDead;

    private bool isCalledOnce;

    private void Start()
    {

          
        //���[�U�[���f�����擾
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //HP�ݒ�
        hp = maxHp;

        //�v���C���[���
        isCatch = false;      //�L���b�`���
        isLeft = false;       //�����Ă������
        isHaveBall = false;   //�{�[���������
        isDash = false;       //�_�b�V�����
        isGround = false;     //�ڒn���
        isJump = false;       //�W�����v���
        isThrow = false;      //�������
        isFeint = false;

        //�{�[�����˗p�v���n�u�擾
        ballPrefab = (GameObject)Resources.Load("Ball");

        //�J�[�\���I�u�W�F�N�g���擾
        cursor = GameObject.Find("Cursor");

        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearEnemy = EnemySerch(enemyTag);

        //�{�[���^�O�擾
        searchNearBall = EnemySerch(ballTag);
        Debug.Log(searchNearBall);
        if(searchNearBall == null)
        {
            searchNearBall = EnemySerch(enemyBallTag);
            Debug.Log(searchNearBall);
        }

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
        feintButton.onClick.AddListener(() => OnClickFeint());

        //������{�^��
        throwButton = GameObject.Find("ThrowButton").GetComponent<Button>();
        throwButton.onClick.AddListener(() => OnClickThrow());


        catchbtn = GameObject.Find("CatchButton");

        //���[�����f���̎擾
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();

        //GameDirector�̎擾
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        //playerAnimation�擾
        //playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>();

        throwNum = 0;
        hitNum = 0;
        catchNum = 0;

        //�{�[���^�O�擾
        searchNearBall = EnemySerch(ballTag);
        if (searchNearBall == null)
        {
            //�{�[�������҃v���C���[�^�O�擾
            searchNearBall = EnemySerch(enemyBallTag);

        }

        //�{�[��or�{�[�������҂��擾�ł����ꍇ
        if (searchNearBall)
        {
            if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x)
            {
                isLeft = true;
            }
            else
            {
                isLeft = false;
            }
        }
        else
        {
            isLeft = true;
        }


        isLast = true;

        isCalledOnce = false;
        isDead = false;
    }

    void Update()
    {
        //HP���Ȃ��Ȃ����ꍇ
        if (!isDead && hp <= 0)
        {
            isLast = false;
            Debug.Log("�f�b�e�B");
            //���S����
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DEAD);

            //����s�\
            Destroy(fixedJoystick);
            //�v���C���[���S�ʒm
            DeadUser();
            isDead = true;
            isDead = true;

        }
        
        //�Ō�̈�l�ɂȂ����ꍇ
        if (!gameDirector.isDead && gameDirector.deadNum >= MAX_PLAYER-1 && this.isLast)
        {
            Debug.Log("�S�����񂾂��炶�Ԃ������");
            if (!isCalledOnce)
            {
                DeadUser();
                isCalledOnce = true;
            }
        }

        //��ԋ߂��G�̍��W�擾
        searchNearEnemy = EnemySerch(enemyTag);

        //�{�[���^�O�擾
        searchNearBall = EnemySerch(ballTag);
        //�{�[��������������
        if (searchNearBall = null)
        {
            //�{�[�������҂��擾
            searchNearBall = EnemySerch(enemyBallTag);
        }

        //�ړ�����
        if (!isCatch)
        {
            Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal) * moveSpeed;

            move.y = rigidbody.velocity.y;

            rigidbody.velocity = move;
        }

        //�����`�F�b�N
        Move();

        //��������X�V
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<AngleManager>().isLeft = this.isLeft;

        if (isHaveBall)
        {
            catchbtn.SetActive(false);

        } else if (!isHaveBall)
        {
            catchbtn.SetActive(true);

        }

        //�L���b�`��Ԃ�������ԂłȂ��ꍇ�݈̂ړ��ł���
        if (!isCatch || !isThrow)
        {
            // ���x�x�N�g���̒�����1�b��moveSpeed�����i�ނ悤�ɒ������܂�
            velocity = velocity.normalized * moveSpeed * Time.deltaTime;

            // �����ꂩ�̕����Ɉړ����Ă���ꍇ
            if (velocity.magnitude > 0)
            {

                // �v���C���[�̈ʒu(transform.position)�̍X�V
                // �ړ������x�N�g��(velocity)�𑫂����݂܂�
                transform.position += velocity;

            }
            else
            {

            }

        }

        //���������ԋ߂��G���擾
        if (searchNearEnemy)
        {
            transform.LookAt(searchNearEnemy.transform);
        }

        //�t�B�[���h�ɓG�v���C���[�����݂��Ă���ꍇ
        if (searchNearEnemy != null)
        {
            cursor.SetActive(true);
            //�J�[�\�����ł��߂��G�̍��W�Ɉړ�
            cursor.transform.DOMove(searchNearEnemy.gameObject.transform.position, 0.1f).SetEase(Ease.Linear);
        }
        //�t�B�[���h�ɓG�v���C���[�����݂��Ă��Ȃ��ꍇ
        else
        {
            cursor.SetActive(false);
            //�J�[�\���������Ȃ��ʒu�Ɉړ�
            cursor.transform.DOMove(new Vector3(10.714f, -1.94f, 12.87f), 0.1f).SetEase(Ease.Linear);
        }


        if (roomModel.isMaster)
        {
            //���b�N�I���J�[�\�����M

           
        }
        
        if (isThrow)
        {
            //�W�����v��Ԃ�������
            if (isJump)
            {
                //�W�����v�����A�j���[�V����

                playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRTHROW);
            }
            else
            {
                //�����A�j���[�V����
                playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.THROW);
            }
        }

        else
        {
            if (isFeint)
            {
                //�W�����v��Ԃ�������
                if (isJump)
                {
                    isJump = false;
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRFEINT);
                }
                else
                {
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);
                }
                
            }
            else
            {
                //�ڒn���W�����v��ԂłȂ��Ƃ��݈̂ړ�
                if (isDash && isGround && !isJump)
                {
                    //�_�b�V���A�j���[�V����
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_DASH);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DASH);

                }
                else if (isCatch)
                {
                    //�L���b�`�A�j���[�V����
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
                }
                else
                {
                    //�A�C�h��(�ҋ@)�A�j���[�V����
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_IDLE);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
                }

                //�W�����v
                if (isJump && !isDash)
                {
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_JUMP);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.JUMP);
                }
            }
        }
      

       

        //�ڒn���Ă��邩����
        if (isGround == true)
        {
            //�W�����v�{�^���������ꂽ��
            if (isJump == true)
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



    /// <summary>
    /// �_�b�V���A�j���[�V�����`�F�b�N
    /// </summary>
    public void Move()
    {
        //����ł��Ȃ��ꍇ
        if (!isDead)
        {
            //�L���b�`��ԂłȂ��ꍇ
            if (!isCatch)
            {
                float dx = fixedJoystick.Horizontal; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�
                float dy = fixedJoystick.Vertical; //joystick�̐��������̓����̒l�A-1~1�̒l���Ƃ�܂�

                float rad = Mathf.Atan2(dx - 0, dy - 0); //�@ ���_(0,0)�Ɠ_�idx,dy)�̋�������p�x���Ƃ��Ă����֗��Ȋ֐�

                float deg = rad * Mathf.Rad2Deg; //radian����degreen�ɕϊ����܂�


                //�ړ��p�b�h����������Ă���ꍇ
                if (deg != 0)
                {
                    isDash = true;

                    //�E������
                    if (deg > 0) isLeft = false;

                    //��������
                    else if (deg < 0) isLeft = true;

                }
                //�ړ��p�b�h���G���Ă��Ȃ����
                else
                {
                    isDash = false;

                    if (/*isDash &&*/ !isJump)
                    {
                        isDash = false;
                    }

                    //�{�[���^�O�擾
                    searchNearBall = EnemySerch(ballTag);
                    if (searchNearBall == null)
                    {
                        //�{�[�������҃v���C���[�^�O�擾
                        searchNearBall = EnemySerch(enemyBallTag);

                    }

                    //�{�[��or�{�[�������҂��擾�ł����ꍇ
                    if (searchNearBall)
                    {
                        if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x)
                        {
                            //Debug.Log("�{�[�����E�ɋ���");
                            isLeft = true;
                        }
                        else
                        {
                            //Debug.Log("�{�[����荶�ɋ���");
                            isLeft = false;
                        }
                    }
                }
                
            }
        }

    }

    public void OnClickJump()
    {
        SEManager.Instance.Play(
           audioPath: SEPath.JUMP,      //�Đ��������I�[�f�B�I�̃p�X
           volumeRate: 1,                //���ʂ̔{��
           delay: 0,                     //�Đ������܂ł̒x������
           pitch: 1,                     //�s�b�`
           isLoop: false,                 //���[�v�Đ����邩
           callback: null                //�Đ��I����̏���
       );
        
        Debug.Log("�W�����v");
        isDash = false;
        isJump = true;
    }

    public void OnClickCatch()
    {
        isDash = false;
        isCatch = true;

        //�L���b�`�A�j���[�V����
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
        Debug.Log("�L���b�`");


        //�L���b�`��ԏ�ԉ���
        Invoke("isCatchOut", catchDelay);
    }

    void isCatchOut()
    {
        isCatch = false;
        Debug.Log("�L���b�`����");
    }
    public void OnClickThrow()
    {
        if (!isDead)
        {
            isThrow = true;



            StartCoroutine(IsThrowOut());
            Shot();
        }
    }

    //�t�F�C���g(������ӂ�)����
    public void OnClickFeint()
    {
        Debug.Log("feint");
        isFeint = true;
        //�t�F�C���g�A�j���[�V����
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);

        

        StartCoroutine(IsThrowOut());
    }
    //�t�F�C���g���Z�b�g
    IEnumerator IsThrowOut()
    {
        yield return new WaitForSeconds(throwDelay);//�P�b�҂�
        Debug.Log("������ԃ��Z�b�g");
        //�v���C���[��ԃ��Z�b�g
        isFeint = false;
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
    }

    /// <summary>
    /// �{�[�����ˏ���
    /// </summary>
    void Shot()
    {
        if (isHaveBall)
        {
            //�}�X�^�[�N���C�A���g�ɕύX
            roomModel.isMaster = true;

            GameObject newbullet = Instantiate(ballPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity); //�e�𐶐�
            Rigidbody bulletRigidbody = newbullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (transform.forward * ballSpeed); //�L�����N�^�[�������Ă�������ɒe�ɗ͂�������

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

        //���n�����o�����̂Œ��n��Ԃ���������
        if (!isGround)
        {
            SEManager.Instance.Play(
                audioPath: SEPath.JUMPED,      //�Đ��������I�[�f�B�I�̃p�X
                volumeRate: 1,                //���ʂ̔{��
                delay: 0.6f,                     //�Đ������܂ł̒x������
                pitch: 1,                     //�s�b�`
                isLoop: false,                 //���[�v�Đ����邩
                callback: null                //�Đ��I����̏���
            );

            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);

            isJump = false;
            isGround = true;
        }

        //�N���A����I�u�W�F�N�g(�f�o�b�O�p)
        if (other.gameObject.tag == "Clear")
        {
            DeadUser();
        }

        //�{�[���I�u�W�F�N�g
        if (other.gameObject.tag == "Ball")
        {
            //�L���b�`��ԂŃ{�[���ɐG������
            if (isCatch)
            {
                SEManager.Instance.Play(
                    audioPath: SEPath.CATCH,      //�Đ��������I�[�f�B�I�̃p�X
                    volumeRate: 1,                //���ʂ̔{��
                    delay: 0.6f,                     //�Đ������܂ł̒x������
                    pitch: 1,                     //�s�b�`
                    isLoop: false,                 //���[�v�Đ����邩
                    callback: null                //�Đ��I����̏���
                );
                //�m�b�N�o�b�N
                rigidbody.AddForce(-transform.forward * knockBack, ForceMode.VelocityChange);

                //�L���b�`�A�j���[�V����
                playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);
                Debug.Log("�L���b�`");

                CancelInvoke();
                //�L���b�`��ԏ�ԉ���
                Invoke("isCatchOut", 0.8f);

                //�{�[��������Ԃɂ���
                isHaveBall = true;
                Destroy(other.gameObject);    //�{�[���폜
                Debug.Log("�L���b�`����");

                catchNum++;
                GetBall();
            }
            //�L���b�`��Ԃ���Ȃ�������
            else
            {
                HitBall();

            }

        }

        //�_���[�W���̂Ȃ���Ԃ̃{�[��
        if (other.gameObject.tag == "EasyBall")
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
        isHaveBall = true;
        //�{�[�������ҕύX
        gameDirector.getUserId = roomModel.ConnectionId;
        Debug.Log("�{�[���擾");

        //�擾�҂�ID��ʒm
        await roomModel.GetBallAsync(roomModel.ConnectionId);
    }
    //�{�[���擾����
    private async void ThrowBall()
    {
        throwNum++;
        
        // �ǉ�
        //rigidbody.AddForce(-transform.forward * 3f, ForceMode.VelocityChange);

        SEManager.Instance.Play(
                    audioPath: SEPath.THROW,      //�Đ��������I�[�f�B�I�̃p�X
                    volumeRate: 1,                //���ʂ̔{��
                    delay: 0.6f,                     //�Đ������܂ł̒x������
                    pitch: 1,                     //�s�b�`
                    isLoop: false,                 //���[�v�Đ����邩
                    callback: null                //�Đ��I����̏���
                );

        //�{�[�����
        var throwData = new ThrowData()
        {
            ConnectionId = roomModel.ConnectionId,            //�ڑ�ID
            ThorwPos = this.gameObject.transform.position,    //�������v���C���[�̍��W
            GoalPos = searchNearEnemy.transform.eulerAngles,    //�ڕW���W

        };

        //�{�[�����˒ʒm
        await roomModel.ThrowBallAsync(throwData);

    }


    private async void HitBall()
    {

        //����������������return
        if (gameDirector.getUserId == roomModel.ConnectionId) {
            Debug.Log("���ŋ�");
            return;

        }

        //�m�b�N�o�b�N
        rigidbody.AddForce(-transform.forward * knockBack, ForceMode.VelocityChange);


        //HP���܂�����ꍇ
        if (hp > 0)
        {
            Debug.Log("�q�b�g");


            Debug.Log("�c��̗�:" + hp);

            //�_�E������
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);

            var hitData = new HitData()
            {
                ConnectionId = roomModel.ConnectionId,              //���Ă�ꂽ���[�U�[��ID
                EnemyId = gameDirector.enemyId,                     //���Ă����[�U�[��ID
                DamagedHP = this.hp,                                //���Ă�ꂽ���[�U�[��HP
                Point = GetPoint(),                                 //�l���|�C���g
            };

            await roomModel.HitBallAsync(hitData);

            //���g��HP����
            this.hp--;

        }
       
    }


    /// <summary>
    /// ���S�ʒm
    /// </summary>
    private async void DeadUser()
    {
        this.gameObject.tag = "Dead";

        var deadData = new DeadData()
        {
            ConnectionId = roomModel.ConnectionId,    //���[�U�[ID
            Name = userModel.userName,                //���[�U�[��
            Point = gameDirector.point,               //���[�U�[�l���|�C���g
            Time = (int)gameDirector.time,            //��������
            ThrowNum = throwNum,                      //��������
            HitNum = gameDirector.hitNum,                          //���Ă���
            CatchNum = catchNum,                      //�L���b�`������
            JoinOrder = gameDirector.JoinNum,         //�v���C���[�ԍ�
            IsLast = this.isLast,
        };

        await roomModel.DeadUserAsync(deadData,gameDirector.deadNum);
    }

    private int GetPoint()
    {
        //���Ă��G�Ǝ��g�̋������擾
        float point = Vector3.Distance(searchNearEnemy.transform.position, this.gameObject.transform.position);

        //�����_�ȉ��؂�グ
        Mathf.Ceil(point);
        Debug.Log("���_:" + point);

        //����������return
        return (int)point;
    }

    /// <summary>
    /// �w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    /// </summary>
    /// <returns></returns>
    private GameObject EnemySerch(string getTagName)
    {

        // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;

        // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
        GameObject searchTargetObj = null;

        // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
        GameObject[] objs = GameObject.FindGameObjectsWithTag(getTagName);

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


   

    //�v���C���[�̌����擾(�G�p)
    public bool GetAngle()
    {
        return isLeft;
    }
}
