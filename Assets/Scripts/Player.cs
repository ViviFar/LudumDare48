using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int maxLife = 10;
    public int MaxLife { get { return maxLife; } }
    [SerializeField]
    private int currentLife;
    public int CurrentLife { get { return currentLife; } }

    private int armor = 0;

    [SerializeField]
    private Image lifeBar;
    [SerializeField]
    private Text lifeText;
    [SerializeField]
    private Image blocksImage;
    [SerializeField]
    private Text blockText;
    //[SerializeField]
    //private List<Card> hand;

    //private List<Card> defausse;


    public void ResetPlayer()
    {
        currentLife = maxLife;
        updateLife(currentLife, maxLife);
        ResetArmor();
    }
    public void ResetArmor()
    {
        armor = 0;
        updateBlocks(armor);
    }

    public void TakeDamage(int dam)
    {
        if (dam > armor)
        {
            SoundManager.Instance.PlayPlayerHurtSound();
        }
        else
        {
            SoundManager.Instance.PlayPlayerBlockSound();
        }
        currentLife -= Mathf.Max((dam - armor), 0);
        updateLife(currentLife, maxLife);
        armor = armor - dam;
        updateBlocks(armor);
        if (currentLife <= 0)
        {
            StateMachine.Instance.CurrentState = States.Lose;
            SoundManager.Instance.PlayerSource = null;
            StateMachine.Instance.DestroyPlayer();
            Destroy(this.gameObject);
        }
        else
        {
            BlurManager.Instance.Blur();
        }
    }

    public void AddArmor(int arm)
    {
        armor += arm;
        updateBlocks(armor);
    }

    private void updateLife(int crtLfe, int maxLfe)
    {
        lifeBar.rectTransform.localScale = new Vector3((float)crtLfe / (float)maxLfe, 1, 1);
        lifeText.text = crtLfe + " / " + maxLfe;
    }

    private void updateBlocks(int newBlocks)
    {
        if (newBlocks <= 0)
        {
            armor = 0;
            blocksImage.gameObject.SetActive(false);
        }
        else
        {
            blocksImage.gameObject.SetActive(true);
            blockText.text = newBlocks.ToString();
        }
    }
}
