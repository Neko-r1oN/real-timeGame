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
using System.Xml.Serialization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;


public class GameDirector : MonoBehaviour
{
    //�v���C���[�v���n�u
    [SerializeField] GameObject characterPrefab;
    //���[�����f��
    [SerializeField] RoomModel roomModel;


    //���[���f�[�^(�\���p)
    [SerializeField] Text roomNameText;
    [SerializeField] Text userName;

    private UserModel userModel;
    //private UserModel userModel;

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

    //���[�U�[�ҋ@��UI�����e�I�u�W�F�N�g
    [SerializeField] private GameObject spawnUIObj;


    //���[�U�[�X�R�A�\��UI�v���n�u
    [SerializeField] GameObject playerUIPrefab;
    //���[�U�[�X�R�A�\��UI�I�u�W�F�N�g
    [SerializeField] GameObject spawnPlayerUIObj;

    //���C�t�v���n�u
    [SerializeField] GameObject lifePrefab;

    //���[�U�[�ʑҋ@UI�v���n�u
    [SerializeField] GameObject standUIPrefab;
    //�X�R�A�\��UI�����e�I�u�W�F�N�g
    [SerializeField] GameObject spawnStandUIObj;


   

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

    //�}�X�^�[�N���C�A���g����
    private bool isJoinFirst;

    //�I�u�W�F�N�g�ړ��J�ڕ⊮����
    private float dotweenTime = 0.1f;
    //�T�[�o�[�ʐM����
    private float commuTime = 0.02f;

    private int animNum;

    public Guid enemyId;     //�GID�ۑ��p
    public int point;

    PlayerManager playerManager;

    //���ŋ��h�~�p�ϐ�
    public Guid getUserId;

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

    
    //�L�����N�^�[���X�g
    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    //�ҋ@UI���X�g
    Dictionary<Guid, GameObject> standUIList = new Dictionary<Guid, GameObject>();
    //�X�R�AUI���X�g
    Dictionary<Guid, GameObject> scoreUIList = new Dictionary<Guid, GameObject>();

    void Awake()
    {
        //�t���[�����[�g�ݒ�
        Application.targetFrameRate = 60; // ������Ԃ�-1�ɂȂ��Ă���
    }
    async void Start()
    {
       

        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;   //���[�U�[����

        roomModel.MatchedUser += this.MatchedUser;     //�}�b�`���O

        roomModel.LeavedUser += this.LeavedUser;       //���[�U�[�ޏo

        roomModel.MovedUser += this.MovedUser;         //���[�U�[�ړ����

        roomModel.MovedBall += this.MovedBall;         //�{�[���ړ����

        roomModel.ThrowedBall += this.ThrowedBall;     //�{�[������

        roomModel.GetBall += this.GetBall;             //�{�[���擾

        roomModel.HitBall += this.HitBall;             //�{�[���q�b�g

        roomModel.ReadyUser += this.ReadyUser;         //���[�U�[��������

        //roomModel.StartGameCount += this.GameCount;    //�Q�[�����J�E���g�J�n

        roomModel.StartGameUser += this.GameStart;     //�Q�[���J�n

        roomModel.FinishGameUser += this.GameFinish;   //�Q�[���I��


        isPlayer = false;

        isBall = false;

        isJoinFirst = false;

        cursor.SetActive(false);

        //���[�U�[���f�����擾
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //���[�U�[ID�\��
        if (userModel.userName != "")
        {
            Debug.Log(userModel.userName);
            userName.text = userModel.userName;
        }


        //�ҋ@
        await roomModel.ConnectAsync();

        

    }

    void Update()
    {
        if (!isPlayer) return;
        
       /* if(game_State == GAME_STATE.PREPARATION)
        {
            standByUI.SetActive(true);
        }
        else
        {
            standByUI.SetActive(false);
        }*/

       
    }



    //��������
    public async void JoinRoom()
    {
        //isJoinFirst = false;

        if (!userId)
        {
            return;
        }

        Debug.Log("���[����:"+roomName.text);
        Debug.Log("���[�U�[ID;" + userModel.userId);

        cursor.SetActive(true);

        game_State = GAME_STATE.PREPARATION;

        await roomModel.JoinAsync(roomName.text, userModel.userId);     //���[�����ƃ��[�U�[ID��n���ē���
        //await roomModel.JoinAsync(roomName.text, userModel.userId);


        //�����ʐM�Ăяo���A�ȍ~��0.02f���ƂɎ��s
        InvokeRepeating(nameof(SendData), 0.0f, commuTime);

        Debug.Log("��������");
    }

