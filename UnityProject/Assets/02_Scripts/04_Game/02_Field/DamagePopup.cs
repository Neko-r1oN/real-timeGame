using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] GameObject targets;             //�Ώۂ̓G
    [SerializeField] GameObject canvas;    //�_���[�WUI�����L�����o�X
    [SerializeField] GameObject damageUI;    //�_���[�WUI

    public float deleteTime = 1.0f;

    void OnCollisionEnter(Collision other)
    {

        
        //�N���A����I�u�W�F�N�g(�f�o�b�O�p)
        if (other.gameObject.tag == "Player")
        {
            PopDamage();
        }


    }
    //�_���[�W�e�L�X�gUI�����֐�
    public void PopDamage(/*GameObject target*/)
    {
        /*targets = target;*/

        //�R�s�[����
        var obj = new GameObject("Target");
        var ui = Instantiate(damageUI);

        //�e�ύX
        obj.transform.SetParent(targets.transform);
        ui.transform.SetParent(canvas.transform);

        ui.SetActive(true);

        var circlePos = Random.insideUnitCircle * 0.6f;
        //UI���^�[�Q�b�g�̉�ʏ�̈ʒu�ɔz�u
        obj.transform.position = transform.position + Vector3.up * Random.Range(3.0f,4.0f) + new Vector3(circlePos.x,circlePos.y);
        ui.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main,obj.transform.position);

        //�w�莞�Ԍ�ɍ폜
        Destroy(obj, deleteTime);
        Destroy(ui, deleteTime);
    }
}
