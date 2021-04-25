using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropCard : MonoBehaviour
{
    [SerializeField]
    private bool selected = false;
    private bool overDropZone = false;
    public bool OverDropZone
    {
        get { return overDropZone; }
        set { overDropZone = value; }
    }
    private Transform initParent;
    private Vector2 initPos;
    private Vector2 targetPos;
    public Vector2 TargetPos
    {
        get { return targetPos; }
        set { targetPos = value; }
    }

    private int onContainer = -1; // will have -1 as value if not on container, else the number of the container;

    private void Start()
    {
        initParent = transform.parent;
        initPos = transform.position;
        targetPos = initPos;
    }

    private void Update()
    {
        if (selected)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }

    public void StartDrag()
    {
        selected = true;
        if (onContainer != -1)
        {
            StateMachine.Instance.CardContainers[onContainer].ChangeColliderStatus(true);
        }
    }

    public void EndDrag()
    {
        //the player is able to drag cards during the enemy turn but it wont go into the containers
        selected = false;
        if (StateMachine.Instance.CurrentState == States.PlayerTurn)
        {
            int i = StateMachine.Instance.DropCardOnContainer(this);
            if (i == -1)
            {
                ResetCardPlacement();
            }
            else
            {
                onContainer = i;
                transform.position = targetPos;
            }
        }
    }

    public void ResetCardPlacement()
    {
        transform.SetParent(initParent, false);
        targetPos = initPos;
        if (onContainer != -1)
        {
            StateMachine.Instance.CardContainers[onContainer].ChangeColliderStatus(true);
            onContainer = -1;
        }
        transform.position = targetPos;
    }

}
