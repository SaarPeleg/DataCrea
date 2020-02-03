using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//character code
public class Hero
{
    public string name="Hero";
    public int attack = 3;
    public int hp = 10;
    public int walk = 3;

    public Hero(int hp1, int attack1, int walk1, string name1)
    {
        this.hp = hp1;
        this.attack = attack1;
        this.walk = walk1;
        this.name = name1;
    }
}
