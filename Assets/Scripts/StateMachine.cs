using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject PauseContainer;

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
    [SerializeField]
    private Button endTurnButton;

    private Animator enemyAnim;

    private bool paused = false;

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
                endTurnButton.interactable = false;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == States.PlayerTurn || currentState == States.EnemyTurn)
            {
                if (paused)
                {
                    Resume();
                }
                else
                {
                    paused = true;
                    MenuContainer.SetActive(false);
                    GameContainer.SetActive(false);
                    GameOverContainer.SetActive(false);
                    PauseContainer.SetActive(true);
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L) && plyr != null)
        {
            plyr.TakeDamage(10000000);
        }
        if (Input.GetKeyDown(KeyCode.W) && plyr != null)
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
        PauseContainer.SetActive(false);
    }

    private void onStartGameStateEnter()
    {
        paused = false;
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
        PauseContainer.SetActive(false);
        currentLevel = 1;
        createEnemy();
        createPlayer();
        currentState = States.PlayerTurn;
    }

    private void onPlayerTurnStateEnter()
    {
        endTurnButton.interactable = true;
        if(plyr != null)
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
        PauseContainer.SetActive(false);
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

    public void Resume()
    {
        paused = false;
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
        PauseContainer.SetActive(false);
    }

    public void BackToMenu()
    {
        currentState = States.Menu;
    }

    public void Exit()
    {
        Application.Quit();
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
        SoundManager.Instance.EnemySource = currentEnemy.GetComponent<AudioSource>();
        enemyAnim = currentEnemy.GetComponent<Animator>();
    }
    private void createPlayer()
    {
        GameObject go = Instantiate(PlayerPrefab, GameContainer.transform);
        plyr = go.GetComponent<Player>();
        SoundManager.Instance.PlayerSource = plyr.GetComponent<AudioSource>();
    }

    private IEnumerator WinLevel()
    {
        enemyAnim.SetBool("Died", true);
        //TODO: lancer animation fin level
        yield return new WaitForSeconds(5);
        enemyAnim.SetBool("Died", false);
        currentLevel++;
        currentEnemy.MaxLives = 10 * currentLevel;
        currentEnemy.ResetEnemy();
        currentState = States.PlayerTurn;
    }

    private IEnumerator LoseGame()
    {

        yield return new WaitForSeconds(5);
        currentState = States.Menu;
    }

    public void DestroyPlayer()
    {
        plyr = null;
    }
    
}
