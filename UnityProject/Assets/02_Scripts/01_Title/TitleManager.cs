using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;



public class TitleManager : MonoBehaviour
{

    
    [SerializeField] UserModel userModel;

    [SerializeField] InputField debug;

    [SerializeField] Text nameText;

    [SerializeField] GameObject NameField;
    [SerializeField] GameObject RegistTrue;
    [SerializeField] GameObject RegistFalse;


    
    // Start is called before the first frame update
    void Start()
    {
        RegistTrue.SetActive(false);
        RegistFalse.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public async void OnClickRegist()
    {

        if (debug != null)
        {
            userModel.userId = int.Parse(debug.text);
            Initiate.Fade("GameScene", Color.black, 1.0f);
        }
        else
        {
            bool isRegist = await userModel.RegistUserAsync(nameText.text);

            if (userModel)
            {

                Debug.Log(isRegist);

                if (isRegist == true)
                {
                    RegistFalse.SetActive(false);
                    RegistTrue.SetActive(true);
                    Initiate.Fade("GameScene", Color.black, 1.0f);
                }
                else if (isRegist == false)
                {
                    RegistTrue.SetActive(false);
                    RegistFalse.SetActive(true);
                }
            }
        }
        
    }
}
