using ColorRings.Runtime.Board.Entity;
using ColorRings.Runtime.GameMode.AdvancedBoard;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardShapeEndlessManager : BaseBoardShapeManager
{
    [SerializeField] private BoardShape[] boardShapes;
    public static int BoardID { get; private set; }
    
    public override BoardShape GetBoardShape() {
#if UNITY_EDITOR && TEST_SHAPE
        var id = AssetDatabase.LoadAssetAtPath<ShapeIDSetting>
                ("Assets/ColorRings/Editor/New Shape ID Setting.asset").id;
        DevLog.Log("BoardShapeID", id);
        return boardShapes[id % boardShapes.Length];
#endif
        var boardShapeID = WeekEventScheduler.GetWeekSinceStart() % boardShapes.Length;
        if (boardShapeID < 0) boardShapeID = 0;
        BoardID = boardShapeID;
        return boardShapes[boardShapeID];
    }

    public override bool HasLock() => true;
}