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

    [SerializeField]
    private int effectStrength = 1;
    public int EffectStrength { get { return effectStrength; } }

    public void ApplyEffect()
    {
        switch (effect)
        {
            case CardEffect.Damage:
            case CardEffect.Block:
            case CardEffect.Copy:
            case CardEffect.Extend:
            case CardEffect.Multiply:
            default:
                break;
        }
    }
}
