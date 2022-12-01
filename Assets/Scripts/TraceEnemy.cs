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

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
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
        return GameManager.Instance.nowTracingEnemy != this && Vector2.Distance(Player.Instance.transform.position, transform.position) <= 1.5f;
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
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
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


        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0)
        {

            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F < CurNode.F || OpenList[i].F == CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // ������
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

            // �� �� �� ��
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
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