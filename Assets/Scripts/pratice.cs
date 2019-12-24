using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum PacManStates
{
    Spawned,
    Roaming,
    Dead,
    WinState,
    SuperPM

}
enum GhostStates
{
    ghostrespawn,
    Ghostroaming,
    GhostDead,
    Ghostscared

}
enum GameEvents
{
    NewLife,
    AllLivesLost,
    EncByGhost,
    CrumsFinished,
    EatSuperTab,
    TimeOut,
    EatCrum,
    GameStart,
    EncByPacMan,
    Run,
    Hunting
}




public class player_move : MonoBehaviour
{

    public static Stack<int> PwrStack = new Stack<int>();

    public static int crums = 0;
    public static int time = 0;
    public static int SMtime = 0;
    public static int power = 0;
    public static int life;
    public static int lifechk;
    public static string otherObj;
    public static string ghostname;




    public GameObject SM6;
    public GameObject SM7;
    public GameObject SM8;
    public GameObject SM9;
    public GameObject player;

    static PacManStates PacState = new PacManStates();
    static GhostStates GhState = new GhostStates();
    static GameEvents CurrentEvent = new GameEvents();
    static Animator anim;
    // Use this for initialization
    void Start()
    {
        life = 2;

        anim = GetComponent<Animator>();


    }




    // Update is called once per frame
    void Update()
    {


        Ghostrespawn();
        time--;

        SMtime--;
        timeCal();


    }




    private void OnTriggerEnter(Collider collision)
    {

        //tring otherObj = collision.gameObject.name;
        string Obj2 = collision.gameObject.name;

        string collidername = GetComponent<Collider>().name;
        GameObject Obj = collision.gameObject;
        EventCheck(collidername, Obj2, Obj);
        // EatCrums(Obj, collidername, Obj2);

        if (collidername == "claire")
        {

            if (Obj2 == "Pickup")
            {
                Debug.Log("Collider = " + collidername + " Collided with =  " + otherObj);
                // Debug.Log("hell");
                player.transform.localScale = new Vector3(2, 2, 2);
                Destroy(collision.gameObject);

                PwrStack.Push(time);
                time = 1000;
                power = 2;
            }
            if (Obj2 == "Pickup (1)")
            {
                Debug.Log("Collider = " + collidername + " Collided with =  " + otherObj);
                //Debug.Log("hell2");
                player.transform.localScale = new Vector3(2, 2, 2);
                Destroy(collision.gameObject);
                PwrStack.Push(time);
                time = 1000;
                power = 2;
            }
        }

    }


    public void OnCollisionEnter(Collision collision)
    {
        GameObject G_Obj = collision.gameObject;
        otherObj = collision.gameObject.name;
        string col_obj = collision.gameObject.name;
        // SuperMode(otherObj);

        string collidername2 = GetComponent<Collider>().name;
        EventCheck(collidername2, col_obj, G_Obj);
        //  Debug.Log("Collider= " + collidername2 + "Other Obj" + otherObj);


    }

    public void timeCal()
    {
        if (time < 0 && life >= 0)
        {

            player.transform.localScale = new Vector3(1, 1, 1);
            GhostRoaming();
            if (power == 1)
            {
                power = 0;
                CurrentEvent = GameEvents.TimeOut;

            }

            if (power == 2)
            {
                time = PwrStack.Pop();
                player.transform.localScale = new Vector3(1, 1, 1);
                power = 1;
                CurrentEvent = GameEvents.TimeOut;
                GhostRoaming();
            }
        }
        else
        {
            if (PwrStack.Count > 0)
            {
                power = 2;
                Ghostscared();
            }


            power = 1;
            GhostRoaming();
        }

    }



   

    //FSM of Ghost

