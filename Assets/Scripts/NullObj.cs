using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class NullObj : MonoBehaviour, IInteractiveObj
{
    public void Interaction()
    {
        string text;
        switch (Random.Range(1, 5))
        {
            case 1:
                text = "Empty";
                break;
            case 2:
                text = "It's nothing";
                break;
            case 3:
                text = "Blank";
                break;
            default:
                text = "At the next time";
                break;
        }
        IngameUIManager.Instance.ShowText(text);
    }
}