    //��������
    public async void JoinLobby()
    {
        if (!userId)
        {
            return;
        }

        Debug.Log("���r�[�N���b�N");
        Debug.Log("���[�U�[ID;" + userId.text);

        cursor.SetActive(true);

        


        await roomModel.JoinLobbyAsync(userModel.userId);     //���[�����ƃ��[�U�[ID��n���ē���

        game_State = GAME_STATE.PREPARATION;

        Debug.Log("�}�b�`���O��");
    }

    //�}�b�`���O�����������Ƃ��̏���
    private async void MatchedUser(string roomName)
    {
        await roomModel.LeaveAsync();


        Debug.Log("�������郋�[����:"+roomName);

        

        //�󂯎�������[�U�[ID�����[�����ɓn���ē���
        await roomModel.JoinAsync(roomName, userModel.userId);

       

        Debug.Log("�}�b�`���O��������");
    }

    //���[�U�[�����������Ƃ��̏���
    private void OnJoinedUser(JoinedUser user)
    {
        //�}�X�^�[�`�F�b�N
        roomModel.OnMasterCheck(user);

        //�L�����N�^�[����
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //�C���X�^���X����

        //�ҋ@��UI����
        GameObject standByCharaUI = Instantiate(standUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);

        //�v���C���[�X�R�AUI����
        GameObject charaInfoUI = Instantiate(playerUIPrefab, Vector3.zero, Quaternion.identity, spawnPlayerUIObj.gameObject.transform);

        characterList[user.ConnectionId] = characterObject;        //�t�B�[���h�ŕێ�
        
        standUIList[user.ConnectionId] = standByCharaUI;        //�t�B�[���h�ŕێ�

        scoreUIList[user.ConnectionId] = charaInfoUI;        //�t�B�[���h�ŕێ�



        //�v���C���[No�擾(UI)
        Image number = standByCharaUI.transform.GetChild(1).gameObject.GetComponent<Image>();

        //�v���C���[���擾
        Text name = standByCharaUI.transform.GetChild(3).gameObject.GetComponent<Text>();
        name.text = user.UserData.Name;

        //�v���C���[���UI�擾(UI)
        Image scoreUINumber = charaInfoUI.transform.GetChild(3).gameObject.GetComponent<Image>();
        Text infoName = charaInfoUI.transform.GetChild(1).gameObject.GetComponent<Text>();
        infoName.text = user.UserData.Name;

        //���C�t����
        /*GameObject lifePos = charaInfoUI.transform.GetChild(5).gameObject.GetComponent<GameObject>();

        for(int i = 1;i <= playerManager.hp; i++)
        {//�ݒ蕪UI����
            lifePos = Instantiate(lifePrefab, Vector3.zero, Quaternion.identity, lifePos.gameObject.transform);
        }
        */
        /*//HP(�f�t�H���g)
        Text playerPoint = charaInfoUI.transform.GetChild(4).gameObject.GetComponent<Text>();
        playerPoint.text = playerManager.hp.ToString();*/
        //���@��ʃe�L�X�g�\��
        GameObject child = characterObject.transform.GetChild(1).gameObject;

        charaNum = child.GetComponent<SpriteRenderer>();

        

        //�v���C���[�i���o�[�摜�����ւ�
        switch (user.JoinOrder)
        {
            case 1:
                number.sprite = player1;
                scoreUINumber.sprite = player1;
                charaNum.sprite = player1;
                characterObject.name = "player1";
                standByCharaUI.name = "UI1";


                break;
            case 2:
                number.sprite = player2;
                scoreUINumber.sprite = player2;
                charaNum.sprite = player2;
                characterObject.name = "player2";
                standByCharaUI.name = "UI2";

                break;
            case 3:
                number.sprite = player3;
                scoreUINumber.sprite = player3;
                charaNum.sprite = player3;
                characterObject.name = "player3";
                standByCharaUI.name = "UI3";
                break;
            case 4:
               
                number.sprite = player4;
                scoreUINumber.sprite = player4;
                charaNum.sprite = player4;
                characterObject.name = "player3";
                /*standByUI.SetActive(false);
                game_State = GAME_STATE.START;*/
                Debug.Log("4�l�ڒʉ�");
                standByCharaUI.name = "UI4";

                break;
            default:
                Debug.Log("�ϐ��");
                break;
        }


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
            string enemy = "Enemy"/* + user.JoinOrder*/;
            characterObject.name = enemy;
            //���@�ȊO�p�̃X�N���v�g���^�O��ǉ�
            characterObject.gameObject.AddComponent<EnemyManager>();
            characterObject.tag = "Enemy";

            
        }
       
