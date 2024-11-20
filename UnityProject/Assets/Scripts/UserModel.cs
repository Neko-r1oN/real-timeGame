//==============================================================
//
//               ���[�U�[API(�N���C�A���g�T�C�h)
//
//
//==============================================================


using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserModel : BaseModel
{
    private int userId;       //�o�^���[�U�[ID
    private string userName;  //�o�^���[�U�[��

    public async UniTask<bool> RegistUserAsync(string name)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {//�o�^����
            Debug.Log("�T�[�o�[�ڑ�����");
            userId = await client.RegistUserAsync(name);
            return true;
        }
        catch (Exception e)
        {//�o�^���s
            Debug.Log("�T�[�o�[�ڑ��G���[");
            Debug.Log(e);
            return false;
        }
    }
}
