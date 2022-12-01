using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Rendering.Universal;

public class ProwlEnemy : TraceEnemy
{
    public Vector2[] prowlPos;
    public bool Loop;
    public float prowlMoveDelay;

    private bool isLooping = false;
    private bool moving = true;
    private int moveIndex = 1;

    private Direction direction;
    private bool isDetectedPlayer;
    private Coroutine prowl;

    protected override void Update()
    {
        base.Update();

        if (!isDetectedPlayer)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, DirectionToVector(direction), 1, LayerMask.GetMask("Player"));
            if (ray.collider != null)
            {
                StopCoroutine(prowl);
                isDetectedPlayer = true;
            }
        }
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
                direction = VectorToDirection(pos);
            }
            else
            {
                moveIndex += isLooping ? -1 : 1;
                continue;
            }

            yield return new WaitForSeconds(prowlMoveDelay);
        }
    }
    private Vector2 DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Vector2.up;
            case Direction.RIGHT:
                return Vector2.right;
            case Direction.DOWN:
                return Vector2.down;
            case Direction.LEFT:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }
    private Direction VectorToDirection(Vector2 vec)
    {
        if (vec.x < 0)
            return Direction.LEFT;
        else if (vec.x > 0)
            return Direction.RIGHT;
        else if (vec.y < 0)
            return Direction.DOWN;
        else if (vec.y > 0)
            return Direction.UP;
        else
            return 0;
    }

    private Vector3 posComparator(Vector3 pos1, Vector3 pos2)
    {
        Vector3 returnVector = Vector3.Normalize(pos1 - pos2) * -1;

        return returnVector;
    }
}
