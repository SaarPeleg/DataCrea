using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NPC definition and actions
public class NPCMove : TacticsMove
{
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        name = "shadow";
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindTarget();
            if (target == null)
            {
                TurnManager.EndTurn();
            }
            CalculatePath();
            FindSelectableTiles(move);
        }
        else
        {
            Move();

        }



    }

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }
    //find nearest target enemy;
    void FindTarget()
    {
        List<GameObject> tar1 = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for (int i = 0; i < tar1.Count; i++)
        {
            if (tar1[i] == null || tar1[i].GetComponent<PlayerMove>().hp <= 0)
            {
                tar1.Remove(tar1[i]);
            }
        }
        GameObject[] targets = tar1.ToArray();
        GameObject nearest = null;
        float distance = Mathf.Infinity;
        foreach (GameObject obj in targets)
        {
            float dis = Vector3.Distance(transform.position, obj.transform.position);
            if (dis < distance)
            {
                distance = dis;
                nearest = obj;
            }
        }
        target = nearest;

    }
}