    public void GhostRoaming()
    {
        SM6.transform.Translate(Vector3.forward * Time.deltaTime / 3);
        SM7.transform.Translate(Vector3.forward * Time.deltaTime / 3);
        SM8.transform.Translate(Vector3.forward * Time.deltaTime / 3);
        SM9.transform.Translate(Vector3.forward * Time.deltaTime / 3);


    }
    public void Ghostscared()
    {
        SM6.transform.Translate(Vector3.forward * Time.deltaTime / 2);
        SM7.transform.Translate(Vector3.forward * Time.deltaTime / 2);
        SM8.transform.Translate(Vector3.forward * Time.deltaTime / 2);
        SM9.transform.Translate(Vector3.forward * Time.deltaTime / 2);


    }
 
    public void Ghostrespawn()
    {
        if (SMtime < 0)
        {
            if (SM6.active == false)
            {
                SM6.SetActive(true);
                SM6.gameObject.transform.position = new Vector3(1, 1, 7);
            }
            if (SM7.active == false)
            {
                SM7.SetActive(true);
                SM7.gameObject.transform.position = new Vector3(1, 1, -5);
            }
            if (SM8.active == false)
            {

                SM8.SetActive(true);
                SM8.gameObject.transform.position = new Vector3(0, 1, 3);
            }
            if (SM9.active == false)
            {
                SM9.SetActive(true);
                SM9.gameObject.transform.position = new Vector3(-11, 1, 4);
            }



        }
    }


