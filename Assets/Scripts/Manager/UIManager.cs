using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class UIManager : Singleton<UIManager>
{
    public Image forFadeBlackScreen;
    protected Canvas canvas;
    public void BlackScreenFade(float startAlpha, float endAlpha, float time, bool isLoop = true)
    {
        forFadeBlackScreen.color = new Color(0, 0, 0, startAlpha);

        if (isLoop)
        {
            forFadeBlackScreen.DOFade(endAlpha, time / 2).OnComplete(() =>
            {
                forFadeBlackScreen.DOFade(0, time / 2);
            });
        }
        else
        {
            forFadeBlackScreen.DOFade(endAlpha, time);

        }
    }
}
