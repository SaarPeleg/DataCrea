using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a tile in the game board
public class Tile : MonoBehaviour
{
    public bool target = false;
    public bool current = false;
    public bool selectable = false;
    public bool walkable = true;
    public bool occupied = false;
    public TacticsMove occupier = null;

    public List<Tile> adjacentcyList = new List<Tile>();

    //needed variables for BFS algorithm
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;


    //needed variables for A*
    public float f = 0;//g+h
    public float g = 0;//cost from parent to current tile
    public float h = 0;//cost from processed tile to destination

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }

        Color textureColor = GetComponent<Renderer>().material.color;
        textureColor.a = 0.1f;
        GetComponent<Renderer>().material.color = textureColor;
    }

    public void Reset()
    {

        adjacentcyList.Clear();
        target = false;
        current = false;
        selectable = false;


        //needed variables for BFS algorithm
        visited = false;
        parent = null;
        distance = 0;

        //A* variables
        f = g = h = 0;
    }

    public void FindNeighbors(Tile target,bool attphase,string attacktag)
    {
        Reset();
        
        CheckTile(new Vector3(0,1,0), target, attphase, attacktag);
        CheckTile(new Vector3(0, -1, 0), target, attphase, attacktag);
        CheckTile(new Vector3(1, 0, 0), target, attphase, attacktag);
        CheckTile(new Vector3(-1, 0, 0), target, attphase, attacktag);

    }

    void CheckTile(Vector3 direction, Tile target, bool attphase, string attacktag)
    {

        Vector3 currentposition=transform.position;
        Vector3 halfextents=new Vector3(0.25f,0.25f,0);
        Collider[] colliders = Physics.OverlapBox(currentposition + direction, halfextents);
        foreach (Collider collider in colliders)
        {
            Tile tile = collider.GetComponent<Tile>();

            if (attphase)
            {
                if (tile != null && tile.occupied)
                {
                    if (tile.occupier.tag != attacktag)
                    {
                        adjacentcyList.Add(tile);
                    }
                }
            }
            else
            {
                if (tile != null && tile.walkable)
                {
                    //check if tile has nothing on it
                    if ((!tile.occupied) || (tile == target))
                    {
                        adjacentcyList.Add(tile);
                    }
                }
            }
            
        }
    }
}
