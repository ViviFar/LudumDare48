using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private int maxLives = 10;
    public int MaxLives
    {
        get { return maxLives; }
        set { maxLives = value; }
    }
    [SerializeField]
    private int lives;

    [SerializeField]
    private int attack = 10;
    [SerializeField]
    private int armor = 0;

    private void Start()
    {
        ResetEnemy();
    }

    private void ResetEnemy()
    {
        lives = maxLives;
    }

    public void Play()
    {
        armor = 0;
        GetArmor(5);
        StateMachine.Instance.Plyr.TakeDamage(10);
        if (StateMachine.Instance.CurrentState != States.EnemyTurn)
        {
            return;
        }
        StateMachine.Instance.CurrentState = States.PlayerTurn;
    }

    public void takeDamage(int damage)
    {
        lives -= Mathf.Max((damage - armor), 0);
        if (lives <= 0)
        {
            StateMachine.Instance.CurrentState = States.WinLevel;
            Destroy(this.gameObject);
        }
    }

    public void GetArmor(int arm)
    {
        armor += arm;
    }
}
