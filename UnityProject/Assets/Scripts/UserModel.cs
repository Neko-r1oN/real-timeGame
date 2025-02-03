////////////////////////////////////////////////////////////////////////////
///
///  ���[�U�[API�X�N���v�g
///  Author : ������C  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

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
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class UserModel : BaseModel
{
    public int userId { get; set; }       //�o�^���[�U�[ID
    public string userName { get; set; }   //�o�^���[�U�[��

    public string authToken { get; set; }   //�o�^���[�U�[�g�[�N��

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
            User user = await client.RegistUserAsync(name);
            //�t�@�C���Ƀ��[�U�[��ۑ�
            this.userName = name;
            this.userId = user.Id;
            this.authToken = user.Token;
            SaveUserData();
            return true;
        }
        catch (Exception e)
        {//�o�^���s
            Debug.Log("�T�[�o�[�ڑ��G���[�����ɓo�^����Ă閼�O�ł�");
            Debug.Log(e);
            return false;
        }
    }

    //�f�[�^�ۑ�����
    private void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.Name = this.userName;
        saveData.UserID = this.userId;
        saveData.AuthToken = this.authToken;
        string json = JsonConvert.SerializeObject(saveData);

        //�t�@�C����Json��ۑ�
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");   // Application.persistentDataPath�͕ۑ��t�@�C����u���ꏊ

        //Json�`���ŏ�������
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    //���[�U�[���ǂݍ��ݏ���
    public bool LoadUserData()
    {
        //���[�J���ɑ��݂��Ȃ��ꍇ
        if (!File.Exists(Application.persistentDataPath + "/saveData.json")) return false;
        
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

        //���[�J���t�@�C������e��l���擾
        this.userId = saveData.UserID;
        this.userName = saveData.Name;
        this.authToken = saveData.AuthToken;

        //�ǂݍ��ݔ���
        return true;
    }

    /// <summary>
    /// ���[�U�[�擾����
    /// </summary>
    /// <param name="userId">�擾���������[�U�[ID</param>
    /// <returns>�o�^���� or ���s</returns>
    public async UniTask<bool> GetUserInfoAsync(int userId)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {//�o�^����
            Debug.Log("�T�[�o�[�ڑ�����");
            User user = await client.GetUserInfoAsync(userId);

            Debug.Log("�擾����");
            //�t�@�C���Ƀ��[�U�[��ۑ�
            this.userName = user.Name;
            this.userId = user.Id;
            this.authToken = user.Token;

            //SaveUserData();

            return true;
        }
        catch (Exception e)
        {//�o�^���s
            Debug.Log("�擾���s");
            Debug.Log("�T�[�o�[�ڑ��G���[�����݂��Ȃ�ID�ł�");
            Debug.Log(e);
            return false;
        }
    }
}
