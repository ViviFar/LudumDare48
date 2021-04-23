using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Play,
    Pause,
    Win
}


public class StateMachine : MonoBehaviour
{
    private static StateMachine instance;
    public static StateMachine Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<StateMachine>();
                if (!instance)
                {
                    GameObject obj = new GameObject();
                    obj.name = "StateMachine";
                    instance = obj.AddComponent<StateMachine>();
                }
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
    }
}
