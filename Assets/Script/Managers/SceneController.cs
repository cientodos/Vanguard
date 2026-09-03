using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoMainTitle()
    {
        SceneManager.LoadScene("MainTitle");
    }
    public void GoOnlinePlay()
    {
        SceneManager.LoadScene("PlayScene"); 
    }
    public void GoDeck()
    {
        SceneManager.LoadScene("DeckBuilder");
    }
    public void Exit()
    { 
        Application.Quit();
    }
}
