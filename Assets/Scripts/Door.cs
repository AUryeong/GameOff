using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, IInteractiveObj
{
    public Vector3 targetPos;
    public static float coolDown;
    public void Interaction()
    {
        PlayerIntDoor();
    }

    public void PlayerIntDoor()
    {
        if (Player.Instance.isMoving)
        {
            Player.Instance.transform.DOKill();
            Player.Instance.isMoving = false;
        }
        Player.Instance.transform.position = targetPos;
        Invoke(nameof(TracingEnemy), 1);
        UIManager.Instance.BlackScreenFade(0.8f, 0f, 0.7f);
    }

    private void TracingEnemy()
    {
        if (GameManager.Instance.nowTracingEnemy != null)
        {
            GameManager.Instance.nowTracingEnemy.transform.position = targetPos;
            GameManager.Instance.tracingRoomChangeCount++;
        }
    }
}
