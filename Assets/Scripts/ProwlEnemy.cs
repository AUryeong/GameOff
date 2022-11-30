using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI.Table;

public class ProwlEnemy : TraceEnemy
{
    public Vector2[] prowlPos;
    public bool Loop;
    public float prowlMoveDelay;

    private bool isLooping = false;
    private bool moving = true;
    private int moveIndex = 1;

    private bool isDetectedPlayer;
    private Coroutine prowl;

    protected override void Update()
    {
        base.Update();
    }
    protected override bool isDetecting()
    {
        return isDetectedPlayer && GameManager.Instance.nowTracingEnemy != this;
    }
    private void Start()
    {
        prowl = StartCoroutine(ProwlCoroutine());
    }
    private IEnumerator ProwlCoroutine()
    {
        while (moving)
        {
            if (moveIndex >= prowlPos.Length || moveIndex < 0)
            {
                if (Loop)
                {
                    moveIndex += isLooping ? 1 : -1;
                    isLooping = !isLooping;
                }
                else
                {
                    moveIndex = 0;
                }
            }

            Vector3 pos = posComparator(transform.position, prowlPos[moveIndex]);
            if (pos != Vector3.zero)
            {
                transform.DOMove(transform.position + pos, prowlMoveDelay).SetEase(Ease.Linear);
            }
            else
            {
                moveIndex += isLooping ? -1 : 1;
                continue;
            }

            yield return new WaitForSeconds(prowlMoveDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            StopCoroutine(prowl);
            isDetectedPlayer = true;
        }
    }

    private Vector3 posComparator(Vector3 pos1, Vector3 pos2)
    {
        Vector3 returnVector = Vector3.Normalize(pos1 - pos2) * -1;

        return returnVector;
    }
}
