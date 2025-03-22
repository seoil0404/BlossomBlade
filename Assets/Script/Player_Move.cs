using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Move : MonoBehaviour
{
    public Rigidbody player_rg;
    public Transform player_tr;
    public Animator player_an;
    public GameObject trail;
    public TrailRenderer trailren;
    public GameObject gamemanager;
    public GameObject[] attack_button = new GameObject[6];
    public GameObject[] dash_button = new GameObject[2];
    public GameObject FA1_effect; //particle
    public int player_state; // 0 = dash, n = attack_level
    public int move_speed;
    public float attack_move_speed;
    private int attack_adress;
    private int value; //이동방향 (velocity)
    private int attack_value; //공격시 이동방향
    private bool first_run; //회전 명령어 최적화
    private bool fb;
    int dash_adress;
    Quaternion to_rotation; //회전방향
    public Quaternion attack_to_rotation; //공격시 회전 방향
    Quaternion dash_rotation; //돌진시 회전 방향
    public int attack_level;
    private bool attacking;
    private bool dashing;
    string anname;
    bool nextdash;
    private readonly int[] rot = new int[2]; //앞으로 뛰는지 뒤로 뛰는지 구분
    Dictionary<string, float> attack_Delay;
    Dictionary<string, float> move_attack_Delay;
    Dictionary<string, float> move_attack_power;
    Dictionary<string, float> move_attack_after_delay;

    private void Awake()
    {
        //first image change ----------------------------------------
        int i;
        for (i = 0; i < 6; i++)
        {
            attack_button[i].GetComponent<Image>().sprite = attack_button[i].GetComponent<ButtonImage>().image[0];
        }
        //first image change ----------------------------------------
        trail.SetActive(false);
        //dictionay add ---------------------------------------------
        attack_Delay = new Dictionary<string, float>()
        {
            {"NA1", 0.5f},
            {"NA2", 0.5f },
            {"NA3", 0.6f },
            {"DASH", 0.7f},
            {"MA1", 0.75f},
            {"MA2", 1f },
            {"MA3", 1f},
            {"FA1", 1.4f},
            {"LDASH", 0.4f},
        };

        move_attack_Delay = new Dictionary<string, float>()
        {
            {"NA2", 0.1f},
            {"NA3", 0.1f},
            {"MA1", 0.3f},
            {"MA2", 0.3f },
            {"MA3", 0.55f},
            {"FA1", 0.95f},
        };

        move_attack_power = new Dictionary<string, float>()
        {
            {"NA2", -0.1f},
            {"NA3", -0.1f},
            {"MA1", 4f},
            {"MA2", 13f },
            {"MA3", 4f},
            {"FA1", 40f},
        };

        move_attack_after_delay = new Dictionary<string, float>()
        {
            {"MA3", 0.2f},
            {"FA1", 0.1f},
        };
        //dictionay add ---------------------------------------------
        first_run = false;
        attack_level = 1;
        move_speed = 3;
        attack_move_speed = 0;
        rot[0] = 0;
        rot[1] = 0;
    }

    public void Buttonmannager(int Button_Name)
    {
        
        if (Button_Name > 3) fb = false;
        else fb = true;
        if (attack_level == 1)
        {
            player_an.SetBool("Player_Adress", true);
            attack_adress++;
            if (Button_Name == 1 || Button_Name == 4) player_an.SetTrigger("Player_NA1");
            else if (Button_Name == 2 || Button_Name == 5) player_an.SetTrigger("Player_NA2");
            else if (Button_Name == 3 || Button_Name == 6) player_an.SetTrigger("Player_NA3");
        }
        else if (attack_level == 2)
        {
            player_an.SetBool("Player_Adress", true);
            attack_adress++;
            if (Button_Name == 2 || Button_Name == 5) player_an.SetTrigger("Player_MA1");
            else if (Button_Name == 3 || Button_Name == 6) player_an.SetTrigger("Player_MA2");
            else if (Button_Name == 1 || Button_Name == 4) player_an.SetTrigger("Player_MA3");
        }
        else if(attack_level == 3)
        {
            
            if (Button_Name == 2 || Button_Name == 5)
            {
                player_an.SetBool("Player_Adress", true);
                attack_adress++;
                player_an.SetTrigger("Player_FA1");
            }
        }
    }

    public void DashButton(bool dfb)
    {
        attack_level = 1;
        //attack level setting-------------
        int i;
        for (i = 0; i < 6; i++)
        {
            attack_button[i].GetComponent<Image>().sprite = attack_button[i].GetComponent<ButtonImage>().image[0];
        }
        gamemanager.GetComponent<Game_Manager>().attack_button[0].GetComponent<Button>().interactable = true;
        gamemanager.GetComponent<Game_Manager>().attack_button[2].GetComponent<Button>().interactable = true;
        gamemanager.GetComponent<Game_Manager>().attack_button[3].GetComponent<Button>().interactable = true;
        gamemanager.GetComponent<Game_Manager>().attack_button[5].GetComponent<Button>().interactable = true;
        //attack level setting-------------
        if (((dash_rotation == Quaternion.Euler(0, 0, 0)) && (dfb == true) || (dash_rotation == Quaternion.Euler(0, 180, 0)) && (dfb == false)) && nextdash == true)
        {
            nextdash = false;
            Dash2(dfb);
        }
        else
        {
            nextdash = true;
            Dash(dfb);
            StartCoroutine(DashButton_Only());
        }
        
        
    }

    IEnumerator DashButton_Only()
    {
        yield return new WaitForSeconds(0.4f);
        nextdash = false;
    }

    public void Dash(bool dfb)
    {
        player_rg.velocity = Vector3.zero;
        attacking = false;
        dashing = true;
        if (dfb == true)
        {
            player_rg.AddForce(new Vector3(0, 0f, -10f * move_speed), ForceMode.Impulse);
            dash_rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            player_rg.AddForce(new Vector3(0, 0f, 10f * move_speed), ForceMode.Impulse);
            dash_rotation = Quaternion.Euler(0, 180, 0);
        }
        player_an.SetBool("Player_Adress", true);
        player_an.SetTrigger("Player_LDash");
        StartCoroutine(DashDelay(attack_Delay["LDASH"], false));
    }

    public void Dash2(bool dfb)
    {
        player_rg.velocity = Vector3.zero;
        attacking = false;
        dashing = true;
        if (dfb == false)
        {
            player_rg.AddForce(new Vector3(0, 0f, 20f * move_speed), ForceMode.Impulse);
            dash_rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            player_rg.AddForce(new Vector3(0, 0f, -20f * move_speed), ForceMode.Impulse);
            dash_rotation = Quaternion.Euler(0, 180, 0);
        }
        player_an.SetBool("Player_Adress", true);
        player_an.SetTrigger("Player_Dash");
        StartCoroutine(DashDelay(attack_Delay["DASH"], true));
    }

    IEnumerator DashDelay(float t, bool dashlevel)
    {
        dash_adress++;
        int da = dash_adress;
        yield return new WaitForSeconds(t);
        if(dashlevel == true && da == dash_adress && attacking == false)
        {
                dashing = false;
                player_an.SetBool("Player_Adress", false);
                player_an.SetTrigger("Player_Dash");
                if (value == -1) to_rotation = Quaternion.Euler(0, 180, 0);
                else if (value == 1) to_rotation = Quaternion.Euler(0, 0, 0);
                else to_rotation = dash_rotation;
        }
        else if(da == dash_adress && attacking == false)
        {
                dashing = false;
                player_an.SetBool("Player_Adress", false);
                player_an.SetTrigger("Player_LDash");
                if (value == -1) to_rotation = Quaternion.Euler(0, 180, 0);
                else if (value == 1) to_rotation = Quaternion.Euler(0, 0, 0);
                else to_rotation = dash_rotation;
        }
    }

    void CancelAttack()
    {
        if(dashing == true)
        {
            dash_adress++;
            player_rg.velocity = new Vector3(player_rg.velocity.x, player_rg.velocity.y, 0);
            dashing = false;
            if (nextdash == true) player_an.SetTrigger("Player_LDash");
            else player_an.SetTrigger("Player_Dash");
            nextdash = false;
        }
        else if(attacking == true)
        {
            attack_adress++;
            if (value == -1) to_rotation = Quaternion.Euler(0, 180, 0);
            else if (value == 1) to_rotation = Quaternion.Euler(0, 0, 0);
            else to_rotation = attack_to_rotation;
            attack_move_speed = 0;
            attack_value = 0;
            player_an.SetTrigger(anname);
            attacking = false;
            StartCoroutine(Trail_die(0f));
        }
    }

    public void FA1()
    {
        if(attack_level == 3)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(FA1_only());
            StartCoroutine(attack_move_delay(move_attack_Delay["FA1"], move_attack_power["FA1"], move_attack_after_delay["FA1"]));
            StartCoroutine(attackDelay(attack_Delay["FA1"], "Player_FA1"));
        }
    }

    IEnumerator FA1_only()
    {
        
        int atlv = attack_adress;
        int dslv = dash_adress;
        yield return new WaitForSeconds(1f);
        if (atlv == attack_adress && dslv == dash_adress)
        {
            GameObject effect;
            effect = Instantiate(FA1_effect);
            if (attack_to_rotation == Quaternion.Euler(0, 180, 0)) effect.GetComponent<F_A_1>().fb = true;
            else effect.GetComponent<F_A_1>().fb = false;
        }
        
    }

    public void MA1()
    {
        if (attack_level == 2)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attack_move_delay(move_attack_Delay["MA1"], move_attack_power["MA1"]));
            StartCoroutine(attackDelay(attack_Delay["MA1"], "Player_MA1"));
        }
    }

    public void MA2()
    {
        if(attack_level == 2)
        {
            attacking = true;
            attack_level_up();
            if(fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attack_move_delay(move_attack_Delay["MA2"], move_attack_power["MA2"]));
            StartCoroutine(attackDelay(attack_Delay["MA2"], "Player_MA2"));
        }
    }

    public void MA3()
    {
        if (attack_level == 2)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attack_move_delay(move_attack_Delay["MA3"], move_attack_power["MA3"], move_attack_after_delay["MA3"]));
            StartCoroutine(attackDelay(attack_Delay["MA3"], "Player_MA3"));
            StartCoroutine(MA3_Only());
        }
    }
    IEnumerator MA3_Only()
    {
        player_rg.AddForce(new Vector3(0f, 30f, 0f), ForceMode.Impulse);
        yield return new WaitForSeconds(0.55f);
        player_rg.AddForce(new Vector3(0f, -60f, 0f), ForceMode.Impulse);
    }

    

    public void NA1()
    {
        if (attack_level == 1)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attackDelay(attack_Delay["NA1"], "Player_NA1"));
        }
    }

    public void NA2()
    {
        if (attack_level == 1)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attack_move_delay(move_attack_Delay["NA2"], move_attack_power["NA2"]));
            StartCoroutine(attackDelay(attack_Delay["NA2"], "Player_NA2"));
        }
    }

    public void NA3()
    {
        if (attack_level == 1)
        {
            attacking = true;
            attack_level_up();
            if (fb == true)
            {
                attack_to_rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                attack_to_rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(attack_move_delay(move_attack_Delay["NA3"], move_attack_power["NA3"]));
            StartCoroutine(attackDelay(attack_Delay["NA3"], "Player_NA3"));
        }
    }

    private void attack_level_up()
    {
        if (attack_level == 1) trailren.time = 0.1f;
        else if(attack_level ==2) trailren.time = 0.5f;
        else if (attack_level == 3) trailren.time = 0.5f;
        if (attack_level < 3) attack_level++;
        else attack_level = 1;

        if (attack_level == 3)
        {
            int i;
            for (i = 0; i < 6; i++)
            {
                attack_button[i].GetComponent<Image>().sprite = attack_button[i].GetComponent<ButtonImage>().image[2];
            }
            gamemanager.GetComponent<Game_Manager>().attack_button[0].GetComponent<Button>().interactable = false;
            gamemanager.GetComponent<Game_Manager>().attack_button[2].GetComponent<Button>().interactable = false;
            gamemanager.GetComponent<Game_Manager>().attack_button[3].GetComponent<Button>().interactable = false;
            gamemanager.GetComponent<Game_Manager>().attack_button[5].GetComponent<Button>().interactable = false;
        }
        else if(attack_level == 2)
        {
            int i;
            for (i = 0; i < 6; i++)
            {
                attack_button[i].GetComponent<Image>().sprite = attack_button[i].GetComponent<ButtonImage>().image[1];
            }
            gamemanager.GetComponent<Game_Manager>().attack_button[0].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[2].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[3].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[5].GetComponent<Button>().interactable = true;
        }
        else
        {
            int i;
            for(i = 0; i < 6; i++)
            {
                attack_button[i].GetComponent<Image>().sprite = attack_button[i].GetComponent<ButtonImage>().image[0];
            }
            gamemanager.GetComponent<Game_Manager>().attack_button[0].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[2].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[3].GetComponent<Button>().interactable = true;
            gamemanager.GetComponent<Game_Manager>().attack_button[5].GetComponent<Button>().interactable = true;
        }
    }

    IEnumerator attack_move_delay(float t, float speed, float ta = 0)
    {
        attack_move_speed = 0;
        int atvl = attack_adress;
        int dsvl = dash_adress;
        yield return new WaitForSeconds(t);
        if(atvl == attack_adress && dsvl == dash_adress)
        {
            if (ta != 0)
            {
                StartCoroutine(attack_after_move_delay(ta));
            }
            if (attack_to_rotation == Quaternion.Euler(0, 0, 0)) attack_value = 1;
            else if (attack_to_rotation == Quaternion.Euler(0, 180, 0)) attack_value = -1;
            attack_move_speed = speed;
        }
    }

    IEnumerator attack_after_move_delay(float t) //can call only [attack_move_delay]
    {
        yield return new WaitForSeconds(t);
        attack_move_speed = 0;
    }

    IEnumerator attackDelay(float t, string an_name)
    {
        anname = an_name;
        trail.SetActive(true);
        int atlv = attack_adress;
        int dslv = dash_adress;
        if (dashing == true)
        {
            player_rg.velocity = new Vector3(player_rg.velocity.x, player_rg.velocity.y, 0);
            dashing = false;
        }
        trail.SetActive(true);
        yield return new WaitForSeconds(t);
        if(atlv == attack_adress && dslv == dash_adress)
        {
            if (value == -1) to_rotation = Quaternion.Euler(0, 180, 0);
            else if (value == 1) to_rotation = Quaternion.Euler(0, 0, 0);
            else to_rotation = attack_to_rotation;
            attack_move_speed = 0;
            attack_value = 0;
            player_an.SetBool("Player_Adress", false);
            player_an.SetTrigger(an_name);
            attacking = false;
            StartCoroutine(Trail_die(0f));
        }
    }

    IEnumerator Trail_die(float t)
    {
        int atlv = attack_adress;
        yield return new WaitForSeconds(t);
        while(trailren.time > 0 && atlv == attack_adress)
        {
            trailren.time -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        if (atlv == attack_adress) if (trailren.time < 0) trail.SetActive(false);
    }
    public void run_value(int value_) //right(front) = 1,-1 || left(back) = 2, -2
    {
        switch(value_)
        {
            case -1:
                rot[0] = 0;
                break;
            case 1:
                rot[0] = 1;
                break;
            case 2:
                rot[1] = 1;
                break;
            case -2:
                rot[1] = 0;
                break;

        }
        if (rot[0] == 1 && rot[1] == 0)
        {
            value = -1;
            player_an.SetBool("Player_Run", true);
            if (first_run == false) first_run = true;
            to_rotation = Quaternion.Euler(0, 180, 0);
            CancelAttack();
        }
        else if (rot[1] == 1 && rot[0] == 0)
        {
            value = 1;
            player_an.SetBool("Player_Run", true);
            if (first_run == false) first_run = true;
            to_rotation = Quaternion.Euler(0, 0, 0);
            CancelAttack();
        }
        else
        {
            value = 0;
            player_an.SetBool("Player_Run", false);
        }
        player_an.SetBool("Player_Adress", false);
    }

    private void FixedUpdate()
    {
        if (attacking == false && dashing == false)
        {
            player_rg.velocity = new Vector3(player_rg.velocity.x, player_rg.velocity.y, value * Time.deltaTime * move_speed * 200); // -1 : front 1 : back
            player_tr.transform.rotation = Quaternion.Lerp(player_tr.rotation, to_rotation, 15 * Time.deltaTime);
        }
        else if (dashing == true)
        {
            player_tr.transform.rotation = Quaternion.Lerp(player_tr.rotation, dash_rotation, 15 * Time.deltaTime);
        }
        else
        {
            player_rg.velocity = new Vector3(player_rg.velocity.x, player_rg.velocity.y, attack_value * Time.deltaTime * attack_move_speed * 200);
            player_tr.transform.rotation = Quaternion.Lerp(player_tr.rotation, attack_to_rotation, 15 * Time.deltaTime);
        }
    }
}
