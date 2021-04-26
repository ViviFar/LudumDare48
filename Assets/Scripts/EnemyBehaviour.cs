using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }
    [SerializeField]
    private int blockAmount = 10;
    public int BlockAmount
    {
        get { return blockAmount; }
        set { blockAmount = value; }
    }
    private int currentBlocks = 0;

    private int attackAndBlockAmount = 5;
    public int AttackAndBlockAmount {
        get { return attackAndBlockAmount; }
        set { attackAndBlockAmount = value; }
    }

    private int nextAttack = 1;
    public int NextAttack { get { return nextAttack; } }

    private EnemyNextAttackIndicator indic;

    private void Start()
    {
        indic = GetComponent<EnemyNextAttackIndicator>();
        ResetEnemy();
    }

    public void ResetEnemy()
    {
        currentBlocks = 0;
        indic.UpdateBlocks(currentBlocks);
        nextAttack = 1;
        lives = maxLives;
        indic.updateNExtAttackVisual(nextAttack, attack);
        indic.UpdateLifeBar(1, maxLives, maxLives);
    }

    public void prepareNextAttack()
    {
        if (StateMachine.Instance.CurrentLevel == 1)
        {
            indic.updateNExtAttackVisual(1, 2);
            return;
        }
        nextAttack = UnityEngine.Random.Range(1, 4);
        int amountOfNextAttack = nextAttack == 1 ? attack : (nextAttack == 2 ? blockAmount : attackAndBlockAmount);
        indic.updateNExtAttackVisual(nextAttack, amountOfNextAttack);
    }

    public void Play()
    {
        if (StateMachine.Instance.CurrentState != States.EnemyTurn)
        {
            return;
        }
        StartCoroutine(PlayTurn());
    }

    IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(1);
        currentBlocks = 0;
        switch (nextAttack)
        {
            case 1:
                EnemyAttack(attack);
                break;
            case 2:
                AddBlocks(blockAmount);
                break;
            case 3:
                AttackAndBlock(attackAndBlockAmount);
                break;
            default:
                break;
        }
        indic.UpdateBlocks(currentBlocks);
        yield return new WaitForSeconds(3);
        prepareNextAttack();
        if (StateMachine.Instance.CurrentState == States.EnemyTurn)
            StateMachine.Instance.CurrentState = States.PlayerTurn;
    }

    public void takeDamage(int damage)
    {
        lives -= Mathf.Max((damage-currentBlocks), 0);
        if (currentBlocks > 0)
        {
            //not really useful, just to update the visual
            currentBlocks = Mathf.Max((currentBlocks - damage ), 0);
            indic.UpdateBlocks(currentBlocks);
        }
        indic.UpdateLifeBar((float)lives / (float)(maxLives), lives, maxLives);
        if (lives <= 0)
        {
            StateMachine.Instance.CurrentState = States.WinLevel;

        }
    }

    public void AddBlocks(int amount)
    {
        currentBlocks += amount;
    }

    public void EnemyAttack(int damage)
    {
        if(StateMachine.Instance.Plyr!= null)
            StateMachine.Instance.Plyr.TakeDamage(damage);
    }

    public void AttackAndBlock(int amount)
    {
        EnemyAttack(amount);
        AddBlocks(amount);
    }
}
