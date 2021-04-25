using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPlayedCardsEffect : MonoBehaviour
{
    private List<PlayedCardContainer> containers;

    [SerializeField]
    private List<CardBehaviour> allCards;
    private int nbPlayedCards = 0;

    //private List<CardBehaviour> cardsPlayed;
    CardBehaviour[] cardsPlayed;
    bool[] extended;
    int[] multiply;
    CardEffect[] copy;
    int[] damage;

    private void Start()
    {
        containers = StateMachine.Instance.CardContainers;
        initializeTurn();
    }

    private void initializeTurn()
    {
        cardsPlayed = new CardBehaviour[5];
        extended = new bool[5];
        multiply = new int[5];
        copy = new CardEffect[5];
        damage = new int[5];
        for (int i = 0; i < cardsPlayed.Length; i++)
        {
            extended[i] = false;
            multiply[i] = 1;
            //we put Copy in it: with that done, when applying the effects, we will only check those differents from copy
            copy[i] = CardEffect.Copy;
            damage[i] = 0;
        }
    }

    public void PlayTurn()
    {
        GetCardsPlayed();
        CalculateEffect();
        if (nbPlayedCards >= allCards.Count)
        {
            ResetCards();
        }
        else
        {
            PutCardsInDiscard();
        }
    }

    private void PutCardsInDiscard()
    {
        for (int i = 0; i < cardsPlayed.Length; i++)
        {
            if (cardsPlayed[i] != null)
            {
                cardsPlayed[i].GetComponent<DragDropCard>().ResetCardPlacement();
                cardsPlayed[i].gameObject.SetActive(false);
            }
        }
    }

    private void ResetCards()
    {
        foreach(CardBehaviour c in allCards)
        {
            c.gameObject.SetActive(true);
            c.GetComponent<DragDropCard>().ResetCardPlacement();
        }
    }

    private void GetCardsPlayed()
    {
        initializeTurn();
        for (int i = 0; i < cardsPlayed.Length; i++)
        {
            CardBehaviour cb = containers[i].GetComponentInChildren<CardBehaviour>();
            cardsPlayed[i] = cb;
            if (cb != null)
            {
                nbPlayedCards++;
            }
        }
    }

    private void CalculateEffect()
    {
        for(int i=0; i<cardsPlayed.Length; i++)
        {
            CardBehaviour cb = cardsPlayed[i];
            if (cb != null)
            {
                if (cb.Effect == CardEffect.Copy)
                {
                    if (cb.AffectCardOnRight && i < cardsPlayed.Length - 1)
                    {
                        cb = cardsPlayed[i + 1];
                        multiply[i] *= multiply[i + 1];
                    }
                    if(!cb.AffectCardOnRight && i > 0)
                    {
                        cb = cardsPlayed[i - 1];
                        multiply[i] *= multiply[i - 1];
                    }
                }
                switch (cb.Effect)
                {
                    case CardEffect.Extend:
                        if (i < cardsPlayed.Length - 1)
                        {
                            extended[i + 1] = true;
                            if(extended[i] && i < cardsPlayed.Length - 2)
                            {
                                extended[i + 2] = true;
                            }
                        }
                        break;
                    case CardEffect.Multiply:
                        if(i < cardsPlayed.Length - 1)
                        {
                            multiply[i + 1] *= multiply[i]* cb.EffectStrength;
                            if (extended[i] && i < cardsPlayed.Length - 2)
                            {
                                multiply[i + 2] *= multiply[i] * cb.EffectStrength;
                            }
                        }
                        break;
                    case CardEffect.Block:
                        Debug.Log("blocking for " + multiply[i] * cb.EffectStrength + " damage");
                        StateMachine.Instance.Plyr.AddArmor(multiply[i] * cb.EffectStrength);
                        break;
                    case CardEffect.Damage:
                        Debug.Log("attacking for " + multiply[i] * cb.EffectStrength + " damage");
                        StateMachine.Instance.CurrentEnemy.takeDamage(multiply[i] * cb.EffectStrength);
                        break;
                }
            }
        }
    }

    #region calculateEffectComplexe
    //private void CalculateEffect()
    //{
    //    //first look for extend effect
    //    ApplyExtend();
    //    //then look for multiply
    //    ApplyMultiply();
    //    //then for copy
    //    ApplyCopy();
    //    //finally apply the basic card with all effect 
    //    //(damage then blocks if 2 are played in the same round but it should not matter as 
    //    for (int i = 0; i < cardsPlayed.Length; i++)
    //    {
    //        CardBehaviour card = cardsPlayed[i];
    //        if (card != null)
    //        {
    //            if (card.Effect == CardEffect.Damage)
    //            {
    //                damage[i] = card.EffectStrength;
    //            }
    //            else if(card.Effect == CardEffect.Block)
    //            {
    //                damage[i] = -card.EffectStrength;
    //            }
    //        }
    //    }
    //}

    //private void ApplyExtend()
    //{
    //    for (int i = 0; i < cardsPlayed.Length; i++)
    //    {
    //        CardBehaviour card = cardsPlayed[i];
    //        if (card != null)
    //        {
    //            if (card.Effect == CardEffect.Extend)
    //            {
    //                if (card.AffectCardOnRight && i < cardsPlayed.Length - 1)
    //                {
    //                    extended[i + 1] = true;
    //                }
    //                else if (!card.AffectCardOnRight && i > 0)
    //                {
    //                    extended[i - 1] = true;
    //                }
    //            }
    //        }
    //    }
    //}


    //private void ApplyMultiply()
    //{
    //    for (int i = 0; i < cardsPlayed.Length; i++)
    //    {
    //        CardBehaviour card = cardsPlayed[i];
    //        if (card != null)
    //        {
    //            if (card.Effect == CardEffect.Multiply)
    //            {
    //                if (card.AffectCardOnRight && i < cardsPlayed.Length - 1)
    //                {
    //                    multiply[i + 1] *= card.EffectStrength;
    //                    if (extended[i] && i < cardsPlayed.Length - 2)
    //                    {
    //                        multiply[i + 2] *= card.EffectStrength;
    //                    }
    //                }
    //                else if (!card.AffectCardOnRight && i > 0)
    //                {
    //                    multiply[i - 1] *= card.EffectStrength;
    //                    if (extended[i] && i > 1)
    //                    {
    //                        multiply[i - 2] *= card.EffectStrength;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //private void ApplyCopy()
    //{
    //    for (int i = 0; i < cardsPlayed.Length; i++)
    //    {
    //        CardBehaviour card = cardsPlayed[i];
    //        if (card != null)
    //        {
    //            if (card.Effect == CardEffect.Copy)
    //            {
    //                if (card.AffectCardOnRight && i < cardsPlayed.Length - 1)
    //                {

    //                }
    //                if (!card.AffectCardOnRight && i > 0)
    //                {

    //                }
    //            }
    //        }
    //    }
    //}

    //private void copyRight(int index, bool extendedCopy)
    //{
    //    copy[index + 1] = cardsPlayed[index + 1].Effect;

    //    switch (copy[index + 1])
    //    {
    //        case CardEffect.Copy:
    //            if (index > 0)
    //            {
    //                copyLeft(index, false);
    //            }
    //            break;
    //        case CardEffect.Multiply:
    //            if(cardsPlayed[index + 1].AffectCardOnRight && index < cardsPlayed.Length - 2)
    //            {
    //                multiply[index + 2] *= multiply[index] * multiply[index + 1] * cardsPlayed[index + 1].EffectStrength;
    //            }
    //            else if(!cardsPlayed[index + 1].AffectCardOnRight && index > 0)
    //            {
    //                multiply[index - 1] *= multiply[index+1] * multiply[index] * cardsPlayed[index + 1].EffectStrength;
    //            }
    //            break;
    //        case CardEffect.Extend:
    //            if(cardsPlayed[index + 1].AffectCardOnRight && index < cardsPlayed.Length - 2)
    //            {

    //            }
    //            break;
    //    }

    //    if (extendedCopy && index < cardsPlayed.Length - 2)
    //    {
    //        copy[index + 2] = cardsPlayed[index + 2].Effect;
    //    }

    //}
    //private void copyLeft(int index, bool extendedCopy)
    //{
    //    copy[index - 1] = cardsPlayed[index - 1].Effect;

    //    switch (copy[index - 1])
    //    {
    //        case CardEffect.Copy:

    //            break;
    //    }

    //    if (extendedCopy && index >1)
    //    {
    //        copy[index - 2] = cardsPlayed[index - 2].Effect;
    //    }
    //}
    #endregion
}
