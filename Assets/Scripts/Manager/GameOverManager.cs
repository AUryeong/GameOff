using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void Retry()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("#" + InGameManager.Instance.clearStage);
    }
    public void Back()
    {
        Destroy(InGameManager.Instance.gameObject);
        SceneManager.LoadScene("Title");
    }
}
