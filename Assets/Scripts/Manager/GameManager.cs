using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public ParticleSystem enemyKilledParticle;
    public int nowStage = 1;
    [SerializeField] protected Vector3 afterPos;

    public int tracingRoomChangeCount;
    public TraceEnemy nowTracingEnemy
    {
        get { return _nowTracingEnemy; }
        set
        {
            _nowTracingEnemy = value;
            EnemyActive();
        }
    }
    [SerializeField]
    private TraceEnemy _nowTracingEnemy;
    private TraceEnemy[] enemies;
    private float tracingTime;
    [SerializeField]
    private Sprite[] killBodySprites;
    private void Start()
    {
        enemies = FindObjectsOfType<TraceEnemy>();

        UIManager.Instance.BlackScreenFade(0.8f, 0, 0.7f);
        Debug.Log((InGameManager.Instance.clearStage == 5) + " , " + (IngameUIManager.Instance.isClear));
        if (InGameManager.Instance.clearStage > nowStage || ((InGameManager.Instance.clearStage == 5) && IngameUIManager.Instance.isClear))
        {
            foreach (var obj in FindObjectsOfType<BaseEnemy>())
            {
                if (InGameManager.Instance.clearStage >= 5 && nowStage != 5)
                {
                    obj.GetComponent<SpriteRenderer>().sprite = killBodySprites[Random.Range(0, killBodySprites.Length)];
                    obj.dead = true;
                }
                else
                    obj.gameObject.SetActive(false);

            }
            foreach (var obj in FindObjectsOfType<TraceEnemy>())
            {
                obj.gameObject.SetActive(false);
            }
        }
    }
    private void Update()
    {
        TracingEnemyFunc();
    }
    private void EnemyActive()
    {
        if (nowTracingEnemy == null)
        {
            foreach (var obj in enemies)
            {
                obj.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var obj in enemies)
            {
                if (obj != nowTracingEnemy)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
    private void TracingEnemyFunc()
    {
        if (nowTracingEnemy != null)
        {
            if (tracingTime >= 65 || tracingRoomChangeCount >= 10)
            {
                tracingTime = 0;
                tracingRoomChangeCount = 0;


                Destroy(nowTracingEnemy.gameObject);
                nowTracingEnemy = null;
            }
            tracingTime += Time.deltaTime;
        }
    }

    public void GameOver()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("GameOver");
    }

    public void BeforeStage()
    {
        if (nowStage == 1)
        {
            if (InGameManager.Instance.clearStage >= 5)
            {
                SceneManager.sceneLoaded += DisableEnemy;
                GotoStage(6);
            }
        }
        else
        {
            if (nowStage == 5)
                if (!IngameUIManager.Instance.isClear)
                    return;
            if (!IngameUIManager.Instance.isClear && IngameUIManager.Instance.killEnemyCount != 0)
                return;
            SceneManager.sceneLoaded += DisableEnemy;
            GotoStage(--nowStage);
        }
    }

    protected void DisableEnemy(Scene arg0, LoadSceneMode arg1)
    {
        Player.Instance.isMoving = false;
        Player.Instance.transform.position = afterPos;
        SceneManager.sceneLoaded -= DisableEnemy;
    }

    protected void GotoStage(int stage)
    {
        DOTween.KillAll();
        SceneManager.LoadScene("#" + stage);
    }

    public void AfterStage()
    {
        if (IngameUIManager.Instance.isClear || InGameManager.Instance.clearStage > nowStage)
        {
            GotoStage(++nowStage);
            InGameManager.Instance.clearStage = Mathf.Max(InGameManager.Instance.clearStage, nowStage);
        }
    }

}
