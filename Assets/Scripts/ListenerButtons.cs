using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс метод нажатия на Button
/// </summary>
public class ListenerButtons : MonoBehaviour
{
    public void BtnStartGame_onClcik() 
    {
        SceneManager.LoadScene("Game");
    }

    public void BtnExit_onClick()
    {
        Application.Quit(); 
    }


    public void BtnMenu_onClick()
    {
       SceneManager.LoadScene("Menu");
    }
}
