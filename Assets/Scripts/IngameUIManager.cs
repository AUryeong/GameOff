using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameUIManager : Singleton<IngameUIManager>
{

    protected int maxEnemyCount;
    protected int killEnemyCount;
    [SerializeField] TextMeshProUGUI talkText;
    protected Coroutine talkTextCoroutine;
    protected float talkFadeDuration = 0.5f;
    protected float talkTextDuration = 0.1f;

    [SerializeField] Image gaugeIcon;
    [SerializeField] Image gaugeBar;
    protected int particleCount = 6;
    protected float particleGaugeDuration = 0.2f;
    protected float toParticleGauge = 0;
    protected float toIconScale = 1;

    [SerializeField] RawImage[] effectRawImages;
    Vector2[] rawImagePos;

    protected void Start()
    {
        killEnemyCount = 0;
        gaugeBar.fillAmount = 0;
        //maxEnemyCount = FindObjectsOfType<BaseEnemy>().Length;
        maxEnemyCount = 3;

        rawImagePos = new Vector2[effectRawImages.Length];
        for(int i = 0; i < effectRawImages.Length; i++)
            rawImagePos[i] = effectRawImages[i].rectTransform.anchoredPosition;


        for (int i = 0; i < effectRawImages.Length; i++)
        {
            effectRawImages[i].rectTransform.DOShakeAnchorPos(99, 10, 1).SetLoops(-1);
        }
    }

    protected void Update()
    {
        RawImageShake();
    }

    protected void RawImageShake()
    {

    }

    public void GetGaugeParticle()
    {
        gaugeIcon.rectTransform.DOKill();
        gaugeIcon.rectTransform.DOScale(0.15f, 0.3f).SetEase(Ease.OutBounce).SetRelative().OnComplete(() =>
        {
            gaugeIcon.rectTransform.DOScale(1, 0.4f).SetEase(Ease.OutBounce);
        });

        toParticleGauge += 1f / maxEnemyCount / particleCount;
        gaugeBar.DOKill();
        gaugeBar.DOFillAmount(toParticleGauge, particleGaugeDuration);
    }
    public void ShowText(string s)
    {
        talkText.DOKill();

        talkText.gameObject.SetActive(true);
        talkText.color = new Color(talkText.color.r, talkText.color.g, talkText.color.b, 0);
        talkText.DOFade(1, talkFadeDuration);
        if (talkTextCoroutine != null)
            StopCoroutine(talkTextCoroutine);
        talkTextCoroutine = StartCoroutine(ShowTextCoroutine(s));
    }
    IEnumerator ShowTextCoroutine(string s)
    {
        talkText.text = s;
        var wait = new WaitForSeconds(talkTextDuration);
        int showIdx = 0;
        talkText.maxVisibleCharacters = 0;
        while (talkText.maxVisibleCharacters < s.Length)
        {
            talkText.maxVisibleCharacters = ++showIdx;
            yield return wait;
        }
        talkText.DOFade(0, talkFadeDuration).SetDelay(1)
            .OnComplete(() =>
            {
                talkText.gameObject.SetActive(false);
            });
    }
}
