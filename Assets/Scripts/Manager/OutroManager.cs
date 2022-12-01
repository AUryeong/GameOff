using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutroManager : MonoBehaviour
{
    [SerializeField] Image outroImage;
    private void Start()
    {
        Destroy(Player.Instance.gameObject);
        Destroy(IngameUIManager.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        Destroy(InGameManager.Instance.gameObject);
        StartCoroutine(OutroCoroutine());
    }

    IEnumerator OutroCoroutine()
    {
        outroImage.color = Color.black;
        outroImage.DOColor(Color.white, 1);
        yield return new WaitForSeconds(5);
        outroImage.color = Color.black;
        yield return new WaitForSeconds(1);
        //TODO ÃÑ»ç¿îµå
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Title");
    }
}
