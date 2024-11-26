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

    //�v���C���[���ݔ���
    private bool isPlayer;
    //Dotween�J�ڎ���
    private float dotweenTime = 0.1f;

    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();

    async void Start()
    {
        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.LeavedUser += this.LeavedUser;

        roomModel.MovedUser += this.MovedUser;
        
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
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();

        }
        isPlayer = true;


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

    //�Q�[�����J�n���ꂽ���̏���
    private async void StartedGame(bool isStart)
    {
        await roomModel.StartGameAsync(isStart);

        
    }

}
