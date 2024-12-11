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
using UnityEngine;
using UnityEngine.Playables;

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    //�ڑ�ID
    public Guid ConnectionId { get; set; }

    //���[�U�[�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    //���[�U�[�ؒf�ʒm
    public Action<Guid> LeavedUser { get; set; }

    //���[�U�[�}�b�`���O�ʒm
    public Action<string> MatchedUser { get; set; }

    //���[�U�[�ړ��ʒm
    public Action<MoveData> MovedUser { get; set; }

    //���[�U�[�ړ��ʒm
    public Action<MoveData> MovedBall { get; set; }

    //�{�[�����˒ʒm
    public Action<ThrowData> ThrowedBall { get; set; }

    //�{�[�����˒ʒm
    public Action getBall { get; set; }

    //���[�U�[������Ԋm�F�ʒm
    public Action<bool> ReadyUser { get; set; }

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

    //�Q�[���I���ʒm
    public Action<Guid, string, bool> FinishGameUser { get; set; }

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
            }
                OnJoinedUser(user);
            
        }
        //���[�U�[��Ԃ�������ɕύX
        userState = USER_STATE.JOIN;
    }

    //�����ʒm
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }


    //�ޏo����
    public async UniTask LeaveAsync()
    {
       �@await roomHub.LeaveAsync();

    }

    //�ޏo�ʒm
    public void Leave(Guid LeaveId)
    {
        LeavedUser(LeaveId);
    }

    //�}�b�`���O����
    public async UniTask MatchingAsync(string roomName)
    {
        //�󂯎����
        await roomHub.MatchAsync(roomName);
    }
    //�}�b�`���O�ʒm
    public void OnMatch(string roomName)
    {
        MatchedUser(roomName);
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
    public async Task GetBallAsync()
    {
        await roomHub.GetBallAsync();
    }

    //�{�[�����˒ʒm
    public void OnGetBall()
    {
        getBall();
    }

    //������������
    public async Task ReadyAsync(bool isReady)
    {
        await roomHub.ReadyAsync(isReady);
    }

    public void Ready(bool isStart)
    {
        ReadyUser(isStart);
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
    //�Q�[���I������
    public async UniTask FinishGameAsync()
    {
        await roomHub.GameFinishAsync();
        Debug.Log("�Q�[���I��");
    }
    //�Q�[���I���ʒm
    public void FinishGame(Guid connectionId,string userName,bool isFinishAllUser)
    {
        FinishGameUser(connectionId, userName, isFinishAllUser);
    }
}

