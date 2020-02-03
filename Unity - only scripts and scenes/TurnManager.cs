using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

//manages the turns of the game
public class TurnManager : MonoBehaviour
{

    public static bool cantouch = true;//allow user to touch screen
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    static Queue<string> turnKey = new Queue<string>();
    static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();
    static DatabaseReference reference;
    
    //before displaying the character
    void Awake()
    {
        units.Clear();
        turnKey.Clear();
        turnTeam.Clear();
        cantouch = true;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://datacrea25.firebaseio.com/");
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    static void InitTeamTurnQueue(){
        List<TacticsMove> teamList = units[turnKey.Peek()];

        foreach (TacticsMove unit in teamList)
        {
            if (unit.hp > 0)
            {
                turnTeam.Enqueue(unit);
            }
        }
        StartTurn();
    }

    public static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
        else
        {
            Debug.Log("Battle End, Loser: " + turnKey.Peek());
            if (turnKey.Peek() == "NPC")
            {
                SceneManager.LoadScene("Victory");
            }
            else
            {
                //losr battle, do not add point, move to main screen after defeat screen
                SceneManager.LoadScene("Lose");
            }

        }
    }

    public static void EndTurn()
    {
        TacticsMove unit = turnTeam.Dequeue();
        unit.EndTurn();
        
        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;
        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units[unit.tag] = list;
            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }
        list.Add(unit);
    }


    public static void RemoveUnit(TacticsMove unit)
    {
        List<TacticsMove> list;
        if (units.ContainsKey(unit.tag))
        {
            list = units[unit.tag];
            if (list.Contains(unit))
            {
                list.Remove(unit);
                unit.GetCurrentTile();
                unit.currentTile.occupied = false;
                unit.currentTile.occupier = null;
                unit.transform.position = new Vector3(90, 90, 90);
            }

        }
        
    }
}
