using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;                 //DOTween���g���Ƃ��͂���using������
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    //�v���C���[�v���n�u
    [SerializeField] GameObject characterPrefab;
    //���[�����f��
    [SerializeField] RoomModel roomModel;

    //���[���f�[�^(�\���p)
    [SerializeField] Text roomNameText;




    //�����f�[�^(���͗p)
    [SerializeField] Text roomName;
    [SerializeField] Text userId;

    //�v���C���[�����e�I�u�W�F�N�g
    [SerializeField] private GameObject spawnObj;
    //�v���C���[�����ʒu
    [SerializeField] private Transform[] spawnPosList;

    SpriteRenderer charaNum;

    //�^�[�Q�b�g�p�J�[�\��
    [SerializeField] GameObject cursor;

    //�ҋ@��UI
    [SerializeField] GameObject standByUI;

    //���[�U�[�ҋ@UI�����e�I�u�W�F�N�g
    [SerializeField] private GameObject spawnUIObj;
    //���[�U�[�ʑҋ@UI�v���n�u
    [SerializeField] GameObject StandUIPrefab;

    //�Q�[��UI
    [SerializeField] GameObject gameUI;

    public Sprite player1;
    public Sprite player2;
    public Sprite player3;
    public Sprite player4;

    public Sprite you;

    //���U���gUI
    [SerializeField] GameObject resultUI;

    //�v���C���[���ݔ���
    private bool isPlayer;
    //�{�[�����ݔ���
    private bool isBall;

    GameObject ballObj;

    [SerializeField] GameObject ballPrefab;

    //�}�X�^�[�N���C�A���g���ǂ���
    private bool isJoinFirst;
    //Dotween�J�ڕ⊮����
    private float dotweenTime = 0.1f;


    //�Q�[�����
    public enum GAME_STATE
    {
        STOP = 0,             //��~��
        PREPARATION = 1,      //������
        READY = 2,            //����������
        COUNTDOWN = 3,        //�J�n�J�E���g��
        START = 4,            //�Q�[����
    }

    GAME_STATE game_State = GAME_STATE.STOP;

    //���[�U�[�A�j���[�V�������
    public enum ANIM_STATE
    {
        IDLE = 0,         //�A�C�h�����
        DASH,             //�_�b�V�����
        CATCH,            //�L���b�`���
        JUMP,             //�W�����v���
        DOWN,             //�_�E�����

        THROW,            //�J�n�J�E���g��
        JUMPTHROW,        //�Q�[����

    }

    ANIM_STATE anim_State = ANIM_STATE.IDLE;


    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    
    Dictionary<Guid, GameObject> standUIList = new Dictionary<Guid, GameObject>();
    void Awake()
    {
        //�t���[�����[�g�ݒ�
        Application.targetFrameRate = 60; // ������Ԃ�-1�ɂȂ��Ă���
    }
    async void Start()
    {
        

        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;   //���[�U�[����

        roomModel.LeavedUser += this.LeavedUser;       //���[�U�[�ޏo

        roomModel.MovedUser += this.MovedUser;         //���[�U�[�ړ����

        roomModel.MovedBall += this.MovedBall;         //�{�[���ړ����

        roomModel.ThrowedBall += this.ThrowedBall;

        roomModel.getBall += this.GetBall;

        roomModel.ReadyUser += this.ReadyUser;         //���[�U�[��������

        //roomModel.StartGameCount += this.GameCount;    //�Q�[�����J�E���g�J�n

        roomModel.StartGameUser += this.GameStart;     //�Q�[���J�n

        roomModel.FinishGameUser += this.GameFinish;   //�Q�[���I��

        isPlayer = false;

        isBall = false;

        isJoinFirst = false;

        cursor.SetActive(false);

        //�ҋ@
        await roomModel.ConnectAsync();

        

    }

    /// <summary>
    /// Unity�̐ݒ肩��P�b���Ƃ̒ʐM�񐔂��w��
    /// </summary>
    private async void FixedUpdate()
    {
        if (!isPlayer) return;


        var userState = new UserState()
        {
            /*isReady = ,                     //��������
            isGameCountFinish = ,           //�J�E���g�_�E���I������
            isGameFinish = ,                //�Q�[���I������
            Ranking = ,                     //���� 
            Score = ,                       //�l���X�R�A
            AnimeId = ,                     //�A�j���[�V����ID
            //AnimeId = characterList[roomModel.ConnectionId].
            */
        };

        //�ړ����
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,      //�ڑ�ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //�L�����ʒu
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //�L������]

        };

        //�v���C���[�ړ�
        await roomModel.MoveAsync(moveData);

        ballObj = GameObject.Find("Ball");

        if(!ballObj) return;

        //�{�[������ꂽ��
        if (ballObj) isBall = true;

        Debug.Log(isJoinFirst);

        //�{�[�������݂��Ă���&�}�X�^�[�N���C�A���g
        if (isBall && isJoinFirst)
        {

            Debug.Log("�}�X�^�[�{�[�����邨");


            //�{�[�����
            var moveBallData = new MoveData()
            {
                ConnectionId = roomModel.ConnectionId,      //�ڑ�ID
                Pos = ballObj.transform.position,           //�{�[���ʒu
                Rotate = ballObj.transform.eulerAngles,     //�{�[����]

            };

            //�{�[���ʒu����
            await roomModel.MoveBallAsync(moveBallData);
        }

        //���[�U�[��� 
        //await roomModel.UpdateUserStateAsync(userState);
    }



    //��������
    public async void JoinRoom()
    {
        Debug.Log("���[����:"+roomName.text);
        Debug.Log("���[�U�[ID;" + userId.text);

        cursor.SetActive(true);


        await roomModel.JoinAsync(roomName.text, int.Parse(userId.text));     //���[�����ƃ��[�U�[ID��n���ē���

        

        Debug.Log("��������");
    }

    //���[�U�[�����������Ƃ��̏���
    private void OnJoinedUser(JoinedUser user)
    {
       
        //�L�����N�^�[����
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //�C���X�^���X����

        //�ҋ@��UI����
        GameObject standByCharaUI = Instantiate(StandUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);

       
        characterList[user.ConnectionId] = characterObject;        //�t�B�[���h�ŕێ�
        
        standUIList[user.ConnectionId] = standByCharaUI;        //�t�B�[���h�ŕێ�

       

         //�v���C���[No�擾(UI)
         Image number = standByCharaUI.transform.GetChild(1).gameObject.GetComponent<Image>();


        //�v���C���[���擾
        Text name = standByCharaUI.transform.GetChild(3).gameObject.GetComponent<Text>();
        name.text = user.UserData.Name;


        //���@��ʃe�L�X�g�\��
        GameObject child = characterObject.transform.GetChild(1).gameObject;

        charaNum = child.GetComponent<SpriteRenderer>();


        //�v���C���[�i���o�[�摜�����ւ�
        switch (user.JoinOrder)
        {
            case 1:
                number.sprite = player1;
                charaNum.sprite = player1;


                

                //�}�X�^�[�N���C�A���g����
                isJoinFirst = true;

                Debug.Log("����ύX");

                break;
            case 2:
                number.sprite = player2;
                charaNum.sprite = player2;

                break;
            case 3:
                number.sprite = player3;
                charaNum.sprite = player3;

                break;
            case 4:
                standByUI.SetActive(false);
                number.sprite = player4;
                charaNum.sprite = player4;

                break;
            default:
                Debug.Log("�ϐ��");
                break;
        }

        /*if (!ballObj)
                {
                    //�{�[������
                    ballObj = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);

                    ballObj.name = "Ball";
        }*/


        if (user.ConnectionId == roomModel.ConnectionId)
        {

            //���@��No��YOU�ɒ���ւ�
            charaNum.sprite = you;

            
            //Instantiate(youPrefab, spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, characterList[roomModel.ConnectionId].gameObject.transform); //�C���X�^���X����

            characterObject.name = "MyPlay";
            //���@�p�̃X�N���v�g���^�O��ǉ�
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            characterList[roomModel.ConnectionId].tag = "Player";

            //���@���ؖ�
            isPlayer = true;

            //�����ԍ�
            Debug.Log("�����ԍ�:"+user.JoinOrder);

        }
        else
        {
            characterObject.name = "Enemy";
            //���@�ȊO�p�̃X�N���v�g���^�O��ǉ�
            characterObject.gameObject.AddComponent<EnemyManager>();
            characterObject.tag = "Enemy";

            
        }
        //�ҋ@��UI�\��
        standByUI.SetActive(true);


        child.SetActive(true);






        game_State = GAME_STATE.PREPARATION;
    }

    //�ؒf����
    public async void DisConnectRoom()
    {
        //�ޏo����
        await roomModel.LeaveAsync();

        //MagicOnion�ؒf����
        //await roomModel.DisConnectAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform player in spawnObj.transform)
        {
            Destroy(player.gameObject);
        }

        //UI�I�u�W�F�N�g�폜
        foreach (Transform ui in spawnUIObj.transform)
        {
            Destroy(ui.gameObject);
        }
        //�{�[���폜
        //Destroy(ballObj);


        isPlayer = false;

        cursor.SetActive(false);

        Debug.Log("�ޏo����");
    }

    //���[�U�[���ޏo�����Ƃ��̏���
    private async void LeavedUser(Guid connnectionId)
    {
        //�ޏo�����v���C���[�̃I�u�W�F�N�g�폜
        Destroy(characterList[connnectionId]);

        //�ޏo�����v���C���[UI�̃I�u�W�F�N�g�폜
        Destroy(standUIList[connnectionId]);

        //Destroy(ballObj);

        //�ޏo�����v���C���[�����X�g����폜
        characterList.Remove(connnectionId);

        //�ޏo�����v���C���[UI�����X�g����폜
        standUIList.Remove(connnectionId);

        Debug.Log("�ޏo�������[�U�[�ԍ�:"+ connnectionId);

        //�v���C���[��������Z�b�g
        isPlayer = false;

        //�ҋ@��UI��\��
        //standByUI.SetActive(false);

        Debug.Log("�ޏo���[�U�[�I�u�W�F�N�g�폜");
    }

    //�}�b�`���O�����������Ƃ��̏���
    private async void MatchedUser(string roomName)
    {
        //�ޏo����
        await roomModel.LeaveAsync();

        //�󂯎�������[�U�[ID�����[�����ɓn���ē���
        await roomModel.JoinAsync(roomName, int.Parse(userId.text));     

        Debug.Log("�}�b�`���O��������");
    }

    //���[�U�[���ړ������Ƃ��̏���
    private async void MovedUser(MoveData moveData)
    {
        //�v���C���[�����Ȃ�������
        if (!characterList.ContainsKey(roomModel.ConnectionId))
        {
            return;
        }

        //�ړ������v���C���[�̈ʒu���
        //characterList[moveData.ConnectionId].transform.position = moveData.Pos;

        //Dotween�ňړ��⊮
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);

        //�ړ������v���C���[�̊p�x���
        //characterList[moveData.ConnectionId].transform.eulerAngles = moveData.Rotate;

        //Dotween�ŉ�]�⊮
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);

        //�A�j���[�V�����X�V
        //characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnomation>().Move(moveData.AnimeId);

    }

    //���[�U�[���ړ������Ƃ��̏���
    private async void MovedBall(MoveData moveBallData)
    {
        //�{�[��������������
        if (!ballObj)
        {
            return;
        }
       

        //Dotween�ňړ��⊮
        ballObj.transform.DOMove(moveBallData.Pos, dotweenTime).SetEase(Ease.Linear);



        //Dotween�ŉ�]�⊮
        ballObj.transform.DORotate(moveBallData.Rotate, dotweenTime).SetEase(Ease.Linear);

        

    }

    //�{�[�����ˏ���
    private async void ThrowedBall(ThrowData throwData)
    {

    }

    //�{�[���擾����
    private async void GetBall()
    {

    }

    //���[�U�[���X�V����
    public�@async void UpdateUserState(Guid connectionId,UserState userState)
    {
        //�v���C���[�����Ȃ�������
        if (!characterList.ContainsKey(roomModel.ConnectionId))
        {
            return;   
        }


        //await roomModel.UpdateStateUser(connectionId,userState);
    }
    //���[�U�[������������
    public async void ReadyUser(bool isReady)
    {
        isReady = true;
        await roomModel.ReadyAsync(isReady);
        Debug.Log("��������");
        game_State = GAME_STATE.READY;
    }

    

    public void  GameCount()
    {
        standByUI.SetActive(false);
        gameUI.SetActive(true);
        Debug.Log("�J�E���g�_�E���J�n");

        Invoke("GameStart", 4.0f);
    }

    public async void GameStart()
    {
        
        Debug.Log("�Q�[���J�n");
        standByUI.SetActive(false);
        gameUI.SetActive(true);
        //await roomModel.StartGameAsync();
    
    }
    

    public void GameFinish(Guid connectionId, string userName, bool isFinishAllUser)
    {
        Debug.Log("�Q�[���I��");

        resultUI.SetActive(true);
    }

   
    public void OnClickHome()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