        child.SetActive(true);

    }

    //�ؒf����
    public async void DisConnectRoom()
    {

        //�����ʐM����
        CancelInvoke();

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
       

        isPlayer = false;

        cursor.SetActive(false);

        Debug.Log("�ޏo����");
    }

    //���[�U�[���ޏo�����Ƃ��̏���
    private async void LeavedUser(Guid connnectionId)
    {
        //�v���C���[�����Ȃ�������
       if (!characterList.ContainsKey(connnectionId))
        {
            return;
        }
       
       


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

        game_State = GAME_STATE.STOP;

        Debug.Log("�ޏo���[�U�[�I�u�W�F�N�g�폜");
    }

    private async void SendData()
    {
        /*if(roomModel.isMaster)
        {
            animNum = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerManager>().animState;
        
            Debug.Log(animNum.ToString());
        }*/
        //�ړ����
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,      //�ڑ�ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //�L�����ʒu
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //�L������]
            AnimId = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().GetAnimId(),      //�A�j���[�V����ID
        };

        //Debug.Log(moveData.AnimId);
        //�v���C���[�ړ�
        await roomModel.MoveAsync(moveData);



        /* var userState = new UserState()
        {
            isReady = false,                     //��������
            isGameCountFinish = false,           //�J�E���g�_�E���I������
            isGameFinish = false,                //�Q�[���I������
            Ranking = 0,                         //���� 
            Score = 0,                           //�l���X�R�A
           

        };

        //���[�U�[��� 
        await roomModel.UpdateUserStateAsync(userState);*/


        //�t�B�[���h��̃{�[������
        ballObj = GameObject.Find("Ball");

        if (!ballObj) return;

        //�{�[������ꂽ��
        if (ballObj) isBall = true;

        //�{�[�������݂��Ă���&�}�X�^�[�N���C�A���g
        if (isBall && roomModel.isMaster)
        {
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
    }

    //���[�U�[���ړ������Ƃ��̏���
    private async void MovedUser(MoveData moveData)
    {
        //�v���C���[�����Ȃ�������
        if (!characterList.ContainsKey(moveData.ConnectionId))
        {
            return;
        }

        //�ړ������v���C���[�̈ʒu���
        //characterList[moveData.ConnectionId].transform.position = moveData.Pos;

        //�ړ������v���C���[�̊p�x���a
        //characterList[moveData.ConnectionId].transform.eulerAngles = moveData.Rotate;

        //Dotween�ňړ��⊮
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Dotween�ŉ�]�⊮
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
        //�A�j���[�V�����X�V
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().SetEnemyAnim(moveData.AnimId);

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
        //���������[�U�[��ID��ۑ�
        enemyId = throwData.ConnectionId;

        //���������W�ɋʂ𐶐�
        Vector3 pos = characterList[throwData.ConnectionId].transform.position;
        GameObject newbullet = Instantiate(ballPrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity); //�e�𐶐�


        //�Ō�ɓ������l�Ƀ}�X�^�[�N���C�A���g�������n����\��

        Debug.Log("�{�[���ʒm�󂯂���");
    }

    //�{�[���擾����
    private async void GetBall(Guid getUserId)
    {
        Debug.Log(getUserId);

        //�擾��ID���X�V
        this.getUserId = getUserId;

        
        Debug.Log(this.getUserId);
       
        Debug.Log("�擾��ID�X�V");
        //�t�B�[���h��̃{�[������
        ballObj = GameObject.Find("Ball");

        Destroy(ballObj.gameObject);    //�{�[���폜

        //�}�X�^�[�N���C�A���g���Z�b�g
        roomModel.isMaster = false;
    }

    public async void HitBall(HitData hitData)
    {
        //�c�@���X�g
        GameObject lifeList = scoreUIList[hitData.ConnectionId].transform.GetChild(5).gameObject ;
        //�c�@�폜
        Destroy(lifeList.transform.GetChild(0).gameObject);

        //�|�C���g���f
        Text playerPoint = scoreUIList[hitData.EnemyId].transform.GetChild(4).gameObject.GetComponent<Text>();

        point = int.Parse(playerPoint.text);

        point += hitData.Point;
        playerPoint.text = point.ToString();

        Debug.Log("�l���|�C���g:" + hitData.Point);
        //���Ă����[�U�[�̓��_���Z

        Debug.Log("�q�b�g");
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
        game_State = GAME_STATE.START;
        gameUI.SetActive(true);
       
        Debug.Log("�J�E���g�_�E���J�n");

        Invoke("GameStart", 4.0f);
    }

    public async void GameStart()
    {
        standByUI.SetActive(false);
        game_State = GAME_STATE.START;

        Debug.Log("�Q�[���J�n");
       
        gameUI.SetActive(true);
        //await roomModel.StartGameAsync();
       

    }
    

    public void GameFinish(Guid connectionId, string userName, bool isFinishAllUser)
    {
        Debug.Log("�Q�[���I��");

        
        resultUI.SetActive(true);
    }

   
    public async void OnClickHome()
    {
        //�ޏo
        await roomModel.LeaveAsync();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
