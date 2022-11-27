using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProwlEnemy : BaseEnemy
{
    public Vector2[] targetPos;
    public bool Loop;
    public float moveDelay;

    private bool isLooping = false;
    private bool moving = true;
    private int moveIndex;
    private void Start()
    {
        StartCoroutine(ProwlCoroutine());
    }
    private IEnumerator ProwlCoroutine()
    {
        while (moving)
        {
            if (moveIndex > targetPos.Length || moveIndex < 0)
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

            Vector3 pos = posComparator(transform.position, targetPos[moveIndex]);
            if (pos != Vector3.zero)
            {
                transform.Translate(pos);
            }
            else
            {
                moveIndex += isLooping ? -1 : 1;
                continue;
            }


            yield return new WaitForSeconds(moveDelay);
        }
    }

    private Vector3 posComparator(Vector3 pos1, Vector3 pos2)
    {

        Vector3 returnVector = Vector3.Normalize(pos1 - pos2);

        return returnVector;
    }
    protected override void Killed()
    {
        base.Killed();
        moving = false;
    }
}
