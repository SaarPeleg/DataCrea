using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using TMPro;

public class FinButton : MonoBehaviour
{

    DatabaseReference reference;
    List<string> pl = new List<string>();
    Dictionary<string, PlayerScore> players = null;
    TextMeshProUGUI err;
    string er = "";
    string savename = "";
    bool done = false;
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        err = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();



    }

    void Update()
    {
        err.SetText(er);
        if (done)
        {
            PlayerPrefs.SetString("player_name", savename);
            PlayerPrefs.SetInt("VictoryPoints", 0);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void FirstTimePlay()
    {

        InputField txt_Input = GameObject.Find("InputField").GetComponent<InputField>();
        string tmpName = txt_Input.text;
        if (tmpName == ""||tmpName ==null)
        {
            er="Name cannot be null";
            //Debug.Log("null name");//WIP error here
        }
        else //name is not null
        {


            FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    
                    er="Could not connect to server";
                    
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    foreach (DataSnapshot s in snapshot.Children)
                    {
                        pl.Add(s.Key);
                    }
                    if (snapshot == null || !pl.Contains(tmpName))//list is empty
                    {
                        er = "";
                        PlayerScore ps = new PlayerScore(tmpName, 0);
                        string json = JsonUtility.ToJson(ps);
                        reference.Child("users").Child(tmpName).SetRawJsonValueAsync(json);//write new user to database
                        savename=tmpName;
                        done = true;
                    }
                    else
                    {
                        er="UserName already exists";
                        //Debug.Log("UserName " + tmpName + " already exists");
                    }
                }
            });
            
            


        }

    }





}
