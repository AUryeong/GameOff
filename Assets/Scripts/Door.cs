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
        UIManager.Instance.BlackScreenFade(0.8f, 0f, 0.7f);
        if (GameManager.Instance.nowTracingEnemy != null)
        {
            GameManager.Instance.tracingRoomChangeCount++;
            GameManager.Instance.nowTracingEnemy.StopCoroutine(GameManager.Instance.nowTracingEnemy.Coroutine);
            Invoke(nameof(TracingEnemy), 1);
        }
    }

    private void TracingEnemy()
    {
        TraceEnemy traceEnemy = GameManager.Instance.nowTracingEnemy;

        traceEnemy.DOKill();
        traceEnemy.transform.position = targetPos;
        traceEnemy.TraceStart = true;
        traceEnemy.StartCoroutine(traceEnemy.TracingStart());
    }
}
