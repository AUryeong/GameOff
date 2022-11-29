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
    }

    public void BeforeStage()
    {
        DOTween.KillAll();
        SceneManager.sceneLoaded += DisableEnemy;
        SceneManager.LoadScene("#" + --nowStage);
    }

    private void DisableEnemy(Scene arg0, LoadSceneMode arg1)
    {
        foreach (var obj in FindObjectsOfType<BaseEnemy>())
            obj.gameObject.SetActive(false);

        Player.Instance.transform.position = Instance.afterPos;
        SceneManager.sceneLoaded -= DisableEnemy;
    }

    public void AfterStage()
    {
        if (IngameUIManager.Instance.toParticleGauge >= 1 || InGameManager.Instance.clearStage > nowStage)
        {
            DOTween.KillAll();
            SceneManager.LoadScene("#" + ++nowStage);
            InGameManager.Instance.clearStage = Mathf.Max(InGameManager.Instance.clearStage, nowStage);
        }
    }

}
