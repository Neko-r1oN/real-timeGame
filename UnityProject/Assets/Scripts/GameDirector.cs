using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;                   //DOTween���g���Ƃ��͂���using������


public class GameDirector : MonoBehaviour
{
    //�v���C���[�v���n�u
    [SerializeField] GameObject characterPrefab;
    //���[�����f��
    [SerializeField] RoomModel roomModel;

    [SerializeField] Text roomName;
    [SerializeField] Text userId;

    //�v���C���[�����e�I�u�W�F�N�g
    [SerializeField] private GameObject spawnObg;
    //�v���C���[�����ʒu
    [SerializeField] private Transform spawnPos;

    //�^�[�Q�b�g�p�J�[�\��
    private GameObject cursor;

    //�v���C���[���ݔ���
    private bool isPlayer;
    //Dotween�J�ڕ⊮����
    private float dotweenTime = 0.1f;


    //�Q�[�����
    public enum DIRECTION_STATE
    {
        STOP = 0,             //��~��
        PREPARATION = 1,      //������
        READY = 2,            //����������
        COUNTDOWN = 3,        //�J�n�J�E���g��
        START = 4,            //�Q�[����
    }

    DIRECTION_STATE direction = DIRECTION_STATE.STOP;


    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    void Awake()
    {
        //�t���[�����[�g�ݒ�
        Application.targetFrameRate = 60; // ������Ԃ�-1�ɂȂ��Ă���
    }
    async void Start()
    {
        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.LeavedUser += this.LeavedUser;

        roomModel.MovedUser += this.MovedUser;

        roomModel.ReadiedUser += this.ReadiedUser;

        isPlayer = false;
        //�ҋ@
        await roomModel.ConnectAsync();

    }

    /// <summary>
    /// Unity�̐ݒ肩��P�b���Ƃ̒ʐM�񐔂��w��
    /// </summary>
    private async void FixedUpdate()
    {
        if (!isPlayer) return;

        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Pos = characterList[roomModel.ConnectionId].transform.position,
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,

        };

        //�ړ�
        await roomModel.MoveAsync(moveData);

    }



    //��������
    public async void JoinRoom()
    {
        Debug.Log("���[����:"+roomName.text);
        Debug.Log("���[�U�[ID;" + userId.text);


        await roomModel.JoinAsync(roomName.text, int.Parse(userId.text));     //���[�����ƃ��[�U�[ID��n���ē���

        Debug.Log("��������");
    }

    //���[�U�[�����������Ƃ��̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity, spawnPos); //�C���X�^���X����

        //characterObject.transform.position = new Vector3(0,0,0);   //���W�w��
        //characterObject.transform.parent = spawnObg.transform;
        characterList[user.ConnectionId] = characterObject;        //�t�B�[���h�ŕێ�
        

        if (user.ConnectionId == roomModel.ConnectionId)
        {
            characterObject.name = "MyPlay";
            //���@�p�̃X�N���v�g���^�O��ǉ�
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            characterList[roomModel.ConnectionId].tag = "Player";

            //�����ԍ�
            Debug.Log("�����ԍ�:"+user.JoinOrder);

            //�J�[�\���I�u�W�F�N�g�ɃX�N���v�g�ǉ�
            //cursor = GameObject.Find("Cursor");
            //cursor.gameObject.AddComponent<LockOnSystem>();
        }
        else
        {
            characterObject.name = "Enemy";
            //���@�ȊO�p�̃X�N���v�g���^�O��ǉ�
            characterObject.gameObject.AddComponent<EnemyManager>();
            characterObject.tag = "Enemy";

            
        }

        isPlayer = true;

        direction = DIRECTION_STATE.PREPARATION;
    }

    //�ؒf����
    public async void DisConnectRoom()
    {
        //�ޏo����
        await roomModel.LeaveAsync();

        //MagicOnion�ؒf����
        //await roomModel.DisConnectAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform player in spawnObg.transform)
        {
            Destroy(player.gameObject);
        }

        isPlayer = false;

        Debug.Log("�ޏo����");
    }

    //���[�U�[���ޏo�����Ƃ��̏���
    private async void LeavedUser(Guid connnectionId)
    {
        //�ޏo�����v���C���[�̃I�u�W�F�N�g�폜
        Destroy(characterList[connnectionId]);
        //�ޏo�����v���C���[�����X�g����폜
        characterList.Remove(connnectionId);

        //�v���C���[��������Z�b�g
        isPlayer = false;

        Debug.Log("�ޏo���[�U�[�I�u�W�F�N�g�폜");
    }

    //���[�U�[���ړ������Ƃ��̏���
    private async void MovedUser(MoveData moveData)
    {
        //�ړ������v���C���[�̈ʒu���
        //characterList[moveData.ConnectionId].transform.position = moveData.Pos;

        //Dotween�ňړ��⊮
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);

        //�ړ������v���C���[�̊p�x���
        //characterList[moveData.ConnectionId].transform.eulerAngles = moveData.Rotate;

        //Dotween�ŉ�]�⊮
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
    }


    public async void Ready()
    {
        await roomModel.ReadyAsync();

        direction = DIRECTION_STATE.READY;
    }
    //���[�U�[�����������������̏���
    private void ReadiedUser(JoinedUser user)
    {
        //���������摜�\��

        
    }


}
