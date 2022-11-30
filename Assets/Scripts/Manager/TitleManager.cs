using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Image blackImage;
    [SerializeField] Image[] titles;

    private void Start()
    {
        blackImage.DOFade(0, 1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(titles[0].rectTransform.DORotate(new Vector3(0, 0, 25), 0.75f).SetEase(Ease.InSine));
        sequence.Append(titles[0].rectTransform.DORotate(new Vector3(0, 0, -25), 0.75f).SetEase(Ease.InSine));
        sequence.SetLoops(-1);

        sequence = DOTween.Sequence();
        sequence.Append(titles[1].rectTransform.DORotate(new Vector3(0, 0, -35), 0.34f).SetEase(Ease.InSine));
        sequence.Append(titles[1].rectTransform.DORotate(new Vector3(0, 0, 35), 0.34f).SetEase(Ease.InSine));
        sequence.SetLoops(-1);

        sequence = DOTween.Sequence();
        sequence.Append(titles[2].rectTransform.DORotate(new Vector3(0, 0, 12), 0.85f).SetEase(Ease.InSine));
        sequence.Append(titles[2].rectTransform.DORotate(new Vector3(0, 0, -12), 0.85f).SetEase(Ease.InSine));
        sequence.SetLoops(-1);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("#1");
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
