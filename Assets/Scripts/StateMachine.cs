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
    private List<PlayedCardContainer> cardContainers;
    public List<PlayedCardContainer> CardContainers { get { return cardContainers; } }

    [SerializeField]
    private GameObject MenuContainer;
    [SerializeField]
    private GameObject GameContainer;
    [SerializeField]
    private GameObject GameOverContainer;

    [SerializeField]
    private GameObject EnemyPrefab;
    private EnemyBehaviour currentEnemy;
    public EnemyBehaviour CurrentEnemy { get { return currentEnemy; } }

    [SerializeField]
    private GameObject PlayerPrefab;
    private Player plyr;
    public Player Plyr { get { return plyr; } }

    int currentLevel = 1;

    [SerializeField]
    private ApplyPlayedCardsEffect playedCardsManager;

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
            if(previousState == States.PlayerTurn)
            {
                playedCardsManager.PlayTurn();
            }
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
            plyr.TakeDamage(10000000);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentEnemy.takeDamage(100000000);
        }
#endif
    }

    #region changingState
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
        plyr.ResetArmor();
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

    #endregion

    public void NextTurnButtonPressed()
    {
        currentState = States.EnemyTurn;
    }

    public int DropCardOnContainer(DragDropCard target)
    {
        for(int i=0; i< cardContainers.Count; i++)
        {
            PlayedCardContainer cont = cardContainers[i];
            if (cont.HasCardOver)
            {
                target.transform.SetParent(cont.transform, false);
                target.TargetPos = cont.EmptyImage.transform.position;
                cont.ChangeColliderStatus(false);
                return i;
            }
        }
        return -1;
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
        plyr = go.GetComponent<Player>();
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
