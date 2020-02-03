using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

//splash screen button to decide if player is new or not
public class splashButton : MonoBehaviour
{
    DatabaseReference reference;
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public void ToMainMenu() {

        if (PlayerPrefs.HasKey("player_name"))
        {
            string username = PlayerPrefs.GetString("player_name");
            PlayerScore ps = new PlayerScore(username, PlayerPrefs.GetInt("VictoryPoints", 0));
            string json = JsonUtility.ToJson(ps);
            reference.Child("users").Child(username).SetRawJsonValueAsync(json);//write data to database
            SceneManager.LoadScene("MainMenu");
        }
        else {

            SceneManager.LoadScene("NewPlayer");

        }
        
        

    }

    




}
