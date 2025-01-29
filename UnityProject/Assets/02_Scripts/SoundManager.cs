using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;       //AudioManager���g���Ƃ��͂���using������
using DG.Tweening;                   //DOTween���g���Ƃ��͂���using������
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    //�����֌W
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //�e���ʕϐ�
    private float SEVolume;
    private float BGMVolume;

    void Start()
    {
        /*BGMSlider.valume = BGMVolume;
        SESlider.valume = SEVolume;*/

    }
    void Update()
    {

        //�X���C�_�[�̒l�����ʂɔ��f
        BGMVolume = BGMSlider.value * 0.01f;
        SEVolume = SESlider.value * 0.01f;

        //BGM�S�̂̃{�����[����ύX
        BGMManager.Instance.ChangeBaseVolume(BGMVolume);

        //SE�S�̂̃{�����[����ύX
        SEManager.Instance.ChangeBaseVolume(SEVolume);

    }
}
