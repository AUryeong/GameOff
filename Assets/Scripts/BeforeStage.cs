using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeStage : MonoBehaviour, IInteractiveObj
{
    public void Interaction()
    {
        GameManager.Instance.BeforeStage();
    }
}
