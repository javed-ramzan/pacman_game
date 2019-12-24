using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacmanController : MonoBehaviour
{


    public enum pacManStates
    {
        spawnedSate,
        roamingState,
        superPMState,
        winState,
        deadState,
        gameOverState

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

      

    private bool isMoving ;
    private int totalCrums ;

    private int Super_pills;

    private bool isDead=false;

    private DateTime _timerforsupermood;
    private int Super_mood_interval;

    private bool isTabletCollision;
    private bool isGhostCollision;

    pacManStates currentState;

    public float MovementSpeed = 0f;

    private Animator animator = null;

    private Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    private Vector3 initialPosition = Vector3.zero;
    //ghost collision
    private Collision _ghostObject;


    static PacManStates PacState = new PacManStates();

   static GameEvents CurrentEvent = new GameEvents();


    static GhostStates GhState = new GhostStates();

 
 

    public void Reset()
    {
        transform.position = initialPosition;
        animator.SetBool("isDead", false);
        animator.SetBool("isMoving", false);

        currentDirection = down;
        currentState = pacManStates.roamingState;
    }

    // Use this for initialization
    void Start()
    {

        QualitySettings.vSyncCount = 0;

        initialPosition = transform.position;
        animator = GetComponent<Animator>();
        currentState = pacManStates.roamingState;
        totalCrums = 0;
        Super_pills = 0;
        Super_mood_interval = 7;
        Reset();


    }

    // Update is called once per frame
    void Update()
    {


        isMoving = true;
        var isDead = animator.GetBool("isDead");
        if (isDead) isMoving = false;
        else if (Input.GetKey(KeyCode.UpArrow)) currentDirection = up;
        else if (Input.GetKey(KeyCode.RightArrow)) currentDirection = right;
        else if (Input.GetKey(KeyCode.DownArrow)) currentDirection = down;
        else if (Input.GetKey(KeyCode.LeftArrow)) currentDirection = left;
        else isMoving = false;
        transform.localEulerAngles = currentDirection;
        animator.SetBool("isMoving", isMoving);


        if (isMoving)
            transform.Translate(Vector3.forward * MovementSpeed * Time.deltaTime);

        changeState();

        Debug.Log("Current State" + currentState);


    



    }




    void changeState()
    {
        switch (currentState)
        {
            case pacManStates.spawnedSate:
                spawnStateCondition();
                break;
            case pacManStates.roamingState:
                roamingStateCondition();
                break;
            case pacManStates.superPMState:
                superPMStateCondition();
                break;
            case pacManStates.winState:
                winStateCondition();
                break;
            case pacManStates.deadState:
                deadStateCondition();
             CurrentEvent = GameEvents.NewLife;
                break;
            case pacManStates.gameOverState:
                gameOverStateCondition();
                break;
            default:
                break;
        }
    }

 

    void spawnStateCondition()
    {
        Debug.Log("spawn");
        // go to roaming state
        currentState = pacManStates.roamingState;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        

    }

    void roamingStateCondition()
    {
        if (currentState == pacManStates.roamingState)
        {
            Debug.Log("roaming");
            //eat crums
            totalCrums = totalCrums + 1;
            CurrentEvent = GameEvents.EatCrum;

            if (isGhostCollision)
            {
                // go to dead state
                isGhostCollision = false;
                isDead = true;
                Debug.Log("Dead");
                currentState = pacManStates.deadState;
            }
            if (isTabletCollision)
            {
               isTabletCollision = false;
               currentState = pacManStates.superPMState;
               _timerforsupermood = DateTime.Now.AddSeconds(Super_mood_interval);
            }
            if (totalCrums == 240)
            {
            //    go to win state
               currentState = pacManStates.winState;
                Debug.Log("Win State....");
            }

        }
        
        


    }

    
    void superPMStateCondition()
    {
        
        Debug.Log("Super state");
        if (isTabletCollision==true)
        {
            // stay here
            isTabletCollision = false;
            _timerforsupermood = DateTime.Now.AddSeconds(Super_mood_interval);
        }
        if (DateTime.Now >= _timerforsupermood)
        {
            //go to normal roaming state
            currentState = pacManStates.roamingState;

        }
        if (isGhostCollision)
        {
            // staty here
            isGhostCollision = false;
            Destroy(_ghostObject.gameObject);
            _ghostObject = null;
           Debug.Log("Ghost Destroyed");

        }
        if (totalCrums == 240)
        {
        //go to win state
            currentState = pacManStates.winState;


        }
    }


   void deadStateCondition()
    {
      //  Debug.Log("Dead State");
        if (isDead==true)
        {
            animator.SetBool("isDead", true);
            currentState = pacManStates.gameOverState;
        }
        if(currentState==pacManStates.deadState)
        {
            // go to game over state
            gameOverStateCondition();
        }
        gameOverStateCondition();

    }

    void winStateCondition()
    {
        Debug.Log("Win ");
        if (totalCrums == 240)
        {
            currentState = pacManStates.winState;
            Debug.Log("Win State Condition True");
            Reset();
        }
    }
    void gameOverStateCondition()
    {

        if(isDead==true)
        {
            // show main menu
            Debug.Log("Pacman dead");
            SceneManager.LoadScene("menu");
            Reset();
        }


    }



    public void EventCheck(string ColliderName, string CollidedWith, GameObject ob)
    {
        // states 
        switch (CurrentEvent)
        {
            case GameEvents.NewLife:

                PacState = PacManStates.Spawned;
                break;

            case GameEvents.GameStart:
                PacState = PacManStates.Roaming;
                break;

            case GameEvents.EncByPacMan:

                GhState = GhostStates.GhostDead;
                break;

            case GameEvents.EatCrum:
                if (ColliderName == "pacman")
                {
                      totalCrums = totalCrums + 1;  
                        if (totalCrums == 200)
                        {
                            CurrentEvent = GameEvents.CrumsFinished;
                        //  Winstate();
                        PacState = PacManStates.WinState;
                        }
                }

                break;
            case GameEvents.EatSuperTab:
                

                GhState = GhostStates.Ghostscared;
                PacState = PacManStates.SuperPM;

                break;
            case GameEvents.AllLivesLost:
                PacState = PacManStates.Dead;
                break;
            case GameEvents.CrumsFinished:
                PacState = PacManStates.WinState;
                break;
            case GameEvents.Run:
                PacState = PacManStates.Roaming;
                break;
            case GameEvents.TimeOut:


                if (GhState == GhostStates.Ghostscared)
                {
                    GhState = GhostStates.Ghostroaming;
                }

                break;
            case GameEvents.Hunting:


                PacState = PacManStates.Dead;
                break;
        }


    }
    //end of states



     
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Pick_Up"))
        {
            other.gameObject.SetActive(false);

            totalCrums = totalCrums + 1;
        }

        if (other.CompareTag("Enemy"))
        {
            deadStateCondition();
        }

        if(other.CompareTag("SuperTablet"))
        {
            superPMStateCondition();
           PacState = PacManStates.SuperPM;

        }


    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("SuperTablet"))
        {
            PacState = PacManStates.SuperPM;
            isTabletCollision = true;
            Destroy(col.gameObject);
 
        }
        if (col.gameObject.CompareTag("Enemy"))
        {
            isGhostCollision = true;
            _ghostObject = col;
        }
    }

 




}
