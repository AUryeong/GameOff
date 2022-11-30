using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public ParticleSystem enemyKilledParticle;
    public int nowStage = 1;
    public int killedEnemyCount = 0;
    [SerializeField] protected Vector3 afterPos;

    public int tracingRoomChangeCount;
    public TraceEnemy nowTracingEnemy
    {
        get { return _nowTracingEnemy; }
        set
        {
            EnemyActive();
            _nowTracingEnemy = value;
        }
    }
    [SerializeField]
    private TraceEnemy _nowTracingEnemy;
    private TraceEnemy[] enemies;
    private float tracingTime;
    private void Start()
    {
        enemies = FindObjectsOfType<TraceEnemy>();

        UIManager.Instance.BlackScreenFade(0.8f, 0, 0.7f);
        if (InGameManager.Instance.clearStage > nowStage)
        {
            foreach (var obj in FindObjectsOfType<BaseEnemy>())
                obj.gameObject.SetActive(false);
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

    public void BeforeStage()
    {
        if (nowStage == 1)
        {
            if (InGameManager.Instance.clearStage >= 5)
            {
                DOTween.KillAll();
                SceneManager.sceneLoaded += DisableEnemy;
                SceneManager.LoadScene("#6");
            }
        }
        else
        {
            if (nowStage == 5)
                if (!IngameUIManager.Instance.isClear)
                    return;
            if (!IngameUIManager.Instance.isClear && IngameUIManager.Instance.killEnemyCount != 0)
                return;
            DOTween.KillAll();
            SceneManager.sceneLoaded += DisableEnemy;
            SceneManager.LoadScene("#" + --nowStage);
        }
    }

    private void DisableEnemy(Scene arg0, LoadSceneMode arg1)
    {
        Player.Instance.isMoving = false;
        Player.Instance.transform.position = afterPos;
        SceneManager.sceneLoaded -= DisableEnemy;
    }

    public void AfterStage()
    {
        if (IngameUIManager.Instance.isClear || InGameManager.Instance.clearStage > nowStage)
        {
            DOTween.KillAll();
            SceneManager.LoadScene("#" + ++nowStage);
            InGameManager.Instance.clearStage = Mathf.Max(InGameManager.Instance.clearStage, nowStage);
        }
    }

}
