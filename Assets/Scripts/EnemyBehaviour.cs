using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private int lives = 10;

    public void Play()
    {
        StateMachine.Instance.CurrentState = States.PlayerTurn;
    }
}
