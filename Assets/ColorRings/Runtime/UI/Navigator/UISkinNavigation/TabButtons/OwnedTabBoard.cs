using UnityEngine;

public class OwnedTabBoard : MonoBehaviour
{
    private bool firstInit = true;
    
    private void OnDisable()
    {
        if (firstInit)
        {
            firstInit = false;
            return;
        }
        
        SkinStatus.Instance.OnCheckAllOwnedSkin();
    }
}
