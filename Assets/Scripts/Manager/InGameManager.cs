using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : SingletonDontDestroy<InGameManager>
{
    public  int clearStage = 1;
    public bool isControllable = true;

    protected void Start()
    {
        clearStage = Mathf.Max(clearStage, GameManager.Instance.nowStage);
    }
}
