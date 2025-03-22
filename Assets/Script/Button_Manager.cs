using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject player;
    public Animator player_an;
    public string Button_Name;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //move direction
        if(Button_Name == "left")
        {
            player.GetComponent<Player_Move>().run_value(2);
        }
        else if (Button_Name == "right")
        {
            player.GetComponent<Player_Move>().run_value(1);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //move direction
        if (Button_Name == "left")
        {
            player.GetComponent<Player_Move>().run_value(-2);
        }
        else if (Button_Name == "right")
        {
            player.GetComponent<Player_Move>().run_value(-1);
        }
    }
}
