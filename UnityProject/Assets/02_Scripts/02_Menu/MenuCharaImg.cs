using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharaImg : MonoBehaviour
{
    GameObject cam;
    GameObject ball;

    [SerializeField] private float carrentPos;
    [SerializeField] private float nowPos;



    private void Start()
    {
        carrentPos = this.transform.position.x;

        cam = GameObject.Find("MenuCamera");
       
        //this.GetComponent<SpriteRenderer>().flipX = true;

    }
    private void FixedUpdate()
    {

       

        carrentPos = this.transform.position.x;


        //�J�����ɑ΂��ĕ��s�ɉ摜���\������鏈��

        // �Ώە��Ǝ������g�̍��W����x�N�g�����Z�o
        Vector3 vector3 = cam.transform.position - this.transform.position;
        // �����㉺�����̉�]�͂��Ȃ�(Base�I�u�W�F�N�g�������痣��Ȃ��悤�ɂ���)�悤�ɂ�������Έȉ��̂悤�ɂ���B
        //vector3.y = 0f;

        // Quaternion(��]�l)���擾
        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // �Z�o������]�l�����̃Q�[���I�u�W�F�N�g��rotation�ɑ��
        this.transform.rotation = quaternion;
    }
}
