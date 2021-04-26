using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNextAttackIndicator: MonoBehaviour { 

    [SerializeField]
    private Text nextAttackText;
    [SerializeField]
    private Image nextAttackImage;

    [SerializeField]
    private Sprite attackSprite;
    [SerializeField]
    private Sprite blockSprite;
    [SerializeField]
    private Sprite attackAndBlockSprite;
    

    public void updateNExtAttackVisual(int actionType, int amount)
    {
        switch (actionType)
        {
            case 1:
                nextAttackText.text = amount + "";
                nextAttackImage.sprite = attackSprite;
                break;
            case 2:
                nextAttackText.text = amount + "";
                nextAttackImage.sprite = blockSprite;
                break;
            case 3:
                nextAttackText.text = amount + "";
                nextAttackImage.sprite = attackAndBlockSprite;
                break;
            default:
                break;
        }
    }

}
