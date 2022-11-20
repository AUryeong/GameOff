using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : Singleton<IngameUIManager>
{
    [SerializeField] Image talkImage;
    [SerializeField] Text talkText;

    protected float talkFadeDuration = 0.5f;
    protected float talkTextDuration = 0.1f;
    public void ShowText(string s)
    {
        talkImage.DOKill();
        talkText.DOKill();

        talkImage.gameObject.SetActive(true);
        talkImage.color = new Color(talkImage.color.r, talkImage.color.g, talkImage.color.b, 0);
        talkImage.DOFade(1, talkFadeDuration);

        talkText.color = Color.white;
        talkText.text = "";
        talkText.DOText(s, talkTextDuration * s.Length)
        .OnComplete(() =>
        {
            talkText.DOFade(0, talkFadeDuration).SetDelay(1);
            talkImage.DOFade(0, talkFadeDuration).SetDelay(1)
            .OnComplete(() => talkImage.gameObject.SetActive(false));
        });
    }
}
