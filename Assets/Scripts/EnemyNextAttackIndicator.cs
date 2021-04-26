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

    [SerializeField]
    private Image lifeBar;
    [SerializeField]
    private Text life;

    [SerializeField]
    private Image blocks;
    [SerializeField]
    private Text blockAmountText;


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

    public void UpdateLifeBar(float ratio, int currentlife, int maxlife)
    {
        lifeBar.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Max(ratio, 0), 1, 1);
        life.text = Mathf.Max(currentlife, 0) + " / " + maxlife;
    }

    public void UpdateBlocks(int blockAmount)
    {
        if (blockAmount <= 0)
        {
            blocks.gameObject.SetActive(false);
        }
        else
        {
            blocks.gameObject.SetActive(true);
            blockAmountText.text = blockAmount.ToString();
        }
    }

}
