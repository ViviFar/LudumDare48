using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum States
{
    Menu,
    StartGame,
    PlayerTurn,
    EnemyTurn,
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
    private VideoPlayer vp;
    [SerializeField]
    private GameObject winContinueQuestionContainer;
    [SerializeField]
    private GameObject textFrstLvl;
    [SerializeField]
    private GameObject textScndLvl;

    [SerializeField]
    private GameObject EnemyPrefab;
    private EnemyBehaviour currentEnemy;
    public EnemyBehaviour CurrentEnemy { get { return currentEnemy; } }

    [SerializeField]
    private GameObject PlayerPrefab;
    private Player plyr;
    public Player Plyr { get { return plyr; } }

    int currentLevel = 1;
    public int CurrentLevel { get { return currentLevel; } }

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
            plyr.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.W) && plyr != null)
        {
            currentEnemy.takeDamage(100000);
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
        vp.gameObject.SetActive(false);
        winContinueQuestionContainer.SetActive(false);
        textFrstLvl.SetActive(false);
        textScndLvl.SetActive(false);
    }

    private void onStartGameStateEnter()
    {
        paused = false;
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
        PauseContainer.SetActive(false);
        vp.gameObject.SetActive(false);
        winContinueQuestionContainer.SetActive(false);
        textFrstLvl.SetActive(false);
        textScndLvl.SetActive(false);
        currentLevel = 1;
        createFirstLevel();
        createPlayer();
        playedCardsManager.HideCardFirstLevel();
        currentState = States.PlayerTurn;
        BlurManager.Instance.Blur();
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
        vp.gameObject.SetActive(false);
        winContinueQuestionContainer.SetActive(false);
        textFrstLvl.SetActive(false);
        textScndLvl.SetActive(false);
        StartCoroutine(LoseGame());
    }
   

    #endregion

    public void NextTurnButtonPressed()
    {
        currentState = States.EnemyTurn;
        textFrstLvl.SetActive(false);
        textScndLvl.SetActive(false);
    }

    public void Resume()
    {
        paused = false;
        MenuContainer.SetActive(false);
        GameContainer.SetActive(true);
        GameOverContainer.SetActive(false);
        PauseContainer.SetActive(false);
        vp.gameObject.SetActive(false);
        winContinueQuestionContainer.SetActive(false);
        if (currentLevel == 1)
        {
            textFrstLvl.SetActive(true);
            textScndLvl.SetActive(false);
        }
        else if(currentLevel == 2)
        {
            textFrstLvl.SetActive(false);
            textScndLvl.SetActive(true);
        }
        else
        {
            textFrstLvl.SetActive(false);
            textScndLvl.SetActive(false);
        }
    }

    public void BackToMenu()
    {
        Destroy(plyr.gameObject);
        Destroy(currentEnemy.gameObject);
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
        plyr.ResetPlayer();
    }

    private IEnumerator WinLevel()
    {
        if (currentLevel != 5)
        {
            enemyAnim.SetBool("Died", true);
            //TODO: lancer animation fin level
            yield return new WaitForSeconds(3);
            enemyAnim.SetBool("Died", false);
            yield return new WaitForSeconds(1);
            currentLevel++;
            playedCardsManager.HideCardFirstLevel();
            playedCardsManager.ResetCards();
            playedCardsManager.NbPlayedCards = 0;
            switch (currentLevel)
            {
                case 2:
                    cardContainers[1].gameObject.SetActive(true);
                    cardContainers[2].gameObject.SetActive(true);
                    currentEnemy.Attack = 7;
                    currentEnemy.MaxLives = 19;
                    textScndLvl.SetActive(true);
                    break;
                case 3:
                    cardContainers[3].gameObject.SetActive(true);
                    cardContainers[4].gameObject.SetActive(true);
                    currentEnemy.MaxLives = 173;
                    currentEnemy.Attack = 8;
                    currentEnemy.BlockAmount = 20;
                    currentEnemy.AttackAndBlockAmount = 5;
                    break;
                case 4:
                    currentEnemy.MaxLives = 346;
                    currentEnemy.Attack = 12;
                    currentEnemy.BlockAmount = 30;
                    currentEnemy.AttackAndBlockAmount = 8;
                    break;
                case 5:
                    currentEnemy.MaxLives = 519;
                    currentEnemy.Attack = 20;
                    currentEnemy.BlockAmount = 50;
                    currentEnemy.AttackAndBlockAmount =10;
                    break;
                default:
                    currentEnemy.MaxLives +=70;
                    currentEnemy.Attack = 20;
                    currentEnemy.BlockAmount = 50;
                    currentEnemy.AttackAndBlockAmount = 10;
                    break;
            }
            currentEnemy.ResetEnemy();
            currentState = States.PlayerTurn;
        }
        else
        {
            PauseContainer.SetActive(false);
            vp.gameObject.SetActive(true);
            winContinueQuestionContainer.SetActive(false);
            vp.isLooping = false;
            vp.Play();
            yield return new WaitForSeconds((float)vp.clip.length);
            vp.gameObject.SetActive(false);
            winContinueQuestionContainer.SetActive(true);
        }
    }

    private IEnumerator LoseGame()
    {
        DestroyEnemy();
        yield return new WaitForSeconds(5);
        currentState = States.Menu;
    }

    public void DestroyPlayer()
    {
        plyr = null;
    }

    public void DestroyEnemy()
    {
        Destroy(currentEnemy.gameObject);
        currentEnemy = null;
    }
    
    private void createFirstLevel()
    {
        createEnemy();
        currentEnemy.MaxLives = 3;
        currentEnemy.Attack = 2;
        for(int i=1; i<cardContainers.Count; i++)
        {
            cardContainers[i].gameObject.SetActive(false);
        }
        textFrstLvl.SetActive(true);
    }

    public void continueAfterWin()
    {
        currentLevel++;
        playedCardsManager.HideCardFirstLevel();
        playedCardsManager.ResetCards();
        playedCardsManager.NbPlayedCards = 0;
        currentEnemy.MaxLives += 10 * currentLevel;
        currentEnemy.ResetEnemy();
        currentState = States.PlayerTurn;
        Resume();
    }


    public void deactivateContainers()
    {
        foreach(PlayedCardContainer ct in cardContainers)
        {
            if (ct.gameObject.activeInHierarchy)
            {
                ct.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void reactivateContainers()
    {
        foreach (PlayedCardContainer ct in cardContainers)
        {
            if (ct.gameObject.activeInHierarchy)
            {
                ct.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
}
