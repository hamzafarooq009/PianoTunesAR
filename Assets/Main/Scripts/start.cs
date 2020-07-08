using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{

    public void ssgame(){
        SceneManager.LoadScene("Main/Scenes/Main");
    }

    public void goBack(){
        SceneManager.LoadScene("Main/Scenes/LandingUI");
    }

    public void openLink(string url){
        Application.OpenURL(url);
    }
}
