using System;
using UnityEngine;

public class PushNotification : MonoBehaviour
{
    private NotificationInformation[] _daysNotificationsInform;
    private INotificationManager _notificationManager;

    private void Awake()
    {
#if UNITY_ANDROID
        _notificationManager = new AndroidNotificationManager();
        _daysNotificationsInform = LastTimeNotification.LastTimeNotificationsInformation;
#endif
    }
    
#if UNITY_ANDROID
    private void Start()
    {
        _notificationManager.ClearAllNotifications();
        _notificationManager.RequestAuthorization();
        _notificationManager.RegisterNotificationChannel();
        SendLastTimeNotification();
    }

    private void SendLastTimeNotification()
    {
        foreach (var notification in _daysNotificationsInform)
        {
            _notificationManager.SendNotification
                (notification.title, notification.text, notification.smallIcon, notification.fireTime);
        }
    }
#endif
}

[Serializable]
public class NotificationInformation
{
    public string title;
    public string text;
    public string smallIcon;
    public float fireTime;
}

public class LastTimeNotification
{
    private const string SMALL_ICON = "small_icon";

    public static readonly NotificationInformation[] LastTimeNotificationsInformation = 
    {
        //day 1
        new()
        {
            text = "It's time to match the rings \u2B55\u2B55\u2B55 and relax!!!",
            smallIcon = SMALL_ICON,
            fireTime = 1f
        },
        //day 3
        new ()
        {
            text = "You are on fire! \U0001F525 " +
                   "You have solved multiple levels in a row. Can you keep the streak going?",
            smallIcon = SMALL_ICON,
            fireTime = 3f
        },
        //day 7
        new ()
        {
            text = "More challenging levels are waiting for you! Come and solve it now!! \U0001F50D",
            smallIcon = SMALL_ICON,
            fireTime = 7f
        },
        //day 15
        new ()
        {
            text = "It's been a while, we miss you! Come back and enjoy some relaxing puzzle with us. \U00002764",
            smallIcon = SMALL_ICON,
            fireTime = 15f
        },
        //day 30
        new ()
        {
            text =
                "You have been away for too long! ☹ Time to training with your brain. " +
                "Log in and sharpen your mind with some fun and challenging puzzles.",
            smallIcon = SMALL_ICON,
            fireTime = 30f
        },
    };
    
}
