using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{

    //�C���X�y�N�^�[����ݒ肷�邩�A����������GetComponent���āAImage�ւ̎Q�Ƃ��擾���Ă����B
    [SerializeField]
    Text img;

    [Header("1���[�v�̒���(�b�P��)")]
    [SerializeField]
    [Range(0.1f, 5.0f)]
    float duration = 1.0f;


    [Header("���[�v�J�n���̐F")]
    [SerializeField]
    Color32 startColor = new Color32(255, 255, 255, 255);
    //���[�v�I��(�܂�Ԃ�)���̐F��0�`255�܂ł̐����Ŏw��B
    [Header("���[�v�I�����̐F")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 0);


    //�C���X�y�N�^�[����ݒ肵���ꍇ�́AGetComponent����K�v���Ȃ��Ȃ�ׁAAwake���폜���Ă��ǂ��B
    void Awake()
    {
        if (img == null)
            img = GetComponent<Text>();
    }

    void Update()
    {
        //Color.Lerp�ɊJ�n�̐F�A�I���̐F�A0�`1�܂ł�float��n���ƒ��Ԃ̐F���Ԃ����B
        //Mathf.PingPong�Ɍo�ߎ��Ԃ�n���ƁA0�`1�܂ł̒l���Ԃ����B
        img.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time / duration, 1.0f));
    }
}
