using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberImg : MonoBehaviour
{
    GameObject cam;

    public bool isY;
    private void Start()
    {
       
        cam = GameObject.Find("MainCamera");
       
    }
    private void FixedUpdate()
    {
       
        //�J�����ɑ΂��ĕ��s�ɉ摜���\������鏈��

        // �Ώە��Ǝ������g�̍��W����x�N�g�����Z�o
        Vector3 vector3 = cam.transform.position - this.transform.position;
        // �����㉺�����̉�]�͂��Ȃ�(Base�I�u�W�F�N�g�������痣��Ȃ��悤�ɂ���)�悤�ɂ�������Έȉ��̂悤�ɂ���B
        if(isY)vector3.y = 0f;

        // Quaternion(��]�l)���擾
        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // �Z�o������]�l�����̃Q�[���I�u�W�F�N�g��rotation�ɑ��
        this.transform.rotation = quaternion;
    }
}
