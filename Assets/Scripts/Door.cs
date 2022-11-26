using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using DG.Tweening;

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
        UIManager.Instance.BlackScreenFade(0.5f, 0, 0.4f);
    }
}
