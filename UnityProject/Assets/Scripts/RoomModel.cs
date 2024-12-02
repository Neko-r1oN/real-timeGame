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

    //���[�U�[�ړ��ʒm
    public Action<MoveData> MovedUser { get; set; }

    //���[�U�[���������ʒm
    public Action<JoinedUser> ReadiedUser { get; set; }




    //MoajicOnion�ڑ�����
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() {Http2Only = true};
        channel = GrpcChannel.ForAddress(ServerURL,new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel,this);
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

    //�Q�[���J�n����
    public async Task ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }
    //���������ʒm
    public void Ready(JoinedUser user)
    {
        ReadiedUser(user);
    }
}
