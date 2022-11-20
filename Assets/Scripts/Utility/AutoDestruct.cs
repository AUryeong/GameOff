using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
    public float duration = 1;
    private float durationTime = 0;

    public static void Add(GameObject obj, float duration = 1)
    {
        AutoDestruct autoDestruct = obj.GetComponent<AutoDestruct>();
        if (autoDestruct == null)
            autoDestruct = obj.AddComponent<AutoDestruct>();
        autoDestruct.duration = duration;
    }

    protected void OnEnable()
    {
        durationTime = duration;
    }

    protected void Update()
    {
        durationTime -= Time.deltaTime;
        if (durationTime < 0)
            gameObject.SetActive(false);
    }
}