    public void EventCheck(string ColliderName, string CollidedWith, GameObject ob)
    {
        if (ColliderName == "claire" && power == 1)
        {
            PacState = PacManStates.SuperPM;
        }

        if (CollidedWith == "claire" && power == 0)
        {

            CurrentEvent = GameEvents.Hunting;
            player.gameObject.transform.position = new Vector3(4, 2, 1);
            player.SetActive(false);
            //   Ghosthunting();
        }
        if (ColliderName == "claire")
        {
            if (CollidedWith == "Sphere (11)" || CollidedWith == "Sphere (12)" || CollidedWith == "Sphere (13)" || CollidedWith == "Sphere (14)"
                      || CollidedWith == "Sphere (15)" || CollidedWith == "Sphere (16)" || CollidedWith == "Sphere (17)" || CollidedWith == "Sphere (18)" || CollidedWith == "Sphere (19)")
            {
                CurrentEvent = GameEvents.EatCrum;
            }
        }

        if (ColliderName == "claire")
        {
            //  Debug.Log("collider = " + col + " other obj = " + target);
            if (CollidedWith == "Capsule" || CollidedWith == "Capsule (1)" || CollidedWith == "Capsule (2)")
            {
                ob.gameObject.SetActive(false);
                CurrentEvent = GameEvents.EatSuperTab;
            }
        }



        switch (PacState)
        {
            case PacManStates.Spawned:

                //       stateOFpac.text = "Spawned";
                player.SetActive(true);

                //  Spawned();
                Debug.Log("Spawned entered");


                CurrentEvent = GameEvents.GameStart;
                break;

            case PacManStates.Roaming:



                break;
            case PacManStates.SuperPM:
                if (CollidedWith == "StoneMonster (9)" && power == 1)
                {


                    SM9.SetActive(false);
                    SMtime = 500;


                }
                if (CollidedWith == "StoneMonster (8)" && power == 1)
                {

                    //  Debug.Log("here in power mode " + otherObj);
                    SMtime = 500;
                    SM8.SetActive(false);


                }
                if (CollidedWith == "StoneMonster (7)" && power == 1)
                {

                    //  Debug.Log("here in power mode " + otherObj);
                    SM7.SetActive(false);

                    SMtime = 500;


                }
                if (CollidedWith == "StoneMonster (6)" && power == 1)
                {

                    //   Debug.Log("here in power mode " + otherObj);
                    SMtime = 500;
                    SM6.SetActive(false);

                }
                break;
            case PacManStates.WinState:

                break;
            case PacManStates.Dead:
                if (life > 0)
                {
                    // player.transform.position = new Vector3(4, 2, 1);
                    //  collision.gameObject.transform.position = new Vector3(4, 2, 1);
                    life--;

                    Debug.Log("Life Lost , lifes left =" + (life));


                    //  PacState = PacManStates.Spawned;
                    CurrentEvent = GameEvents.NewLife;
                    //  PacState = PacManStates.Spawned;      
                }


                else if (life == 0)
                {
                    life--;

                    Destroy(player);

                }

                break;

            default:

                break;
        }


        switch (GhState)
        {
            case GhostStates.ghostrespawn:

                Ghostrespawn();

                break;

            case GhostStates.Ghostroaming:
                //   Debug.Log("roam ghost  ");
                SM6.transform.Translate(Vector3.forward * Time.deltaTime / 3);
                SM7.transform.Translate(Vector3.forward * Time.deltaTime / 3);
                SM8.transform.Translate(Vector3.forward * Time.deltaTime / 3);
                SM9.transform.Translate(Vector3.forward * Time.deltaTime / 3);

                break;
            case GhostStates.Ghostscared:

                SM6.transform.Translate(Vector3.forward * Time.deltaTime / 2);
                SM7.transform.Translate(Vector3.forward * Time.deltaTime / 2);
                SM8.transform.Translate(Vector3.forward * Time.deltaTime / 2);
                SM9.transform.Translate(Vector3.forward * Time.deltaTime / 2);

                break;
            case GhostStates.GhostDead:
                CurrentEvent = GameEvents.TimeOut;

                break;


        }

        switch (CurrentEvent)
        {
            case GameEvents.NewLife:

                PacState = PacManStates.Spawned;
                break;

            case GameEvents.GameStart:
                PacState = PacManStates.Roaming;
                break;

            case GameEvents.EncByPacMan:

                //   GhState = GhostStates.GhostDead;
                break;

            case GameEvents.EncByGhost:
                if (life > 0)
                {
                    // player.transform.position = new Vector3(4, 2, 1);
                    //  collision.gameObject.transform.position = new Vector3(4, 2, 1);
                    life--;
                    Debug.Log("Life Lost , lifes left =" + (life));



                    //  PacState = PacManStates.Spawned;      
                }


                else if (life == 0)
                {
                    life--;
                    // Dead();

                    //   PacState = PacManStates.Dead;
                }
                PacState = PacManStates.Dead;
                break;

            case GameEvents.EatCrum:
                if (ColliderName == "claire")
                {



                    if (CollidedWith == "Sphere (11)" || CollidedWith == "Sphere (12)" || CollidedWith == "Sphere (13)" || CollidedWith == "Sphere (14)"
                        || CollidedWith == "Sphere (15)" || CollidedWith == "Sphere (16)" || CollidedWith == "Sphere (17)" || CollidedWith == "Sphere (18)" || CollidedWith == "Sphere (19)")
                    {
                        //  Debug.Log(crums);


                        ob.gameObject.SetActive(false);
                        //Destroy(col.gameObject);
                        // transform.Rotate(0, 180, 0);
                        //    Debug.Log("rotated" + otherObj);
                        crums = crums + 1;
                        if (crums == 18)
                        {
                            CurrentEvent = GameEvents.CrumsFinished;
                            //  Winstate();
                        }
                    }
                }

                break;
            case GameEvents.EatSuperTab:

                //Debug.Log("here " + otherObj);
                player.transform.localScale = new Vector3(2, 2, 2);
                //  ob.gameObject.SetActive(false);
                time = 1000;
                power = 1;

                GhState = GhostStates.Ghostscared;


                break;
            case GameEvents.AllLivesLost:

                break;
            case GameEvents.CrumsFinished:

                break;
            case GameEvents.Run:

                break;
            case GameEvents.TimeOut:


                if (GhState == GhostStates.Ghostscared)
                {
                    GhState = GhostStates.Ghostroaming;
                }
                //if (time < 0 && life >= 0)
                //{
                //    player.transform.localScale = new Vector3(1, 1, 1);
                //    power = 0;
                //   // GhostRoaming();
                //}
                //else
                //{
                //    power = 1;
                //   // Ghostscared();
                //}
                break;
            case GameEvents.Hunting:


                PacState = PacManStates.Dead;
                break;
        }

    }
}