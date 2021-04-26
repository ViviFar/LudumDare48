using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenuPanel;

    [SerializeField]
    private GameObject CreditPanel;

    // Start is called before the first frame update
    void Start()
    {
        ActivateMenu();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateMenu()
    {
        MainMenuPanel.SetActive(true);
        CreditPanel.SetActive(false);
    }

    public void ActivateCredit()
    {
        MainMenuPanel.SetActive(false);
        CreditPanel.SetActive(true);
    }

    public void PlayButtonClick()
    {
        StateMachine.Instance.CurrentState = States.StartGame;
    }
}
