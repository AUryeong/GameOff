using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.ComponentModel;

public class BaseEnemy : MonoBehaviour, IInteractiveObj
{
    private SpriteRenderer spriteRenderer;
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
        Instantiate(GameManager.Instance.enemyKilledParticle, transform.position, Quaternion.identity);
        spriteRenderer.DOFade(0, 1).OnComplete(() => Destroy(gameObject));

    }
}
