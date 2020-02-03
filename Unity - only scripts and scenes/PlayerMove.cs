using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player character definition and actions
public class PlayerMove : TacticsMove
{


    // Start is called before the first frame update
    void Start()
    {
        //name = "hero";
        if (name == PlayerPrefs.GetString("sName"))
        {
            attack = PlayerPrefs.GetInt("sAtt");
            mhp = PlayerPrefs.GetInt("sHp");
            hp = mhp;
            move = PlayerPrefs.GetInt("sWalk");
            this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name.ToLower() + "/down/1");
        }
        else
        {
            hp = 0;
            this.transform.position += new Vector3(100f,0,0);
            //TurnManager.RemoveUnit(this);
        }
        
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!turn)
        {
            return;
        }
        
        if (!attackphase)
        {
            if (!moving)
            {
                FindSelectableTiles(move);
                CheckTouch();
            }
            else
            {
                Move();
            }
        }
        else
        {

            FindSelectableTiles(attackrange);
            CheckTouch();
        }
        
    }
    void CheckTouch()
    {
        if (TurnManager.cantouch)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Tile")
                    {
                        Tile t = hit.collider.GetComponent<Tile>();
                        if (!attackphase && t.selectable)
                        {
                            MoveToTile(t);
                        }
                        else if (attackphase && t.occupied)
                        {
                            if (t.occupier == this)
                            {
                                attackphase = false;
                                TurnManager.EndTurn();
                            }
                            else if ((t.occupier != null && t.occupier != this))
                            {
                                Attack(t);
                                attackphase = false;
                                TurnManager.EndTurn();
                            }

                        }

                    }
                }
            }
        }
        
    }

    
}
