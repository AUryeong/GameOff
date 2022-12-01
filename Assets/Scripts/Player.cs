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
    public bool isMoving = false;
    protected Direction direction;

    protected readonly float moveTileDuration = 0.2f;

    private float intCooldown = 1;
    private float talkNoMoveDuration = 0;
    private float talkMoveDuraiton = 0;
    SpriteRenderer spriteRenderer;
    Animator animator;
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if (Instance != this)
        {
            Instance.transform.position = transform.position;
            Destroy(gameObject);
        }
        else if (GameManager.Instance.nowStage == 5)
            DontDestroyOnLoad(gameObject);
    }
    protected void Update()
    {
        if (!InGameManager.Instance.isControllable) return;

        Move();
        CheckInt();
        CheckTalk();
        intCooldown -= Time.deltaTime;
    }

    protected void CheckTalk()
    {
        if (!isMoving)
            talkNoMoveDuration += Time.deltaTime;
        if (talkNoMoveDuration >= 15)
        {
            talkNoMoveDuration -= 15;
            IngameUIManager.Instance.ShowText("For princess.");
        }
        talkMoveDuraiton += Time.deltaTime;
        if (talkMoveDuraiton >= 60)
        {
            talkMoveDuraiton -= 60;
            switch (Random.Range(1, 4))
            {
                case 1:
                    IngameUIManager.Instance.ShowText("The time has come.");
                    break;
                case 2:
                    IngameUIManager.Instance.ShowText("Though I be damned");
                    break;
                case 3:
                    IngameUIManager.Instance.ShowText("Let the hunt begin.");
                    break;
            }
        }
    }

    protected void CheckInt()
    {
        if (Input.GetKeyDown(KeyCode.Space) && intCooldown <= 0)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetDirection(), 1, LayerMask.GetMask("IntObject"));
            if (raycastHit2D.collider != null)
            {
                raycastHit2D.collider.GetComponent<IInteractiveObj>().Interaction();
                intCooldown = 1;
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
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction = Direction.DOWN;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT;
            spriteRenderer.flipX = false;
        }
        else
        {
            if (animator.GetBool("isWalking"))
                animator.SetBool("isWalking", false);
            return;
        }

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetDirection(), 1, LayerMask.GetMask("Object", "IntObject", "Wall"));
        if (raycastHit2D.collider == null)
        {
            talkNoMoveDuration = 0;
            isMoving = true;
            animator.SetBool("isWalking", true);
            transform.DOLocalMove(GetDirection(), moveTileDuration).SetRelative().OnComplete(() =>
            {
                isMoving = false;
            }).SetEase(Ease.Linear);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.GetComponent<TraceEnemy>() != null)
        {
            SoundManager.Instance.PlaySoundClip("002_2_Man_Screaming", SoundType.SFX, 0.5f, Random.Range(0.7f, 1.3f));
            GameManager.Instance.GameOver();
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
