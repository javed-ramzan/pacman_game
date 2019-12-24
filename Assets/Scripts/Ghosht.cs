using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Ghosht : MonoBehaviour
{

    public GameObject target;
    NavMeshAgent agent;
    public enum GhostState
    {
        sponSate,
        roamingState,
        chasepacman,
        awayState,
        dead
    }

    public enum t_g_transition
    {
        game_start,
        chase_pacman,
        time_out,
        supertablet_eaten_by_pacman,
        destroy_pacman,
        eaten_by_pacman
    }



    public enum HeadMoveDirection
    {
        LEFT,
        RIGHT,
        STAY
    }

    HeadMoveDirection headmovedirection;

    GhostState gcurrentState;

    private int Super_mood_interval;

    private System.DateTime _timerforsupermood;

    static PacManStates PacState = new PacManStates();



    static t_g_transition CurrentTransition = new t_g_transition();

    static GhostStates GhState = new GhostStates();





    HeadMoveDirection curdir;

    public static Stack<GameObject> ob = new Stack<GameObject>();

    public Transform targt;
    public float speed = 5f;
    private float minDistance = 3f;
    private float range;
    private bool isgdead;

    // Use this for initialization
    void Start()
    {
        gcurrentState = GhostState.roamingState;

        Super_mood_interval = 2;

        agent = GetComponent<NavMeshAgent>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Pacman");

    }

    // Update is called once per frame
    void Update()
    {

        if (isgdead == true)
        {
            SetTransformX(450.0f);
        }
        ghostchangeState();

    }


    void ghostchangeState()
    {
        switch (gcurrentState)
        {
            case GhostState.sponSate:
                Debug.Log("Spon state");

                switch (curdir)
                {
                    case HeadMoveDirection.RIGHT:
                        gcurrentState = GhostState.roamingState;
                        break;

                    case HeadMoveDirection.STAY:
                        gcurrentState = GhostState.sponSate;
                        break;
                    default:
                        break;
                }
                sponState();
                break;
            case GhostState.roamingState:
                //roaming state
                switch (curdir)
                {
                    case HeadMoveDirection.RIGHT:
                        gcurrentState = GhostState.chasepacman;
                        break;

                    case HeadMoveDirection.STAY:
                        gcurrentState = GhostState.roamingState;
                        break;
                    default:
                        break;
                }
                roamingState();
                break;
            case GhostState.chasepacman:
                //chase pacman
                switch (curdir)
                {
                    case HeadMoveDirection.LEFT:
                        gcurrentState = GhostState.awayState;
                        break;
                    case HeadMoveDirection.STAY:
                        gcurrentState = GhostState.chasepacman;
                        break;
                    default:
                        break;
                }
                chasePState();
                break;
            case GhostState.awayState:
                //run away from pacman
                switch (curdir)
                {
                    case HeadMoveDirection.RIGHT:
                        gcurrentState = GhostState.roamingState;
                        break;

                    case HeadMoveDirection.STAY:
                        gcurrentState = GhostState.awayState;
                        break;
                    default:
                        break;
                }
                awayState();
                break;
            case GhostState.dead:
                switch (curdir)
                {
                    case HeadMoveDirection.LEFT:
                        SetTransformX(450.0f);
                        break;
                    default:
                        break;
                }
                dead();
                break;
            default:
                break;

        }
    }

    void dead()
    {
        isgdead = true;
    }

    void sponState()
    {
        // go to roaming state
        gcurrentState = GhostState.roamingState;
        Debug.Log(" Ghost Spon State");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (CurrentTransition == t_g_transition.game_start)
        {
            // go to roaming state
            gcurrentState = GhostState.roamingState;
            Debug.Log(" Ghost Spon State");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            gcurrentState = GhostState.roamingState;
        }



    }

    void roamingState()
    {
        gcurrentState = GhostState.roamingState;
        Debug.Log("Ghost  Roaming State");

        agent.destination = target.transform.position;



        if (CurrentTransition == t_g_transition.supertablet_eaten_by_pacman)
        {
            gcurrentState = GhostState.awayState;
        }
        if (CurrentTransition == t_g_transition.time_out)
        {
            gcurrentState = GhostState.roamingState;
            Debug.Log("Ghost  Roaming State");
            agent.destination = target.transform.position;
        }
        if (CurrentTransition == t_g_transition.eaten_by_pacman)
        {
            gcurrentState = GhostState.dead;
        }
        if (CurrentTransition == t_g_transition.destroy_pacman)
        {
            Debug.Log("Winners");
        }
    }

    void chasePState()
    {
        Debug.Log("chase state");
        agent.destination = target.transform.position;

    }

    void awayState()
    {
        target.transform.position = transform.TransformPoint(0, 2, 1);
    }


    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Pacman")
        {
            if (PacState == PacManStates.SuperPM)
            {
                gameObject.SetActive(false);
                ob.Push(gameObject);
                if (ob.Count <= 3)
                {
                    GameObject n = ob.Pop();
                    n.SetActive(true);
                }

            }
        }


    }


    void SetTransformX(float n)
    {
        transform.position = new Vector3(n, transform.position.y, transform.position.z);
    }


    public class State
    {
        readonly public string Name;
        public bool IsInitialState;
        public bool IsAcceptState;
        public bool IsRejectState;

        public State(string name)
        {
            Name = name;
        }
    }

    public class Transition
    {
        public State CurrentState;
        public State NextState;
        public string Read;
        public string Write;
        public HeadMoveDirection HeadMoveDirection;

        public Transition(State current, string read, string write, HeadMoveDirection headMoveDirection, State next)
        {
            this.CurrentState = current;
            this.Read = read;
            this.Write = write;
            this.HeadMoveDirection = headMoveDirection;
            this.NextState = next;
        }
    }

    public class TurningMachine
    {
        private List<string> _InputTape;
        public int InputTapeSize;
        public string HeadContent
        {
            get
            {
                return _InputTape[_HeadPosition];
            }
        }

        private int _HeadPosition;
        public HeadMoveDirection HeadMoveDirection;

        public List<Transition> Transitions;

        public State Currentstate;

        public TurningMachine()
        {

            InputTapeSize = 100;
          

            _HeadPosition = _InputTape.Capacity / 2;
            Transitions = new List<Transition>();
        }

    

        public void AddTransition(State currentState, string read, string write, HeadMoveDirection headMoveDirection, State nextState)
        {
            Transitions.Add(new Transition(currentState, read, write, headMoveDirection, nextState));
        }

        public void Run()
        {
            while (true)
            {
                foreach (var transition in Transitions)
                {
                    if ((transition.CurrentState.Name == this.Currentstate.Name) && transition.Read == this.HeadContent)
                    {
                        switch (transition.HeadMoveDirection)
                        {
                            case HeadMoveDirection.RIGHT:

                                _InputTape[_HeadPosition] = transition.Write;
                                _HeadPosition++;

                                break;
                            case HeadMoveDirection.LEFT:

                                _InputTape[_HeadPosition] = transition.Write;
                                _HeadPosition--;

                                break;
                            case HeadMoveDirection.STAY:
                                _InputTape[_HeadPosition] = transition.Write;
                                break;
                        }
                        this.Currentstate = transition.NextState;
                    }
                }

                if (this.Currentstate.IsAcceptState)
                {
                    break;
                }
            }
        }
    }
}




        