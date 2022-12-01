using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.ComponentModel;

public class BaseEnemy : MonoBehaviour, IInteractiveObj
{
    [SerializeField]
    private ParticleSystem particle;
    protected SpriteRenderer spriteRenderer;
    public bool dead;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Interaction()
    {
        Killed();
    }
    protected virtual void Killed()
    {
        if (dead) return;

        dead = true;
        IngameUIManager.Instance.KillEnemy();
        GameObject obj = Instantiate(GameManager.Instance.enemyKilledParticle, transform.position, Quaternion.identity).gameObject;
        obj.SetActive(true);
        Destroy(obj, 10);
        Instantiate(particle, transform.position, Quaternion.identity);
        foreach (GameObject cam in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            cam.GetComponent<Camera>().DOShakePosition(1);
        }
        spriteRenderer.DOFade(0, 1).OnComplete(() => Destroy(gameObject));
    }
}
