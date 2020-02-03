using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

//qr reader
public class QRCodeReaderDemo : MonoBehaviour {

    private IReader QRReader;
    public Text resultText;
    public RawImage image;
    string er = "";
    bool updated = true;
    public DatabaseReference reference;
    List<string> pl = new List<string>();
    Text text1;
    string barval;

    void Awake () {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
	}

    // Use this for initialization
    private void Start()
    {
        text1 = GameObject.Find("Text1").GetComponent<Text>();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");
        
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        /*string name123 = "2VP";
        VP vp = new VP(name123);
        string json = JsonUtility.ToJson(vp);
        reference.Child("QR").Child(name123).SetRawJsonValueAsync(json);//write new user to database*/ //this is initalization code for new qr codes
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();

        QRReader.OnReady += StartReadingQR;

        QRReader.StatusChanged += QRReader_StatusChanged;
    }

    private void QRReader_StatusChanged(object sender, System.EventArgs e)
    {
        resultText.text = "Status: " + QRReader.Status;
    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        image.transform.localEulerAngles = QRReader.Camera.GetEulerAngles();
        image.transform.localScale = QRReader.Camera.GetScale();
        image.texture = QRReader.Camera.Texture;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        float height = rectTransform.sizeDelta.x * (QRReader.Camera.Height / QRReader.Camera.Width);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    // Update is called once per frame
    void Update () {

        if (QRReader == null)
        {
            return;
        }
        if (!updated)
        {
            string tes = PlayerPrefs.GetString(barval,"kuku");
            if (tes != "kuku")
            {
                er = barval + ": " + "Code was already used";
            }
            else
            {

                string username = PlayerPrefs.GetString("player_name", "");
                er = "gained 1 Victory point!";
                PlayerPrefs.SetInt("VictoryPoints", PlayerPrefs.GetInt("VictoryPoints", 0) + 1);
                PlayerScore ps = new PlayerScore(username, PlayerPrefs.GetInt("VictoryPoints", 0));
                PlayerPrefs.SetString(barval, barval);
                text1.text = username;
                string json = JsonUtility.ToJson(ps);
                reference.Child("users").Child(username).SetRawJsonValueAsync(json);//write data to database
            }
            updated = true;

        }
        QRReader.Update();
        text1.text = er;

    }

    public void StartScanning()
    {
        if (QRReader == null)
        {
            Debug.LogWarning("No valid camera - Click Start");
            return;
        }

        // Start Scanning
        QRReader.Scan((barCodeType, barCodeValue) => {
            QRReader.Stop();
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + barCodeValue +"</b>";
            FirebaseDatabase.DefaultInstance.GetReference("QR/").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {

                    er = "Could not connect to server";

                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    foreach (DataSnapshot s in snapshot.Children)
                    {
                        er = "contains key: " + s.Key+ "barcode: "+ barCodeValue;
                        if(s.Key== barCodeValue)
                        {
                            barval = barCodeValue;
                            updated = false;
                            er = "match found: " + s.Key;
                            break;
                        }
                    }
                }
            });




            
            

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }
}
