using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public int MaxCharacter; //�ő�L�����l��
    public int SelectCharacter;// �L�����Z���̊���U��ԍ�
    public Text _1PCharacterName;

    // �L�����̕\���G�֌W
    public GameObject _1PCharacter;
    public Image _1PCharacterImage;
    public Sprite _1PCharacterSprite;

    // �J�[�\���̐ݒ�
    public GameObject[] cursor;

    // �J�[�\���̐ݒ�
    public Sprite[] Box;


    void Start()
    {
        SelectCharacter = 1;
       

        _1PCharacter.gameObject.SetActive(true);// �L�����G���A�N�e�B�u��
        cursor[SelectCharacter].gameObject.SetActive(true);// �J�[�\�����A�N�e�B�u��

       
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
  
    // �L�����N�^�[�̌���
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
           

            SceneManager.LoadScene("BattleScene");// �ΐ�V�[���ֈړ�
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