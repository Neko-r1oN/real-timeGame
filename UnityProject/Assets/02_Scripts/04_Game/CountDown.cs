using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using UnityEngine.InputSystem.XR;

public class CountDown : MonoBehaviour
{
    [SerializeField] List<GameObject> imgNumberList;
    [SerializeField] List<GameObject> imgTextList;
    public float animTime;
    public float initScale;

    public bool isAnimEnd { get; private set; }

    [SerializeField] GameObject controller;

    private int changeCamNum;

    public float spanNum;

    public CinemachineVirtualCameraBase mainCam;
    public CinemachineVirtualCameraBase pCam1;
    public CinemachineVirtualCameraBase pCam2;
    public CinemachineVirtualCameraBase pCam3;
    public CinemachineVirtualCameraBase pCam4;

   
   



    


    // Start is called before the first frame update
    void Start()
    {
        isAnimEnd = false;
        InitUI();

        controller.SetActive(false);

        changeCamNum = 0;
       
        //カメラ優先度設定
        pCam1.Priority = 1;
        pCam2.Priority = 0;
        pCam3.Priority = 0;
        pCam4.Priority = 0;
        mainCam.Priority = 0;

        Invoke("CoutDownAnim", 0.7f);

        ChangeCamera();
        Invoke("ChangeCamera", spanNum * 1.0f);
        Invoke("ChangeCamera", spanNum * 2.0f);
        Invoke("ChangeCamera", spanNum * 3.0f);
        Invoke("ChangeCamera", spanNum * 4.0f);
        

    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void InitUI()
    {
        foreach (var img in imgNumberList)
        {
            img.transform.Rotate(new Vector3(0f, 0f, 90f));
            img.transform.localScale = new Vector3(initScale, initScale, 1f);
            img.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        foreach (var img in imgTextList)
        {
            img.transform.localScale = new Vector3(initScale * 2, initScale * 2, 1f);
            img.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    /// <summary>
    /// アニメーション開始
    /// </summary>
    public void CoutDownAnim()
    {
        var _tween = DOTween.Sequence();

        // ナンバーのTween
        for (int i = 0; i < imgNumberList.Count; i++)
        {
            var sequence = DOTween.Sequence();
            GameObject hideObj = new GameObject();
            hideObj = imgNumberList[i];

            sequence.Append(imgNumberList[i].transform.DORotate(Vector3.zero, animTime * 2))
                .Join(imgNumberList[i].transform.DOScale(new Vector3(1f, 1f, 1f), animTime * 2))
                .Join(imgNumberList[i].GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), animTime * 2))
                .OnComplete(() => { hideObj.SetActive(false); });

            // sequenceを繋げる
            _tween.Append(sequence);
        }

        // テキストのTween
        for (int i = 0; i < imgTextList.Count; i++)
        {
            var sequence = DOTween.Sequence();
            int curentIndex = new int();
            curentIndex = i;

            

            sequence.Append(imgTextList[i].transform.DOScale(new Vector3(1f, 1f, 1f), animTime * 2).SetEase(Ease.InOutBack))
                .Join(imgTextList[i].GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), animTime * 2).SetEase(Ease.InOutBack))
                .SetDelay(animTime / 1.5f);

            if (i == imgTextList.Count - 1)
            {
                
                sequence.OnComplete(() =>
                  {
                     
                      Invoke("HideImgText", animTime);
                     
                  });
            }

            // sequenceを繋げる
            _tween.Join(sequence);
        }

        _tween.OnComplete(() => { isAnimEnd = true; });

        
        _tween.Play();
    }

    void HideImgText()
    {
        

        foreach (var img in imgTextList)
        {
            img.SetActive(false);
        }
    }


    public void ChangeCamera()
    {// カメラの表示・非表示を切り替える

        Debug.Log(changeCamNum);
        switch (changeCamNum)
        {
            
            case 0:
                {
                    pCam1.Priority = 1;
                    pCam2.Priority = 0;
                    pCam3.Priority = 0;
                    pCam4.Priority = 0;
                    mainCam.Priority = 0;
                    Debug.Log("1");

                    changeCamNum++;
                    break;
                }
            case 1:
                {
                    pCam1.Priority = 0;
                    pCam2.Priority = 1;
                    pCam3.Priority = 0;
                    pCam4.Priority = 0;
                    mainCam.Priority = 0;
                    Debug.Log("2");

                    changeCamNum++;
                    break;
                }
            case 2:
                {
                    pCam1.Priority = 0;
                    pCam2.Priority = 0;
                    pCam3.Priority = 1;
                    pCam4.Priority = 0;
                    mainCam.Priority = 0;

                    Debug.Log("3");

                    changeCamNum++;
                    break;
                }
            case 3:
                {
                    pCam1.Priority = 0;
                    pCam2.Priority = 0;
                    pCam3.Priority = 0;
                    pCam4.Priority = 1;
                    mainCam.Priority = 0;

                    Debug.Log("4");

                    changeCamNum++;
                    break;
                }
            case 4:
                {
                    pCam1.Priority = 0;
                    pCam2.Priority = 0;
                    pCam3.Priority = 0;
                    pCam4.Priority = 0;
                    mainCam.Priority = 1;

                    Debug.Log("5");

                    changeCamNum++;

                    controller.SetActive(true);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
