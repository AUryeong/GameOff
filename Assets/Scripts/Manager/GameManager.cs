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

    private void Start()
    {
        UIManager.Instance.BlackScreenFade(0.8f, 0, 0.7f);
        if (InGameManager.Instance.clearStage > nowStage)
        {
            foreach (var obj in FindObjectsOfType<BaseEnemy>())
                obj.gameObject.SetActive(false);
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
