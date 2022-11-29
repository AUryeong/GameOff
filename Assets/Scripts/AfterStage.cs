using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterStage : MonoBehaviour, IInteractiveObj
{
    public void Interaction()
    {
        GameManager.Instance.AfterStage();
    }
}
