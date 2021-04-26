using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : GenericSingleton<SoundManager>
{
    [SerializeField]
    private AudioSource backgroundSource;
    
    private AudioSource playerSource;
    public AudioSource PlayerSource
    {
        get { return playerSource; }
        set { playerSource = value; }
    }
    
    private AudioSource enemySource;
    public AudioSource EnemySource
    {
        get { return enemySource; }
        set { enemySource = value; }
    }

    [SerializeField]
    private AudioClip bgrndMusic;

    [SerializeField]
    private AudioClip playerAttack;

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        backgroundSource.clip = bgrndMusic;
        backgroundSource.loop = true;
        backgroundSource.Play();
    }

    public void PlayPlayerAttackSound()
    {
        if (playerSource != null)
        {
            playerSource.clip = playerAttack;
            playerSource.loop = false;
            playerSource.Play();
        }
    }
}
