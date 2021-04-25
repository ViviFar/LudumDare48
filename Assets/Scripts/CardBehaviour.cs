using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardEffect
{
    Damage,
    Block,
    Multiply,
    Copy,
    Extend
}

public class CardBehaviour : MonoBehaviour
{
    [SerializeField]
    private CardEffect effect;
    public CardEffect Effect { get{ return effect; } }

    [SerializeField]
    private bool affectCardOnRight;
    public bool AffectCardOnRight { get { return affectCardOnRight; } }
}
