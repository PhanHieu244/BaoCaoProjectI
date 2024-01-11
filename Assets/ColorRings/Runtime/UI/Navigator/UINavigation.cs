using DG.Tweening;
using Puzzle.UI;
using System.Collections;
using UnityEngine;

public class UINavigation : MonoBehaviour, IPopUpContent
{
    [SerializeField] Navigator navigator;

    public IEnumerator Hide()
    {
        yield return Hub.Hide(gameObject);

        var elements = GetComponentsInChildren<INavigatorElement>();
        foreach (var element in elements)
        {
            var content = element.GetContent();
            Destroy(content);
        }
    }

    public IEnumerator Show()
    {
        var elements = GetComponentsInChildren<INavigatorElement>();
        foreach( var element in elements)
        {
            var content = element.GetContent();
            content.SetActive(true);
            yield return null;
            yield return null;
            content.SetActive(false);
        }

        yield return Hub.Show(gameObject).WaitForCompletion();
        navigator.ShowDefaultContent();
    }
}
