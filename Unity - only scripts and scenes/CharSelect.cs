using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{
    List<Hero> chars = new List<Hero>();
    int currentindex=0;
    void Awake()
    {
        chars.Add(new Hero(10, 3, 3, "Hero"));
        chars.Add(new Hero(8, 7, 5, "Falcon"));
        chars.Add(new Hero(20, 3, 3, "Lion"));
        loadchar(new Hero(10, 3, 3, "Hero"));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void next()
    {
        if ((currentindex + 1) >= chars.Count)
        {
            currentindex = 0;
            loadchar(chars[currentindex]);
        } else
        {
            currentindex++;
            loadchar(chars[currentindex]);
        }
    }

    public void prev()
    {
        if ((currentindex - 1) < 0)
        {
            currentindex = chars.Count-1;
            loadchar(chars[currentindex]);
        }
        else
        {
            currentindex--;
            loadchar(chars[currentindex]);
        }
    }

    public void battle()
    {
        PlayerPrefs.SetInt("sAtt", chars[currentindex].attack);
        PlayerPrefs.SetString("sName", chars[currentindex].name);
        PlayerPrefs.SetInt("sHp", chars[currentindex].hp);
        PlayerPrefs.SetInt("sWalk", chars[currentindex].walk);
        SceneManager.LoadScene("Fight");
    }

    private void loadchar(Hero hero)
    {
        GameObject.Find("Char").GetComponent<Image>().sprite = Resources.Load<Sprite>(hero.name.ToLower() + "/down/1");
        GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = hero.name;
        GameObject.Find("HP").GetComponent<TextMeshProUGUI>().text = "HP: "+hero.hp;
        GameObject.Find("Attack").GetComponent<TextMeshProUGUI>().text = "Attack: "+hero.attack;
        GameObject.Find("Walk").GetComponent<TextMeshProUGUI>().text = "Walk: "+hero.walk;
    }

}
