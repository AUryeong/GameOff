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
    public int killEnemyCount;
    public bool isClear
    {
        get
        {
            if (maxEnemyCount == 0)
                maxEnemyCount = FindObjectsOfType<BaseEnemy>().Length;
            return maxEnemyCount <= killEnemyCount;
        }
    }

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

    private bool princessKilled;
    private bool videoNoise;
    private bool mumbleNoise;
    private float sirenCooldown = 12;
    private float mumbleCooldown = 12;
    private float videoNoiseCooldown = 20;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
            Destroy(gameObject);
        else if (GameManager.Instance.nowStage == 5)
            DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {

        killEnemyCount = 0;
        gaugeBar.fillAmount = 0;
        if (maxEnemyCount == 0)
            maxEnemyCount = FindObjectsOfType<BaseEnemy>().Length;

        if (effectRawImages != null && effectRawImages.Length > 0)
            for (int i = 0; i < effectRawImages.Length; i++)
                effectRawImages[i].rectTransform.DOShakeAnchorPos(3, 16, 1).SetEase(Ease.Linear).SetLoops(-1);

        if (princessImage != null)
            StartCoroutine(PrincessImageCoroutine());

        talkText.DOFade(0, talkFadeDuration).SetDelay(1)
            .OnComplete(() =>
            {
                talkText.gameObject.SetActive(false);
            });
    }

    IEnumerator PrincessImageCoroutine()
    {
        princessImage.color = Color.black;
        princessImage.DOColor(Color.white, 1);
        princessImage.gameObject.SetActive(true);
        InGameManager.Instance.isControllable = false;
        yield return new WaitForSeconds(stopImageDuration + 1);
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
        if (videoNoise)
        {
            videoNoiseCooldown -= Time.deltaTime;
            if (videoNoiseCooldown <= 0)
            {
                videoNoiseCooldown = Random.Range(8, 22);
                SoundManager.Instance.PlaySoundClip("006_Video_Noise", SoundType.SFX);
            }
        }
        if (mumbleNoise)
        {
            mumbleCooldown -= Time.deltaTime;
            if (mumbleCooldown <= 0)
            {
                mumbleCooldown = Random.Range(8, 22);
                SoundManager.Instance.PlaySoundClip("007_Mumble", SoundType.SFX);
            }
        }
        if (princessKilled)
        {
            sirenCooldown -= Time.deltaTime;
            if (sirenCooldown <= 0)
            {
                sirenCooldown = Random.Range(10, 20);
                SoundManager.Instance.PlaySoundClip("Siren", SoundType.SFX);
            }
        }


    }
    public void KillEnemy()
    {
        killEnemyCount++;
        switch (maxEnemyCount - killEnemyCount)
        {
            case 3:
                ShowText("3.");
                break;
            case 2:
                ShowText("Finally, 2");
                break;
            case 1:
                ShowText("The last one.");
                break;
            case 0:
                ShowText("To the next floor.");
                break;
        }
        if (GameManager.Instance.nowStage == 3) videoNoise = true;
        if (GameManager.Instance.nowStage == 4) mumbleNoise = true;
        if (GameManager.Instance.nowStage == 5)
        {
            SoundManager.Instance.PlaySoundClip("002_Screaming", SoundType.SFX);
            SoundManager.Instance.PlaySoundClip("Siren", SoundType.SFX);
            princessKilled = true;
        }
        else
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    SoundManager.Instance.PlaySoundClip("002_Screaming", SoundType.SFX,0.5f,Random.Range(0.7f,1.3f));
                    break;
                case 1:
                    SoundManager.Instance.PlaySoundClip("002_2_Man_Screaming", SoundType.SFX, 0.5f, Random.Range(0.7f, 1.3f));
                    break;
            }
        }
        switch (Random.Range(0, 2))
        {
            case 0:
                SoundManager.Instance.PlaySoundClip("Splat2", SoundType.SFX, 0.5f, Random.Range(0.7f, 1.3f));
                break;
            case 1:
                SoundManager.Instance.PlaySoundClip("Splat3", SoundType.SFX, 0.5f, Random.Range(0.7f, 1.3f));
                break;
        }
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
