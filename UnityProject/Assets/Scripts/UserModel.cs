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
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserModel : BaseModel
{
    public int userId { get; set; }       //�o�^���[�U�[ID
    public string userName { get; set; }   //�o�^���[�U�[��

    private static UserModel instance;

    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObj = new GameObject("UserModel");
                //GameObject�����ANetworkManager�R���|�[�l���g��ǉ�
                instance = gameObj.AddComponent<UserModel>();
                //�V�[���ړ����ɍ폜���Ȃ��悤�ɂ���
                DontDestroyOnLoad(gameObj);

            }
            return instance;
        }
    }


    //���[�U�[�ړ��ʒm
    public Action<MoveData> MovedUser { get; set; }

    /// <summary>
    /// ���[�U�[�o�^����
    /// </summary>
    /// <param name="name">���[�U�[��</param>
    /// <returns>�o�^���� or ���s</returns>
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
            Debug.Log("�T�[�o�[�ڑ��G���[�����ɓo�^����Ă閼�O�ł�");
            Debug.Log(e);
            return false;
        }
    }
}
