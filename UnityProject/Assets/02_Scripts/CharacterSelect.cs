using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public int MaxCharacter; //最大キャラ人数
    public int SelectCharacter;// キャラセレの割り振り番号
    public Text _1PCharacterName;

    // キャラの表示絵関係
    public GameObject _1PCharacter;
    public Image _1PCharacterImage;
    public Sprite _1PCharacterSprite;

    // カーソルの設定
    public GameObject[] cursor;

    // カーソルの設定
    public Sprite[] Box;


    void Start()
    {
        SelectCharacter = 1;
       

        _1PCharacter.gameObject.SetActive(true);// キャラ絵をアクティブ化
        cursor[SelectCharacter].gameObject.SetActive(true);// カーソルをアクティブ化

       
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursor[SelectCharacter].gameObject.SetActive(false);


            if (SelectCharacter >= MaxCharacter)
            {
                SelectCharacter = 1;
            }
            else
            {
                ++SelectCharacter;
            }

            cursor[SelectCharacter].gameObject.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
           
             cursor[SelectCharacter].gameObject.SetActive(false);
    

            if (SelectCharacter <= 1)
            {
                SelectCharacter = MaxCharacter;
            }
            else
            {
                 --SelectCharacter;
            }

            cursor[SelectCharacter].gameObject.SetActive(true);
            
        }
    }
  
    // キャラクターの決定
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
           

            SceneManager.LoadScene("BattleScene");// 対戦シーンへ移動
        }
    }

    void FixedUpdate()
    {
        switch (SelectCharacter)
        {
            case 1:
                _1PCharacterName.text = "SHINO";


                
                break;

            case 2:
                _1PCharacterName.text = "HIYORI";

                
               
                break;

            case 3:
                _1PCharacterName.text = "HIYAME";
                break;

            case 4:
                _1PCharacterName.text = "KINAKO";

              
               
                break;

            case 5:
                _1PCharacterName.text = "NAYA";

               
               
                break;

            case 6:
                _1PCharacterName.text = "RIR";

               
                break;
        }

        _1PCharacterImage.sprite = Box[SelectCharacter];
    }
}