////////////////////////////////////////////////////////////////////////////
///
///  ���[�����f���X�N���v�g
///  Author : ������C  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// ���[�������N���X
/// </summary>
public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    private int userId;

    public bool isMaster;

    //�ڑ�ID
    public Guid ConnectionId { get; set; }
    //���[�U�[�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser {  get; set; }
    //���[�U�[�ؒf�ʒm
    public Action<Guid> LeavedUser { get; set; }
    //���[�U�[�}�b�`���O�ʒm
    public Action<string> MatchedUser { get; set; }
    //�}�X�^�[�`�F�b�N
    public Action MasterCheckedUser { get; set; }


    //���[�U�[�ړ��ʒm
    public Action<MoveData> MovedUser { get; set; }
    //�{�[���ړ��ʒm
    public Action<MoveData> MovedBall { get; set; }
    //�{�[�����˒ʒm
    public Action<ThrowData> ThrowedBall { get; set; }
    //�{�[���l���ʒm
    public Action<Guid> GetBall { get; set; }

    //�{�[���q�b�g�ʒm
    public Action<HitData> HitBall { get; set; }
    //�J�[�\���ړ��ʒm
    public Action<Vector3> MoveCursor { get; set; }
    //���[�U�[�_�E���ʒm
    public Action<Guid> DownUser { get; set; }
    //���[�U�[�_�E�����A�ʒm
    public Action<Guid> DownBackUser { get; set; }

    //�����J�ڒʒm
    public Action StandUser { get; set; }
    //���[�U�[������Ԋm�F�ʒm
    public Action<Guid,bool> ReadyUser { get; set; }
    //�Q�[���J�E���g�J�n�ʒm
    public Action StartGameCount { get; set; }
    //�J�E���g�_�E���ʒm
    public Action<int> GameCountUser { get; set; }
    //�Q�[���J�E���g�I���ʒm
    public Action FinishGameCount { get; set; }
    //�Q���ґS���̃Q�[�����J�E���g�_�E���I���ʒm
    public Action FinishGameCountAllUser { get; set; }
    //���[�U�[��ԍX�V�ʒm
    public Action<Guid,UserState> UpdateStateUser { get; set; }
    //���[�U�[���S�ʒm
    public Action<DeadData, int> UserDead { get; set; }
    //�Q�[���I���ʒm
    public Action FinishGameUser { get; set; }
    //�Q�[���J�n�ʒm
    public Action StartGameUser { get; set; }

    //���[�U�[���
    public enum USER_STATE
    {
        NONE = 0,             //��~��
        CONNECT = 1,          //�ڑ���
        JOIN = 2,             //������
        LEAVE = 3,            //�ޏo��
    }

    USER_STATE userState = USER_STATE.NONE;

    //MoajicOnion�ڑ�����
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() {Http2Only = true};
        channel = GrpcChannel.ForAddress(ServerURL,new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel,this);
    
        //���[�U�[��Ԃ�ڑ����ɕύX
        userState = USER_STATE.CONNECT;

        //�}�X�^�[�N���C�A���g����
        isMaster = false;
    }

    //MagicOnion�ؒf����
    public async UniTask DisConnectAsync()
    {
        if(roomHub != null)
        {
            await roomHub.DisposeAsync();
        }
        if(channel != null)
        {
            await channel.ShutdownAsync();
        }

        roomHub = null;channel = null;

        //���[�U�[��Ԃ��~���ɕύX
        userState = USER_STATE.NONE;
    }

    //�j������
    async void OnDestroy()
    {
        await DisConnectAsync();
    }

    //��������
    public async UniTask JoinAsync(string roomName, int userId)
    {
        //�z��Ɉ����Ŏ󂯎��������ǉ�
        JoinedUser[] users = await roomHub.JoinAsync(roomName,userId);

        //�z��̗v�f�����[�v
        foreach (var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;

                Debug.Log(this.ConnectionId);
            }
            OnMasterCheck(user);

            //
            OnJoinedUser(user);
        }

        //���[�U�[��Ԃ�������ɕύX
        userState = USER_STATE.JOIN;

        Debug.Log("������");
    }

    //�����ʒm
    public void OnJoin(JoinedUser user)
    {
        //null����Ȃ���������s(null�������Z�q)
        OnJoinedUser?.Invoke(user);
    }

   
    //���r�[����
    public async UniTask JoinLobbyAsync(int userId)
    {
        //�z��Ɉ����Ŏ󂯎��������ǉ�
        await roomHub.JoinLobbyAsync(userId);

        Debug.Log("���r�[����");
    }
    //�����ʒm
    public void OnJoinLobby(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    //�}�b�`���O�ʒm
    public void OnMatch(string roomName)
    {
        MatchedUser(roomName);
        Debug.Log("�}�b�`���O����:"+  roomName);
    }


    //�ޏo����
    public async UniTask LeaveAsync()
    {
        //�}�X�^�[����폜
        isMaster = false;
        await roomHub.LeaveAsync();
    }

    //�ޏo�ʒm
    public void Leave(Guid LeaveId)
    {
        LeavedUser(LeaveId);
    }


    //�}�X�^�[�N���C�A���g���菈��
    public void OnMasterCheck(JoinedUser user)
    {
        //�}�X�^�[�N���C�A���g����
        if (user.ConnectionId == this.ConnectionId && user.JoinOrder == 1)
        {
            Debug.Log(user.UserData.Name + "�}�X�^�[");
            isMaster = true;
        }
        else Debug.Log(user.UserData.Name + "�}�X�^�[����Ȃ�");
    }

    //�v���C���[�ړ�����
    public async Task MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }
    //�ړ��ʒm
    public void OnMove(MoveData moveData)
    {
        MovedUser(moveData);
    }

    //�{�[�����W��������
    public async Task MoveBallAsync(MoveData moveBallData)
    {
        await roomHub.MoveBallAsync(moveBallData);
    }
    //�{�[�����W�����ʒm
    public void OnMoveBall(MoveData moveBallData)
    {
        MovedBall(moveBallData);
    }

    //�{�[�����ˏ���
    public async Task ThrowBallAsync(ThrowData ThrowData)
    {
        await roomHub.ThrowBallAsync(ThrowData);
    }

    //�{�[�����˒ʒm
    public void OnThrowBall(ThrowData ThrowData)
    {
        ThrowedBall(ThrowData);
    }

    //�{�[���擾����
    public async Task GetBallAsync(Guid getUserId)
    {
        //���g�̐ڑ�ID��ʒm
        await roomHub.GetBallAsync(getUserId); 
    }

    //�{�[�����˒ʒm
    public void OnGetBall(Guid getUserId)
    {
        GetBall(getUserId);
    }

    //�q�b�g����
    public async Task HitBallAsync(HitData hitData)
    {
        await roomHub.HitBallAsync(hitData);
    }
    //�q�b�g�ʒm
    public void OnHitBall(HitData hitData)
    {
        HitBall(hitData);
    }

    //�W���J�[�\�����W��������
    public async Task MoveCursorAsync(Vector3 cursorPos)
    {
        await roomHub.MoveCursorAsync(cursorPos);
    }
    //�W���J�[�\�����W�����ʒm
    public void OnMoveCursor(Vector3 cursorPos)
    {
        MoveCursor(cursorPos);
    }

    //�_�E������
    public async Task DownUserAsync(Guid downUserId)
    {
        await roomHub.DownUserAsync(downUserId);
    }
    //�_�E���ʒm
    public void OnDownUser(Guid downUserId)
    {
        DownUser(downUserId);
    }
    //�_�E�����A����
    public async Task DownBackUserAsync(Guid downUserId)
    {
        await roomHub.DownBackUserAsync(downUserId);
    }
    //�_�E�����A�ʒm
    public void OnDownBackUser(Guid downUserId)
    {
        DownBackUser(downUserId);
    }

    //������ԑJ��
    public void OnStand()
    {
        StandUser();
    }

    //������������
    public async Task ReadyAsync(Guid id ,bool isReady)
    {
        await roomHub.ReadyAsync(id,isReady);
    }
    //�ʒm
    public void Ready(Guid id, bool isStart)
    {
        ReadyUser(id, isStart);
    }
    //���������ʒm
   
    //���[�U�[���X�V����
    public async UniTask UpdateUserStateAsync(UserState state)
    {
        await roomHub.UpdateUserStateAsync(state);
    }
    //���[�U�[���X�V�ʒm
    public void UpdateUserState(Guid connectionId,UserState state)
    {
        UpdateStateUser(connectionId, state);
    }


    //���g�̃Q�[�����J�E���g�_�E���I������
    public async UniTask FinishGameCountAsync()
    {
        await roomHub.GameCountFinishAsync();
    }
    //�S���̃Q�[�����J�E���g�_�E���I���ʒm
    public void GameCountFinish()
    {
        FinishGameCountAllUser();
    }

    //�J�E���g�_�E����������
    public async UniTask GameCountAsync(int currentTime)
    {
        await roomHub.GameCountAsync(currentTime);
    }
    //�J�E���g�_�E�������ʒm
    public void GameCount(int currentTime)
    {
        GameCountUser(currentTime);
    }

    // �Q�[���J�n�ʒm
    public void StartGame()
    {
        StartGameUser();
    }

    //���[�U�[���S����
    public async UniTask DeadUserAsync(DeadData deadData,int deadNum)
    {
        await roomHub.DeadUserAsync(deadData,deadNum);
    }
    //���[�U�[���S�ʒm
    public void DeadUser(DeadData deadData, int deadNum)
    {
        UserDead(deadData, deadNum);
    }

    //�Q�[���I���ʒm
    public void FinishGame()
    {
        Debug.Log("�I���ʒm�󂯎��");
        FinishGameUser();
    }
}

