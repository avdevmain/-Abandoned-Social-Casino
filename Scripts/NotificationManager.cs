using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSamples;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    private int notificationDelay;
    [SerializeField] private GameNotificationsManager notificationsManager;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        if (instance == this)
            Destroy(gameObject);
    }

    private void InitializeNotifications()
    {
        GameNotificationChannel channel = new GameNotificationChannel("main_channel", "Main Notifications Channel", "Basic notification");
        notificationsManager.Initialize(channel);
    }


    private void CreateNotification(string title, string body, DateTime time) //Настройки проекта, Project Settings, Mobile Notification Settins - Иконки маленькие и большие
    {
        IGameNotification notification = notificationsManager.CreateNotification();
        if (notification != null)
        {
            notification.Title = title;
            notification.Body = body;
            notification.DeliveryTime = time;
            notificationsManager.ScheduleNotification(notification);
            //Тут же можно указывать идентификаторы для маленьких и больших иконок
        }
    }
}
