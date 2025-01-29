using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;       //AudioManagerを使うときはこのusingを入れる
using DG.Tweening;                   //DOTweenを使うときはこのusingを入れる
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    //音響関係
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //各音量変数
    private float SEVolume;
    private float BGMVolume;

    void Start()
    {
        /*BGMSlider.valume = BGMVolume;
        SESlider.valume = SEVolume;*/

    }
    void Update()
    {

        //スライダーの値を音量に反映
        BGMVolume = BGMSlider.value * 0.01f;
        SEVolume = SESlider.value * 0.01f;

        //BGM全体のボリュームを変更
        BGMManager.Instance.ChangeBaseVolume(BGMVolume);

        //SE全体のボリュームを変更
        SEManager.Instance.ChangeBaseVolume(SEVolume);

    }
}
