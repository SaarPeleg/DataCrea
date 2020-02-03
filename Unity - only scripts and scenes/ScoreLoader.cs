using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

//load score to leaderboard
public class ScoreLoader : MonoBehaviour
{
    bool updated = true;
    DatabaseReference reference;
    List<PlayerScore> pl = new List<PlayerScore>();
    //Text name;
    bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        //name = GameObject.Find("Name").GetComponent<Text>();

        FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync().ContinueWith(task =>
        {
            try
            {
                if (task.IsFaulted)
                {
                    
                    //er = "Could not connect to server";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    foreach (DataSnapshot s in snapshot.Children)
                    {
                        string o = s.GetRawJsonValue();
                        pl.Add(JsonUtility.FromJson<PlayerScore>(o));
                    }
                    pl.Sort();
                    updated = false;


                }
            }
            catch (Exception e)
            {
            }
            
        });



    }

    // Update is called once per frame
    void Update()
    {
        if (!updated)
        {
            for (int i = 0; i < 10; i++)
            {
                Text name = GameObject.Find("Name (" + (i + 1) + ")").GetComponent<Text>();
                name.text = pl[i].id;
                Text vp = GameObject.Find("Score (" + (i + 1) + ")").GetComponent<Text>();
                vp.text = pl[i].victory_points.ToString();
            }
            
            foreach (PlayerScore ps in pl)
            {
                if (ps.id == PlayerPrefs.GetString("player_name"))
                {
                    Text name = GameObject.Find("Name (11)").GetComponent<Text>();
                    name.text = ps.id;
                    Text vp = GameObject.Find("Score (11)").GetComponent<Text>();
                    vp.text = ps.victory_points.ToString();

                    Text nm = GameObject.Find("Number (11)").GetComponent<Text>();
                    nm.text = (pl.IndexOf(ps)+1).ToString();
                }
            }
            updated = true;
            
        }
        
    }
}
