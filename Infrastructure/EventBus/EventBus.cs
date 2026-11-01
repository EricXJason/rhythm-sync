using System;
using System.Collections.Generic;

/// <summary>
/// ============================================================================
/// 檔案名稱: EventBus.cs
/// 所屬模組: Infrastructure
/// 責任描述: 全域事件匯流排。使用標準 Dictionary 字典儲存事件與回呼方法，
///           提供訂閱 (Subscribe)、退訂 (Unsubscribe) 與發布 (Publish) 功能，達成跨模組解耦。
/// 架構分層: Infrastructure / EventBus
/// 聯動關係:
///   - 下游消費: 供全專案所有領域 (Gameplay, Player, Audio, UI, Visuals) 進行事件解耦通訊
/// 邊界處理: 透過 ContainsKey 進行安全檢查，使用標準 C# 委派加減 (+/-)
/// ============================================================================
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

    /// <summary>
    /// 訂閱事件：將回呼方法註冊至字典中
    /// </summary>
    public static void Subscribe<T>(Action<T> listener)
    {
        if (listener == null) return;

        Type type = typeof(T);

        if (eventTable.ContainsKey(type))
        {
            eventTable[type] = (Action<T>)eventTable[type] + listener;
        }
        else
        {
            eventTable[type] = listener;
        }
    }

    /// <summary>
    /// 取消訂閱：將回呼方法從字典中移除
    /// </summary>
    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (listener == null) return;

        Type type = typeof(T);

        if (eventTable.ContainsKey(type))
        {
            Action<T> currentDelegate = (Action<T>)eventTable[type] - listener;

            if (currentDelegate == null)
            {
                eventTable.Remove(type);
            }
            else
            {
                eventTable[type] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// 發布廣播：從字典找出該事件的所有監聽者並執行
    /// </summary>
    public static void Publish<T>(T eventData)
    {
        Type type = typeof(T);

        if (eventTable.ContainsKey(type))
        {
            Action<T> callback = (Action<T>)eventTable[type];
            callback?.Invoke(eventData);
        }
    }

    /// <summary>
    /// 清空所有事件監聽者
    /// </summary>
    public static void Clear()
    {
        eventTable.Clear();
    }
}
