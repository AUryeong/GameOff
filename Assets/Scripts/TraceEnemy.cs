using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using System;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class TraceEnemy : MonoBehaviour
{
    public float moveDelay;

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public Coroutine Coroutine;
    public bool TraceStart;
    protected Direction direction;

    private bool canReTrace;
    private IEnumerator traceStart;
    private IEnumerator trace;
    protected SpriteRenderer SpriteRenderer;
    private Animator animator;

    private int sizeX, sizeY;
    private Node[,] NodeArray;
    private Node StartNode, TargetNode, CurNode;
    private List<Node> OpenList, ClosedList;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update()
    {
        if (!TraceStart && isDetecting())
        {
            TraceStart = true;
            GameManager.Instance.nowTracingEnemy = this;
            StartCoroutine(TracingStart());
        }
        if (direction == Direction.RIGHT)
        {
            SpriteRenderer.flipX = true;
        }
        if (direction == Direction.LEFT)
        {
            SpriteRenderer.flipX = false;
        }

        animator.SetBool("trace", TraceStart);
    }

    protected virtual bool isDetecting()
    {
        return GameManager.Instance.nowTracingEnemy != this && Vector2.Distance(Player.Instance.transform.position, transform.position) <= 2f;
    }

    public IEnumerator TracingStart()
    {
        Vector3 playerPos = Player.Instance.transform.position;
        targetPos = new Vector2Int((int)(playerPos.x), (int)(playerPos.y));
        startPos = new Vector2Int((int)(transform.position.x), (int)(transform.position.y));

        PathFinding();

        canReTrace = false;
        Coroutine = StartCoroutine(Tracing());
        yield return new WaitForSeconds(1);
        canReTrace = true;
    }
    private IEnumerator Tracing()
    {
        foreach (Node node in FinalNodeList)
        {
            Vector2 vec = new Vector2(node.x, node.y);
            if ((Vector2)transform.position == vec) continue;

            direction = VectorToDirection(vec - (Vector2)transform.position);
            transform.DOMove(vec, moveDelay).SetEase(Ease.Linear);
            yield return new WaitForSeconds(moveDelay);

            if (canReTrace) break;
        }

        while (Vector3.Distance(transform.position, Player.Instance.transform.position) < 1f) yield return null;
        Coroutine = StartCoroutine(TracingStart());
    }

    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.1f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")
                        || col.gameObject.layer == LayerMask.NameToLayer("Object")
                       || col.gameObject.layer == LayerMask.NameToLayer("IntObject")) isWall = true;

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }


        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0)
        {

            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F < CurNode.F || OpenList[i].F == CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                return;
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }


    void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }
    protected Vector2 DirectionToVector(Direction direction)
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
    protected Direction VectorToDirection(Vector2 vec)
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

}