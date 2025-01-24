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
using KanKikuchi.AudioManager;
using static GameDirector;
using UnityEngine.InputSystem.XR;
using MessagePack.Resolvers;


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

    //����UI
    [SerializeField] GameObject joinUI;
    //�ҋ@��UI
    [SerializeField] GameObject standByUI;
    [SerializeField] GameObject matchText;

    //���[�U�[�ҋ@��UI�����e�I�u�W�F�N�g
    [SerializeField] private GameObject spawnUIObj;
    //���j���[UI
    [SerializeField] GameObject menuCanvas;

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

    [SerializeField] GameObject controller;
    public bool isStart;
    public float time;              //��������

    private Guid[] joinedId = new Guid[0];
    int joinNum = 0;

    //�Q�[��UI
    [SerializeField] GameObject gameUI;

    public Sprite player1;
    public Sprite player2;
    public Sprite player3;
    public Sprite player4;

    public Sprite you;

    //���U���gUI�v���n�u
    [SerializeField] GameObject resultUIPrefab;
    //���U���gUI�����ʒu
    [SerializeField] GameObject resultUIPos;

    //���U���gUI�����ʒu
    [SerializeField] GameObject resultObj;

    //�v���C���[���ݔ���
    private bool isPlayer;
    //�{�[�����ݔ���
    private bool isBall;
    private bool isMatch;

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
    public int enemyPoint;
    public int point;
    public int hitNum;

    PlayerManager playerManager;

    //���Ŗh�~�p�ϐ�
    public Guid getUserId;
    public int deadNum;
    public bool isDead;

    public int JoinNum;
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

    Color RedColor = new Color32(247, 33, 73, 255);
    Color BlueColor = new Color32(33, 112, 247, 255);
    Color GreenColor = new Color32(33, 247, 87, 255);
    Color YellowColor = new Color32(247, 247, 33, 255);



    //�L�����N�^�[���X�g
    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    //�ҋ@UI���X�g
    Dictionary<Guid, GameObject> standUIList = new Dictionary<Guid, GameObject>();
    //�X�R�AUI���X�g
    Dictionary<Guid, GameObject> scoreUIList = new Dictionary<Guid, GameObject>();
    //���U���gUI���X�g
    Dictionary<Guid, GameObject> resultUIList = new Dictionary<Guid, GameObject>();

    void Awake()
    {
        //�t���[�����[�g�ݒ�
        Application.targetFrameRate = 60; // ������Ԃ�-1�ɂȂ��Ă���
    }
    async void Start()
    {
        isMatch = false;

        //���j���[BGM
        BGMManager.Instance.Play(
            audioPath: BGMPath.MENU, //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                //�Đ������܂ł̒x������
            pitch: 1,                //�s�b�`
            isLoop: true,             //���[�v�Đ����邩
            allowsDuplicate: false             //����BGM�Əd�����čĐ������邩
        );

        //����
        BGMManager.Instance.Play(
            audioPath: BGMPath.GUEST,         //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 0.5f,                    //���ʂ̔{��
            delay: 0,                         //�Đ������܂ł̒x������
            pitch: 1,                         //�s�b�`
            isLoop: true,                     //���[�v�Đ����邩
            allowsDuplicate: true             //����BGM�Əd�����čĐ������邩
        );
        //����
        BGMManager.Instance.Play(
            audioPath: BGMPath.GUEST,         //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 0.5f,                    //���ʂ̔{��
            delay: 0.7f,                         //�Đ������܂ł̒x������
            pitch: 1,                         //�s�b�`
            isLoop: true,                     //���[�v�Đ����邩
            allowsDuplicate: true             //����BGM�Əd�����čĐ������邩
        );

        enemyPoint = 0;
        point = 0;
        hitNum = 0;

       

        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;   //���[�U�[����

        roomModel.MatchedUser += this.MatchedUser;     //�}�b�`���O

        roomModel.LeavedUser += this.LeavedUser;       //���[�U�[�ޏo

        roomModel.MovedUser += this.MovedUser;         //���[�U�[�ړ����

        roomModel.MovedBall += this.MovedBall;         //�{�[���ړ����

        roomModel.ThrowedBall += this.ThrowedBall;     //�{�[������

        roomModel.GetBall += this.GetBall;             //�{�[���擾

        roomModel.HitBall += this.HitBall;             //�{�[���q�b�g

        roomModel.MoveCursor += this.MovedCursor;

        roomModel.DownUser += this.DownUser;             //�{�[���q�b�g
        roomModel.DownBackUser += this.DownBackUser;             //�{�[���q�b�g

        roomModel.ReadyUser += this.ReadyUser;         //���[�U�[��������

        //roomModel.StartGameCount += this.GameCount;    //�Q�[�����J�E���g�J�n

        roomModel.StartGameUser += this.GameStart;     //�Q�[���J�n

        roomModel.UserDead += this.DeadUser;           //���[�U�[���S

        roomModel.FinishGameUser += this.FinishGameUser;   //�Q�[���I��


        isPlayer = false;

        isBall = false;

        isJoinFirst = false;

        cursor.SetActive(true);

        deadNum = 0;
        isDead = false;
        resultObj.SetActive(false);

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
        //���[�U�[ID�X�V(�\���p)
        /*if (userModel.userName != "")
        {
            //Debug.Log(userModel.userName);
            userName.text = userModel.userName;
        }*/

        // if (!isPlayer) return;
        
        if(isStart)
        {

            //�������ԍX�V
            time += Time.deltaTime;

        }

        //Debug.Log(game_State);
        switch (game_State)
        {
            

            case GAME_STATE.READY:
                standByUI.SetActive(true);
                break;
            case GAME_STATE.START:
                standByUI.SetActive(false);
                break;
            case GAME_STATE.STOP:
                standByUI.SetActive(false);
                break;
        }
      
    }



    //��������
    public async void JoinRoom()
    {
        //isJoinFirst = false;

        /*if (!userId)
        {
            return;
        }*/

        Debug.Log("���[����:"+roomName.text);
        Debug.Log("���[�U�[ID;" + userModel.userId);

        cursor.SetActive(true);

        game_State = GAME_STATE.READY;
        if (!isMatch)
        {

            await roomModel.JoinAsync(roomName.text, userModel.userId);     //���[�����ƃ��[�U�[ID��n���ē���

            isMatch = true;//await roomModel.JoinAsync(roomName.text, userModel.userId);
        }

        //�����ʐM�Ăяo���A�ȍ~�� commuTime ���ƂɎ��s
        InvokeRepeating(nameof(SendData), 0.0f, commuTime);

        Debug.Log("��������");
    }

    //��������
    public async void JoinLobby()
    {
        /*if (!userId)
        {
            return;
        }*/

       
        game_State = GAME_STATE.READY;

        menuCanvas.SetActive(true);
         cursor.SetActive(true);




         await roomModel.JoinLobbyAsync(userModel.userId);     //���[�����ƃ��[�U�[ID��n���ē���

        //�����ʐM�Ăяo���A�ȍ~�� commuTime ���ƂɎ��s
        //InvokeRepeating(nameof(SendData), 0.0f, commuTime);
        game_State = GAME_STATE.READY;

        Debug.Log("�}�b�`���O��");
    }

    //�}�b�`���O�����������Ƃ��̏���
    private async void MatchedUser(string roomName)
    {
      
            await roomModel.LeaveAsync();


            Debug.Log("�}�b�`:" + roomName);



        if (!isMatch)
        {
            //�󂯎�������[�U�[ID�����[�����ɓn���ē���
            await roomModel.JoinAsync(roomName, userModel.userId);
            
            //�����ʐM�Ăяo���A�ȍ~�� commuTime ���ƂɎ��s
            InvokeRepeating(nameof(SendData), 0.0f, commuTime);
            isMatch = true;
        }
    }

    //���[�U�[�����������Ƃ��̏���
    private void OnJoinedUser(JoinedUser user)
    {
        bool isJoined = false;

        if (joinedId != null)
        {
            Debug.Log("�ۑ�ID:"+joinedId);
            //�d���`�F�b�N
            foreach (Guid id in joinedId)
            {
                Debug.Log(id + ":" + user.ConnectionId);
                if (id == user.ConnectionId) return;
            }
        }
        //�}�X�^�[�`�F�b�N
        roomModel.OnMasterCheck(user);

        //�L�����N�^�[����
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //�C���X�^���X����

        //�ҋ@��UI����
        GameObject standByCharaUI = Instantiate(standUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);

        //�v���C���[�X�R�AUI����
        GameObject charaInfoUI = Instantiate(playerUIPrefab, Vector3.zero, Quaternion.identity, spawnPlayerUIObj.gameObject.transform);

        characterList[user.ConnectionId] = characterObject;        //�t�B�[���h�ŕێ�

        //�R���|�[�l���g�t�^
        characterList[user.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();


        standUIList[user.ConnectionId] = standByCharaUI;        //�t�B�[���h�ŕێ�

        scoreUIList[user.ConnectionId] = charaInfoUI;        //�t�B�[���h�ŕێ�

        
        //�����ۑ�
        Array.Resize(ref joinedId, joinedId.Length +1);
        joinedId[joinedId.Length - 1] = user.ConnectionId;
       // joinedId[joinNum] = user.ConnectionId;
        //joinNum++;

        //UI�J���[
        Image standUIColor = standByCharaUI.gameObject.GetComponent<Image>();
        Image infoUIColor = charaInfoUI.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Image>();

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
                charaInfoUI.name = "UI1";
                standUIColor.color = RedColor;
                infoUIColor.color = RedColor;
                break;
            case 2:
                number.sprite = player2;
                scoreUINumber.sprite = player2;
                charaNum.sprite = player2;
                characterObject.name = "player2";
                standByCharaUI.name = "UI2";
                charaInfoUI.name = "UI2";

                standUIColor.color = BlueColor;
                infoUIColor.color = BlueColor;
                break;
            case 3:
                number.sprite = player3;
                scoreUINumber.sprite = player3;
                charaNum.sprite = player3;
                characterObject.name = "player3";
                standByCharaUI.name = "UI3";
                charaInfoUI.name = "UI3";

                standUIColor.color = GreenColor;
                infoUIColor.color = GreenColor;
                break;
            case 4:
               
                number.sprite = player4;
                scoreUINumber.sprite = player4;
                charaNum.sprite = player4;
                characterObject.name = "player4";
                standByCharaUI.name = "UI4";
                charaInfoUI.name = "UI4";

                standUIColor.color = YellowColor;
                infoUIColor.color = YellowColor;

                break;
            default:
                Debug.Log("�ϐ��");
                break;
        }


        if (user.ConnectionId == roomModel.ConnectionId)
        {

            //���@��No��YOU�ɒ���ւ�
            //���@��ʃe�L�X�g�\��
            GameObject you = characterObject.transform.GetChild(1).transform.GetChild(0).gameObject;
            you.SetActive(true);

            JoinNum = user.JoinOrder;

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
            string enemy = "Enemy";
            characterObject.name = enemy;
            //���@�ȊO�p�̃X�N���v�g���^�O��ǉ�

            characterObject.tag = "Enemy";

            
        }
       
        child.SetActive(true);

    }

    //�ؒf����
    public async void DisConnectRoom()
    {
        isMatch = false;
        //�����ʐM����
        CancelInvoke();

        //�ޏo����
        await roomModel.LeaveAsync();

        standByUI.SetActive(false);
        menuCanvas.SetActive(false);

        
        game_State = GAME_STATE.STOP;

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

        //�X�R�AUI�I�u�W�F�N�g�폜
        foreach (Transform info in spawnPlayerUIObj.transform)
        {
            Destroy(info.gameObject);
        }


        isPlayer = false;

        //cursor.SetActive(false);

      
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

        //�ޏo�����v���C���[UI�̃I�u�W�F�N�g�폜
        Destroy(scoreUIList[connnectionId]);

        //�ޏo�����v���C���[�����X�g����폜
        characterList.Remove(connnectionId);

        //�ޏo�����v���C���[UI�����X�g����폜
        standUIList.Remove(connnectionId);

        //�ޏo�����v���C���[UI�����X�g����폜
        scoreUIList.Remove(connnectionId);

        Debug.Log("�ޏo�������[�U�[�ԍ�:"+ connnectionId);

        //�v���C���[��������Z�b�g
        isPlayer = false;

        game_State = GAME_STATE.STOP;

        Debug.Log("�ޏo���[�U�[�I�u�W�F�N�g�폜");

        isMatch = false;

        menuCanvas.SetActive(true);
    }

    private async void SendData()
    {
        /*if(roomModel.isMaster)
        {
            animNum = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerManager>().animState;
        
            Debug.Log(animNum.ToString());
        }*/
        
        //�R���|�[�l���g�t�^
        //characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().Init();

       
        //�ړ����
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,      //�ڑ�ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //�L�����ʒu
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //�L������]
            Angle = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<AngleManager> ().GetAngle(),  //�L�����N�^�[�̌���
            AnimId = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().GetAnimId(),      //�A�j���[�V����ID
        };

        //Debug.Log(moveData.AnimId);
        //�v���C���[�ړ�
        await roomModel.MoveAsync(moveData);

        //�J�[�\�����W���M
        if(roomModel.isMaster) await roomModel.MoveCursorAsync(cursor.transform.position);

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
        //�����Ă����v���C���[�����݂��Ă��Ȃ�������
        if (!characterList.ContainsKey(moveData.ConnectionId))
        {
            return;
        }

        //�R���|�[�l���g�t�^
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();

        //Dotween�ňړ��⊮
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Debug.Log("�ړ������ʂ���");
        //Dotween�ŉ�]�⊮

        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
        //Debug.Log("��]����");
        //�A�j���[�V�����X�V
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().SetEnemyAnim(moveData.AnimId);
        //Debug.Log("�A�j���[�V�����ʂ���");
        //�L�����N�^�[�̌����X�V
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<AngleManager>().SetAngle(moveData.Angle);
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
    //���[�U�[���ړ������Ƃ��̏���
    private async void MovedCursor(Vector3 cursorPos)
    {
        Debug.Log("�J�[�\����");
        //Dotween�ňړ��⊮
        cursor.transform.DOMove(cursorPos, dotweenTime).SetEase(Ease.Linear);

    }

    //�{�[�����ˏ���
    private async void ThrowedBall(ThrowData throwData)
    {
        SEManager.Instance.Play(
            audioPath: SEPath.THROW,      //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                     //�Đ������܂ł̒x������
            pitch: 1,                     //�s�b�`
            isLoop: false,                 //���[�v�Đ����邩
            callback: null                //�Đ��I����̏���
        );

        //�^�O�ύX
        characterList[throwData.ConnectionId].gameObject.tag = "Enemy";
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

        //�^�O�ύX
        characterList[getUserId].gameObject.tag = "BallPlayer";
        //�擾��ID���X�V
        this.getUserId = getUserId;

        
        Debug.Log(this.getUserId);
       
        Debug.Log("�擾��ID�X�V");
        bool isDelete = false;
        //�t�B�[���h��̃{�[������
        while (isDelete)
        {
            ballObj = GameObject.Find("Ball");

            if (ballObj)
            {
                Destroy(ballObj.gameObject);    //�{�[���폜
                isDelete = true;
            }
        }

        ballObj = GameObject.Find("Ball");

        if (ballObj) Destroy(ballObj.gameObject);    //�{�[���폜

        //�}�X�^�[�N���C�A���g���Z�b�g
        roomModel.isMaster = false;

        if(getUserId == roomModel.ConnectionId) roomModel.isMaster = true;
    }

    public async void HitBall(HitData hitData)
    {
        SEManager.Instance.Play(
            audioPath: SEPath.HIT,        //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                     //�Đ������܂ł̒x������
            pitch: 1,                     //�s�b�`
            isLoop: false,                 //���[�v�Đ����邩
            callback: null                //�Đ��I����̏���
        );

        SEManager.Instance.Play(
            audioPath: SEPath.GUEST_HIT,  //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                     //�Đ������܂ł̒x������
            pitch: 1,                     //�s�b�`
            isLoop: false,                 //���[�v�Đ����邩
            callback: null                //�Đ��I����̏���
        );


        //�c�@���X�g
        GameObject lifeList = scoreUIList[hitData.ConnectionId].transform.GetChild(5).gameObject;
        //�c�@�폜
        Destroy(lifeList.transform.GetChild(0).gameObject);

        //�|�C���g���f
        Text playerPoint = scoreUIList[hitData.EnemyId].transform.GetChild(4).gameObject.GetComponent<Text>();

        enemyPoint = int.Parse(playerPoint.text);

        enemyPoint += hitData.Point;
        playerPoint.text = enemyPoint.ToString();

        if (roomModel.ConnectionId == hitData.EnemyId)
        {
            point += hitData.Point;
            hitNum++;
        }

        Debug.Log("�l���|�C���g:" + hitData.Point);
        //���Ă����[�U�[�̓��_���Z

        Debug.Log("�q�b�g");
    }
        
    public async void DownUser(Guid downUserId)
    {
        CapsuleCollider hitBox;
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //�R���C�_�[�擾
        hitBox.isTrigger = true; //�g���K�[�I�t(�{�[�����˕\��)

        characterList[downUserId].gameObject.tag = "Down";

        Debug.Log(characterList[downUserId].name + ":�_�E��");

        StartCoroutine(PiyoPiyo(downUserId));
    }

    IEnumerator PiyoPiyo(Guid id)
    {
        yield return new WaitForSeconds(0.7f);//�P�b�҂�

        GameObject piyo = characterList[id].gameObject.transform.GetChild(2).gameObject;   //�R���C�_�[�擾

        MeshRenderer rend = piyo.GetComponent<MeshRenderer>();
        rend.enabled = true;
        //piyo.SetActive(true);
    }
    //�_�E�����A����
    public async void DownBackUser(Guid downUserId)
    {
        CapsuleCollider hitBox;
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //�R���C�_�[�擾
        hitBox.isTrigger = false; //�g���K�[�I��(�{�[�����˕\��)

        //�s���s����\��
        GameObject piyo = characterList[downUserId].gameObject.transform.GetChild(2).gameObject;   //�R���C�_�[�擾


        MeshRenderer rend = piyo.GetComponent<MeshRenderer>();
        rend.enabled = false;

        //piyo.SetActive(false);

        //���@�������ꍇ
        if (roomModel.ConnectionId == downUserId) characterList[downUserId].gameObject.tag = "Player";    //Pleyer�^�O��

        else characterList[downUserId].gameObject.tag = "Enemy";

        Debug.Log(characterList[downUserId].name + ":�_�E�����A");
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
        Text text = matchText.GetComponent<Text>();    
        matchText.GetComponent<TextManager>().enabled = false;

        text.text = "�}�b�`���O����";

        StartCoroutine(StartCount());
    }

    IEnumerator StartCount()
    {
        yield return new WaitForSeconds(2f);//�P�b�҂�

        joinUI.SetActive(false);

        BGMManager.Instance.Stop(BGMPath.MENU);


        BGMManager.Instance.Play(
            audioPath: BGMPath.BUTTLE, //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                //�Đ������܂ł̒x������
            pitch: 1,                //�s�b�`
            isLoop: true,             //���[�v�Đ����邩
            allowsDuplicate: true             //����BGM�Əd�����čĐ������邩
        );

        SEManager.Instance.Play(
            audioPath: SEPath.COUNT_DOWN, //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0.0f,                     //�Đ������܂ł̒x������
            pitch: 1,                     //�s�b�`
            isLoop: false,                 //���[�v�Đ����邩
            callback: null                //�Đ��I����̏���
        );



        standByUI.SetActive(false);
        game_State = GAME_STATE.START;

        Debug.Log("�Q�[���J�n");

        gameUI.SetActive(true);

        isStart = true;
        Debug.Log("toutatu");
        //���U���g�\��
       
    }


    public void DeadUser(DeadData deadData,int deadNum)
    {

        //�Ō�̃v���C���[�ȊO�͍Đ�
        if (!deadData.IsLast)
        {

            SEManager.Instance.Play(
            audioPath: SEPath.GUEST_HIT,  //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 1,                //���ʂ̔{��
            delay: 0,                     //�Đ������܂ł̒x������
            pitch: 1,                     //�s�b�`
            isLoop: false,                 //���[�v�Đ����邩
            callback: null                //�Đ��I����̏���
        );


            SEManager.Instance.Play(
                audioPath: SEPath.DOWN,        //�Đ��������I�[�f�B�I�̃p�X
                volumeRate: 1,                //���ʂ̔{��
                delay: 1,                     //�Đ������܂ł̒x������
                pitch: 1,                     //�s�b�`
                isLoop: false,                 //���[�v�Đ����邩
                callback: null                //�Đ��I����̏���
            );
        }

        if (deadData.ConnectionId == roomModel.ConnectionId)
        {
            //���S����
            isDead = true;
        }

        //���S���[�U�[�̃^�O�ύX
        characterList[deadData.ConnectionId].tag = "Dead";

        //���S�l���X�V
        this.deadNum = deadNum;

        //�c�@���X�g
        GameObject lifeList = scoreUIList[deadData.ConnectionId].transform.GetChild(5).gameObject;
        //���S�҂̎c�@���X�g�폜
        Destroy(lifeList);


        //���S�҂̃��U���gUI����
        GameObject resultUI = Instantiate(resultUIPrefab, Vector3.zero, Quaternion.identity, resultUIPos.gameObject.transform);
        //���U���gUI��Image�R���|�[�l���g�擾
        Image UIColor = resultUI.GetComponent<Image>();

        resultUIList[deadData.ConnectionId] = resultUI;        //�t�B�[���h�ŕێ�

        //�v���C���[No�擾(UI)
        Image number = resultUI.transform.GetChild(4).gameObject.GetComponent<Image>();

        //���U���g�ڍ׏��擾
        GameObject detailList = resultUI.transform.GetChild(5).gameObject;

        //�v���C���[���擾
        Text name = resultUI.transform.GetChild(3).gameObject.GetComponent<Text>();
        name.text = deadData.Name;

        //�ڍ׏��
        
        //�|�C���g
        Text pointText = detailList.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Text>();
        pointText.text = deadData.Point.ToString();

        //��������
        Text timeText = detailList.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>();
        int minutes = 0;  //��
        int seccond = 0;  //�b

        //�Ōォ
        Debug.Log(deadData.IsLast);
       
        //�Ō�̃v���C���[�������ꍇ
        if(deadData.IsLast)
        {

            timeText.text = "--:--";
        }
        else
        {
            

            while (deadData.Time >= 60)
            {
                deadData.Time -= 60;
                minutes++;
            }
            seccond = deadData.Time;

            //�e�L�X�g��0�l��
            var minutesText = minutes.ToString("D2");
            var seccondText = seccond.ToString("D2");

            timeText.text = minutesText + ":" + seccondText;
        }
        
        //��������
        Text throwText = detailList.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>();
        throwText.text = deadData.ThrowNum.ToString();

        //���Ă���
        Text hitText = detailList.transform.GetChild(3).transform.GetChild(1).gameObject.GetComponent<Text>();
        hitText.text = deadData.HitNum.ToString();

        //�L���b�`��
        Text catchText = detailList.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Text>();
        catchText.text = deadData.CatchNum.ToString();


        //�v���C���[�i���o�[�摜�����ւ�
        switch (deadData.JoinOrder)
        {
            case 1:
                number.sprite = player1;
                resultUI.name = "player1";
                UIColor.color = RedColor;
                break;
            case 2:
                number.sprite = player2;
                resultUI.name = "player2";
                UIColor.color = BlueColor;


                break;
            case 3:
                number.sprite = player3;
                resultUI.name = "player3";
                UIColor.color = GreenColor;

                break;
            case 4:
                number.sprite = player4;
                resultUI.name = "player4";
                UIColor.color = YellowColor;

                break;
            default:
                Debug.Log("�s���B�_");
                break;
        }

    }

    public void FinishGameUser()
    {
        Debug.Log("�Q�[���I���ʒm");

        // Coroutine�i�R���[�`���j���J�n
        StartCoroutine(FinishGame());
    }
    IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1f);//�P�b�҂�

        Debug.Log("toutatu");
        //���U���g�\��
        resultObj.SetActive(true);
    }
   
    public async void OnClickHome()
    {
        //�����ʐM����
        CancelInvoke();

        //�ޏo����
        await roomModel.LeaveAsync();

        //MagicOnion�ؒf����
        await roomModel.DisConnectAsync();

        //�V�[���ēǂݍ���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
