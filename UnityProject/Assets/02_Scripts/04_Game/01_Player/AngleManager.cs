using UnityEngine;

public class AngleManager : MonoBehaviour
{
    public bool isLeft;     //�摜�̌���

    private SpriteRenderer img = null;

    GameObject cam;

  
    
    private void Start()
    {
        cam = GameObject.Find("MainCamera");


        isLeft = false;
    }
    private void FixedUpdate()
    {
        //�J�����ɑ΂��ĕ��s�ɉ摜���\������鏈��

        // �Ώە��Ǝ������g�̍��W����x�N�g�����Z�o
        Vector3 vector3 = cam.transform.position - this.transform.position;

        // �����㉺�����̉�]�͂��Ȃ�(Base�I�u�W�F�N�g�������痣��Ȃ��悤�ɂ���)�悤�ɂ�������Έȉ��̂悤�ɂ���B
        vector3.y = 0f;

        //Quaternion(��]�l)���擾

        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // �Z�o������]�l�����̃Q�[���I�u�W�F�N�g��rotation�ɑ��
        this.transform.rotation = quaternion;
    }

    void Update()
    {
        if (isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = false;

        }
        if (!isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

    }
    public bool GetAngle()
    {
        return isLeft;
    }

    //�G�l�~�[�p
    public void SetAngle(bool isLeft)
    {
        this.isLeft = isLeft;
    }
}