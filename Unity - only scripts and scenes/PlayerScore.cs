using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//score to send to firebase, implements comparable in order to allow sorting for leaderboard
public class PlayerScore : IComparable<PlayerScore>
{
    public string id;
    public int victory_points=0;

    public PlayerScore(string username, int vp)
    {
        this.victory_points = vp;
        this.id = username;
    }

    public int CompareTo(PlayerScore other)
    {
        if (this.victory_points > other.victory_points) return -1;
        else if (this.victory_points < other.victory_points) return 1;
        else return 0;
    }


    

}
