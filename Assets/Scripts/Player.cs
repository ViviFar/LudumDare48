using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int maxLife = 10;
    public int MaxLife { get { return maxLife; } }
    private int currentLife;

    private int armor = 0;

    //[SerializeField]
    //private List<Card> hand;

    //private List<Card> defausse;

    // Start is called before the first frame update
    void Start()
    {
        ResetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayer()
    {
        currentLife = maxLife;
        ResetArmor();
    }
    public void ResetArmor() { armor = 0; }
    public void TakeDamage(int dam)
    {
        currentLife -= Mathf.Max((dam-armor), 0);
        if (currentLife <= 0)
        {
            StateMachine.Instance.CurrentState = States.Lose;
            SoundManager.Instance.PlayerSource = null;
            Destroy(this.gameObject);
        }
    }

    public void AddArmor(int arm)
    {
        armor += arm;
    }
}
