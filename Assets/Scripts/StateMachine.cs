using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Menu,
    PlayerTurn,
    EnemyTurn,
    Pause,
    Win,
    Lose
}


public class StateMachine : GenericSingleton<StateMachine>
{
    [SerializeField]
    private States currentState;
    public States CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }
    private States previousState;

    [SerializeField]
    List<PlayedCardContainer> cardContainers;

    [SerializeField]
    private GameObject MenuContainer;
    [SerializeField]
    private GameObject GameContainer;
    [SerializeField]
    private GameObject GameOverContainer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        previousState = States.Menu;
        currentState = States.Menu;
        onMenuStateEnter();
    }

    private void Update()
    {
        if (currentState != previousState)
        {
            previousState = currentState;
            switch (currentState)
            {
                case States.Menu:
                    onMenuStateEnter();
                    break;
                case States.PlayerTurn:
                    onPlayerTurnStateEnter();
                    break;
                case States.EnemyTurn:
                    break;
                case States.Win:
                    break;
                case States.Lose:
                    break;
                case States.Pause:
                    break;
                default:
                    break;
            }
        }
    }

    private void onMenuStateEnter()
    {
        MenuContainer.SetActive(true);
        GameContainer.SetActive(false);
        GameOverContainer.SetActive(false);
    }

    private void onPlayerTurnStateEnter()
    {
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
    }

    private void onEnemyTurnStateEnter()
    {

    }

    private void onWinStateEnter()
    {

    }

    private void onLoseStateEnter()
    {

    }

    private void onPauseStateEnter()
    {

    }

    public void NextTurnButtonPressed()
    {
        currentState = States.EnemyTurn;
    }

    public bool DropCardOnContainer(DragDropCard target)
    {
        foreach(PlayedCardContainer cont in cardContainers)
        {
            if (cont.HasCardOver)
            {
                target.transform.SetParent(cont.transform);
                target.TargetPos = cont.EmptyImage.transform.position;
                return true;
            }
        }
        return false;
    }
}
