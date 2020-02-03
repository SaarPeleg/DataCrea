using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//general character definition and actions
public class TacticsMove : MonoBehaviour
{
    
    public bool turn = false;//is it the unit's turn
    List<Tile> selectableTiles = new List<Tile>();//list of slectable tiles
    GameObject[] tiles;//list of all tiles
    Stack<Tile> path = new Stack<Tile>();//path to a tile;
    public Tile currentTile;//tile you are on

    public Sprite up, down, left, right;//sprite states, to change direction
    public string name;
    public int move = 3;//move distance
    public float movespeed = 2f;//move speed
    public bool moving = false;//is the unit currently moving

    Vector3 velocity = new Vector3();//velocity of unit
    Vector3 heading = new Vector3();//direction vector

    //for A*
    public Tile actualTargetTile;

    //attack phase
    public bool attackphase = false;
    public int attackrange = 1;

    //stats
    public int attack = 1;
    public int hp = 10;
    public int mhp = 10;

    GameObject hpbar;
    //initializetion
    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        hpbar = GameObject.Find(gameObject.name + "HpBar");
        TurnManager.AddUnit(this);
    }

    //assign current tile to be the tile the player is on
    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    //return the target tile
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile=null;

        if (Physics.Raycast(target.transform.position, new Vector3(0, 0, 1), out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    //compute adjacent tiles
    public void ComputeAdjacencyLists(Tile target)
    {

        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(target,attackphase,this.tag);
        }
    }
    //find selectable tiles around user using bfs search
    public void FindSelectableTiles(int range)
    {
        ComputeAdjacencyLists(null);
        GetCurrentTile();
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();
            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < range)
            {
                foreach (Tile ti in t.adjacentcyList)
                {
                    if (!ti.visited)
                    {
                        
                        ti.parent = t;
                        ti.visited = true;
                        ti.distance = t.distance + 1;
                        process.Enqueue(ti);
                    }
                }
            }
        }
    }

    //calculate path to tile
    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next.parent != null)
        {
            path.Push(next);
            next = next.parent;
            
        }
    }

    //movement implementation
    public void Move()
    {
        
        if (path.Count > 0)
        {

            currentTile.occupied = false;
            currentTile.occupier = null;
            Tile t = path.Peek();
            if (path.Count == 1)
            {
                t.occupied = true;
                t.occupier = this;
            }
            Vector3 target = t.transform.position;
            //Debug.Log(transform.position.x + "," + transform.position.y + "," + transform.position.z + ":" + target.x + "," + target.y + "," + target.z);
            if (Vector3.Distance(new Vector3(transform.position.x,transform.position.y,1), target) >= 0.2f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();
                //transform.up = heading;
                transform.position += velocity * Time.deltaTime;
                
            }
            else
            {
                //tile center reached
                transform.position = new Vector3(target.x, target.y, 0);
                path.Pop();
                if (Math.Abs(heading.x) > Math.Abs(heading.y))
                {
                    if (heading.x >= 0)
                    {
                        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name+"/right/1");
                    }
                    else
                    {
                        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name +"/left/1");
                    }
                }
                else
                {
                    if (heading.y >= 0)
                    {
                        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name+"/up/1");
                    }
                    else
                    {
                        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name+"/down/1");
                    }
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                
            }
        }
        else
        {
            GetCurrentTile();
            currentTile.occupier = this;
            currentTile.occupied = true;
            RemoveSelectableTiles();
            moving = false;
            attackphase = true;
            if (this.tag == "Player")
            {
                StartCoroutine(Touchwait());
            }
            //until npc attack is implemented
            if(this.tag=="NPC"){
                FindSelectableTiles(attack);
                foreach(Tile t in selectableTiles){
                    if (t.occupied)
                    {
                        if (t.occupier.tag != this.tag)
                        {
                            Attack(t);
                            break;
                        }
                    }
                    
                }
                
                TurnManager.EndTurn();
                attackphase = false;
            }
            
        }
    }

    //calculate velocity
    void SetHorizontalVelocity()
    {
        velocity = heading * movespeed;
    }

    //move direction calculation and sprite assignment
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading = new Vector3(heading.x, heading.y, 0);//remove any rouge z movement to minimize 
        heading.Normalize();
        
        if (Math.Abs(heading.x) > Math.Abs(heading.y))
        {
            if (heading.x >= 0)
            {
                this.GetComponent<SpriteRenderer>().sprite = right;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = left;
            }
        }
        else
        {
            if (heading.y >= 0)
            {
                this.GetComponent<SpriteRenderer>().sprite = up;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = down;
            }
        }
    }

    //remove all selectable tiles from list for recalculation
    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile t in selectableTiles)
        {
            t.Reset();
        }

        selectableTiles.Clear();
    }

    //NPC Move related, A* algorithm
    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();
        Tile next = t.parent;
        while (next != null)
        {
            
            tempPath.Push(next);
            next = next.parent;
        }
        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();
        }
        return endTile;
    }
    
    protected void FindPath(Tile target)
    {   
        ComputeAdjacencyLists(target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();//every tile that was not processed
        List<Tile> closedList = new List<Tile>();//every tile that was processed

        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList);
            closedList.Add(t);

            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacentcyList)
            {
                if (closedList.Contains(tile))
                {

                    Debug.Log("Path not found1");
                    //in closed list, do nothing
                }
                else if (openList.Contains(tile))
                {//found second way to this tile
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    if (tempG < tile.g)
                    {
                        tile.parent = t;
                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else//on none of the lists
                {
                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.h + tile.g;

                    openList.Add(tile);
                }
            }

        }

        //todo: if no path
        //Debug.Log("Path not found");
    }

    protected Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];
        foreach (Tile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }
        list.Remove(lowest);
        
        return lowest;
    }

    //end of A* methods

    //start unit turn
    public void BeginTurn()
    {
        GetCurrentTile();
        currentTile.occupier = this;
        currentTile.occupied = true;
        turn = true;

    }

    //end unit turn
    public void EndTurn()
    {
        turn = false;
    }

    //wait between touches to prevent early end turn
    IEnumerator Touchwait()
    {
        TurnManager.cantouch = false;
        yield return new WaitForSeconds(0.5f);
        TurnManager.cantouch = true;
    }

    protected void Attack(Tile t)
    {
        if (t != null&&t.occupied)
        {
            t.occupier.hp = t.occupier.hp - this.attack;
            float reduced = (float)this.attack / (float)t.occupier.mhp;
            Vector3 scaleChange = new Vector3(-reduced, 0, 0);
            t.occupier.hpbar.transform.localScale += scaleChange;
            Debug.Log(this.name);
            if (t.occupier.hp <= 0)
            {
                TurnManager.RemoveUnit(t.occupier);
            }

            if (this.tag == "Player")
            {
                StartCoroutine(Touchwait());
            }
        }
    }

}
