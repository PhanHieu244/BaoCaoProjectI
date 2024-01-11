using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;


public static class DevLog
{
    private const Color ColorLog = Color.GREEN; 
    private const Color ColorWarning = Color.YELLOW; 
    private const Color ColorError = Color.RED; 
    private const string ColorTag = "#35adcf"; 
    
    [Conditional("DEV_LOG")]
    public static void Log(object content)
    {
#if DEV_LOG
        Debug.Log($"<color={ColorLog}>DEVLOG: </color>" + content); 
#endif
    }
    
    [Conditional("DEV_LOG")]
    public static void Log(object tag, object content, float size = 17f, string colorTag = ColorTag)
    {
#if DEV_LOG
         Debug.Log($"<size={size}> <color={ColorLog}>DEVLOG: <color={colorTag}>{tag} </color></color>{content}</size>"); 
#endif
    }
    
    [Conditional("DEV_LOG")]
    public static void Log<T>(T[] contents)
    {
#if DEV_LOG
        StringBuilder logContent = new StringBuilder("[");
        foreach (var content in contents)
        {
            logContent.Append(content).Append(", ");
        }
        logContent.Append("]");
        Debug.Log($"<color={ColorLog}>DEVLOG: </color>" + logContent);
#endif
    }

    [Conditional("DEV_LOG")]
    public static void Log<T>(object text, T[] contents)
    {
#if DEV_LOG
        StringBuilder logContent = new StringBuilder(text + " [");
        foreach (var content in contents)
        {
            logContent.Append(content).Append(", ");
        }
        logContent.Append("]");
        Debug.Log($"<color={ColorLog}>DEVLOG: </color>" + logContent);
#endif
    }
    
    [Conditional("DEV_LOG")]
    public static void LogWarning(object content)
    {
#if DEV_LOG
        Debug.LogWarning($"<color={ColorWarning}>DEVLOG: </color>" + content); 
#endif
    }
    
    [Conditional("DEV_LOG")]
    public static void LogError(object content)
    {
#if DEV_LOG
        Debug.LogError($"<color={ColorError}>DEVLOG: </color>" + content); 
#endif
    }
    
    [Conditional("DEV_LOG")]
    public static void LogError(object tag, object content, float size = 17f, string colorTag = ColorTag)
    {
#if DEV_LOG
        Debug.LogError($"<size={size}> <color={ColorLog}>DEVLOG: <color={colorTag}>{tag} </color></color>{content}</size>"); 
#endif
    }
}