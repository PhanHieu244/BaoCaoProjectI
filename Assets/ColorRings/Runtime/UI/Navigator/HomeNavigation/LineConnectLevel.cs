using JackieSoft;
using UnityEngine;

public class LineConnectLevel : MonoBehaviour, Cell.IView
{

}

public class LineConnectData : Cell.Data<LineConnectLevel>
{
    protected override void SetUp(LineConnectLevel cellView)
    {
    }
}