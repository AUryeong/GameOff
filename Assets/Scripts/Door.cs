using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Door : MonoBehaviour, IInteractiveObj
{
    public Vector3 targetPos;
    public void Interaction()
    {
        PlayerIntDoor();
    }

    public void PlayerIntDoor()
    {
        Player.Instance.transform.position = targetPos;
    }
}
