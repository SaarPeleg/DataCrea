using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//navigation of scenes
public class tomain : MonoBehaviour
{

    public void ToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        

    }

    public void ToBattle()
    {
        SceneManager.LoadScene("Charselect");
        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    public void ToFight()
    {
        SceneManager.LoadScene("Fight");
        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    public void ToScores()
    {
        SceneManager.LoadScene("Scores");
        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    public void ToQR()
    {
        SceneManager.LoadScene("QR");
        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    




}
