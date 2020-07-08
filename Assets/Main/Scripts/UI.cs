using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI  : MonoBehaviour
{
    public string theName;
    public GameObject inputField;
    public GameObject textDisplay;

    public void StoreName(){
        theName = inputField.GetComponent<Text>().text;
        PlayerPrefs.SetString("youtubeUrl",theName);
        PlayerPrefs.Save();
        // string player = PlayerPrefs.GetString("youtubeUrl");
        textDisplay.GetComponent<Text>().text = "Playing " + theName;
    }

}

