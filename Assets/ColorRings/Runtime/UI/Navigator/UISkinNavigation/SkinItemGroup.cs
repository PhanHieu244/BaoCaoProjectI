using JackieSoft;
using UnityEngine;
using UnityEngine.Serialization;

public class SkinItemGroup : MonoBehaviour, Cell.IView
{
    [SerializeField] private SkinItem[] skinItems;
    public const int MaxItemInGroupAmount = 3;

    public void Init(int[] ids, SkinItemsUnity skinItemsUnity)
    {
        var itemAmount = Mathf.Min(ids.Length, MaxItemInGroupAmount);
        int i;
        for (i = 0; i < itemAmount; i++)
        {
            skinItems[i].gameObject.SetActive(true);
            skinItems[i].Init(ids[i], skinItemsUnity);
        }

        for (i = itemAmount; i < MaxItemInGroupAmount; i++)
        {
            skinItems[i].gameObject.SetActive(false);
        }
    }
}

public class SkinItemGroupData : Cell.Data<SkinItemGroup>
{
    public SkinItemsUnity skinItemsUnity;
    public int[] ids;

    protected override void SetUp(SkinItemGroup cellView)
    {
        cellView.Init(ids, skinItemsUnity);
    }
}

