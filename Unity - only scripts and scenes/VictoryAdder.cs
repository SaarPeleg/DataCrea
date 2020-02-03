using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

//adds a point to the winner of the battle
public class VictoryAdder : MonoBehaviour
{
    static DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        //win battle, add points, move to main screen after victory screen
        string username = PlayerPrefs.GetString("player_name", "");
        PlayerPrefs.SetInt("VictoryPoints", PlayerPrefs.GetInt("VictoryPoints", 0) + 1);
        PlayerScore ps = new PlayerScore(username, PlayerPrefs.GetInt("VictoryPoints", 0));
        string json = JsonUtility.ToJson(ps);
        reference.Child("users").Child(username).SetRawJsonValueAsync(json);//write data to database
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
