using System.Collections.Generic;
using UnityEngine.Purchasing;

public interface ICustomDetailedStoreListener : IDetailedStoreListener
{
    IEnumerable<string> ProductIds { get; }
    ProductType ProductType { get; }
}