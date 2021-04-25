using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Menu,
    StartGame,
    PlayerTurn,
    EnemyTurn,
    Pause,
    WinLevel,
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

    [SerializeField]
    private GameObject EnemyPrefab;
    private EnemyBehaviour currentEnemy;

    [SerializeField]
    private GameObject PlayerPrefab;
    private Player player;

    int currentLevel = 1;

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
                case States.StartGame:
                    onStartGameStateEnter();
                    break;
                case States.PlayerTurn:
                    onPlayerTurnStateEnter();
                    break;
                case States.EnemyTurn:
                    onEnemyTurnStateEnter();
                    break;
                case States.WinLevel:
                    onWinLevelStateEnter();
                    break;
                case States.Lose:
                    onLoseStateEnter();
                    break;
                case States.Pause:
                    onPauseStateEnter();
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            player.TakeDamage(10000000);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentEnemy.takeDamage(100000000);
        }
#endif
    }

    private void onMenuStateEnter()
    {
        MenuContainer.SetActive(true);
        GameContainer.SetActive(false);
        GameOverContainer.SetActive(false);
    }

    private void onStartGameStateEnter()
    {
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
        currentLevel = 1;
        createEnemy();
        createPlayer();
        currentState = States.PlayerTurn;

    }

    private void onPlayerTurnStateEnter()
    {

    }

    private void onEnemyTurnStateEnter()
    {
        currentEnemy.Play();
    }

    private void onWinLevelStateEnter()
    {
        StartCoroutine(WinLevel());
    }

    private void onLoseStateEnter()
    {
        MenuContainer.SetActive(false);
        GameContainer.SetActive(false);
        GameOverContainer.SetActive(true);
        StartCoroutine(LoseGame());
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


    private void createEnemy()
    {
        GameObject go = Instantiate(EnemyPrefab, GameContainer.transform);
        currentEnemy = go.GetComponent<EnemyBehaviour>();
        currentEnemy.MaxLives = 10 * currentLevel;
    }
    private void createPlayer()
    {
        GameObject go = Instantiate(PlayerPrefab, GameContainer.transform);
        player = go.GetComponent<Player>();
    }

    private IEnumerator WinLevel()
    {
        //TODO: lancer animation fin level
        yield return new WaitForSeconds(5);
        currentLevel++;
        createEnemy();
        currentState = States.PlayerTurn;
    }

    private IEnumerator LoseGame()
    {

        yield return new WaitForSeconds(5);
        currentState = States.Menu;
    }

}
