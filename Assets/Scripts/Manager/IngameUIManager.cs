using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class IngameUIManager : Singleton<IngameUIManager>
{

    protected int maxEnemyCount;
    protected int killEnemyCount;

    [Header("대화")]
    [SerializeField] TextMeshProUGUI talkText;
    protected Coroutine talkTextCoroutine;
    protected float talkFadeDuration = 0.5f;
    protected float talkTextDuration = 0.1f;

    [Header("게이지")]
    [SerializeField] Image gaugeIcon;
    [SerializeField] Image gaugeBar;
    protected int particleCount = 6;
    protected float particleGaugeDuration = 0.2f;
    public float toParticleGauge = 0;
    protected float toIconScale = 1;

    [SerializeField] RawImage[] effectRawImages;

    [SerializeField] Image rainbowFilter;
    [SerializeField] Gradient rainbow;

    [SerializeField] Image noise;
    protected float noiseCooltime = 5;
    protected float noiseFadeDuration = 0.1f;
    protected float noiseDuration = 0.2f;
    protected float noiseNowDuration = 0;
    protected float noiseAlpha = 1f;

    [SerializeField] Image princessImage;
    protected float stopImageDuration = 2;
    protected float fadeDuration = 1;
    protected void Start()
    {
        killEnemyCount = 0;
        gaugeBar.fillAmount = 0;
        maxEnemyCount = FindObjectsOfType<BaseEnemy>().Length;

        if (effectRawImages != null && effectRawImages.Length > 0)
            for (int i = 0; i < effectRawImages.Length; i++)
                effectRawImages[i].rectTransform.DOShakeAnchorPos(3, 16, 1).SetEase(Ease.Linear).SetLoops(-1);

        if (princessImage != null)
            StartCoroutine(PrincessImageCoroutine());
    }

    IEnumerator PrincessImageCoroutine()
    {
        princessImage.gameObject.SetActive(true);
        InGameManager.Instance.isControllable = false;

        yield return new WaitForSeconds(stopImageDuration);
        while (!Input.anyKeyDown) yield return null;

        princessImage.DOFade(0, fadeDuration).OnComplete(() =>
        {
            princessImage.gameObject.SetActive(false);
            InGameManager.Instance.isControllable = true;
        });
    }

    protected void Update()
    {
        if (rainbowFilter != null)
            FilterUpdate();
        if (noise != null)
            NoiseUpdate();
    }

    protected void NoiseUpdate()
    {
        noiseNowDuration += Time.deltaTime;
        if (noiseNowDuration >= noiseCooltime)
        {
            noiseNowDuration -= noiseCooltime;
            noise.gameObject.SetActive(true);
            noise.color = new Color(1, 1, 1, 0);
            noise.DOFade(noiseAlpha, noiseFadeDuration).OnComplete(() =>
            {
                noise.DOFade(0, noiseFadeDuration).SetDelay(noiseDuration).OnComplete(() =>
                {
                    noise.gameObject.SetActive(false);
                });
            });
        }
        if (noise.gameObject.activeSelf)
            noise.rectTransform.anchoredPosition = new Vector2(Random.Range(-960f, 960f), Random.Range(-960f, 960f));
    }
    protected void FilterUpdate()
    {
        rainbowFilter.color = rainbow.Evaluate(Mathf.Repeat(Time.time * 0.3f, 1f));
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
