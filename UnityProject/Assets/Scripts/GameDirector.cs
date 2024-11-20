using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();

    async void Start()
    {
        //���[�U�[�����������ۂ�OnJoinedUser���]�b�g�����s����悤�Ƀ��f���ɓo�^���Ă���
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.LeavedUser += this.LeavedUser;

        //�ҋ@
        await roomModel.ConnectAsync();
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

        Debug.Log("�ޏo����");
    }

    //���[�U�[���ޏo�����Ƃ��̏���
    private async void LeavedUser(Guid connnectionId)
    {
        //�ޏo�����v���C���[�̃I�u�W�F�N�g�폜
        Destroy(characterList[connnectionId]);
        //�ޏo�����v���C���[�����X�g����폜
        characterList.Remove(connnectionId);
    }
}
