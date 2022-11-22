using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Direction
{
    UP,
    LEFT,
    DOWN,
    RIGHT
}

public class Player : Singleton<Player>
{
    protected bool isMoving = false;
    protected Direction direction;

    protected readonly float moveTileDuration = 0.2f;
    protected void Update()
    {
        Move();
        CheckInt();
    }

    protected void CheckInt()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetDirection(), 1, LayerMask.GetMask("IntObject"));
            if (raycastHit2D.collider != null)
            {
                raycastHit2D.collider.GetComponent<IInteractiveObj>().Interaction();
            }
        }
    }

    protected void Move()
    {
        if (isMoving) return;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction = Direction.UP;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction = Direction.RIGHT;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction = Direction.DOWN;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT;
        }
        else return;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetDirection(), 1, LayerMask.GetMask("Object", "IntObject", "Wall"));
        if (raycastHit2D.collider == null)
        {
            isMoving = true;
            transform.DOLocalMove(GetDirection(), moveTileDuration).SetRelative().OnComplete(() =>
            {
                isMoving = false;
            });
        }
    }

    public Quaternion GetQuaternion()
    {
        return Quaternion.Euler(0, 0, (int)direction * 90);
    }

    public Vector2 GetDirection()
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
}
