using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private float m_Timer = 0;

    private TriggeredEvent m_CurrentEvent;

    public UnityEngine.Video.VideoPlayer videoPlayer;

    public List<TriggeredEvent> m_Events
        = new List<TriggeredEvent>();

    private Mouledoux.Components.Mediator.Subscriptions sub = new Mouledoux.Components.Mediator.Subscriptions();
    private Mouledoux.Callback.Callback onNotify;

    void Awake()
    {
        m_CurrentEvent = m_Events[0];
        onNotify = TriggerNextEvent;

        sub.Subscribe("nextVideo", onNotify);

    }

    void Update()
    {
        if(!videoPlayer.isPrepared || videoPlayer.isLooping || videoPlayer.isPlaying) return;

        else TriggerNextEvent(new object[]{});
    }


    [ContextMenu("TriggerNext")]
    public void TriggerNextEvent(object [] args)
    {
        Mouledoux.Components.Mediator.instance.NotifySubscribers($"{m_CurrentEvent.EventName}: done");
        int cIndex = m_Events.FindIndex(i => i == m_CurrentEvent);
        TriggerEventAtIndex(cIndex + 1);
        Mouledoux.Components.Mediator.instance.NotifySubscribers($"{m_CurrentEvent.EventName}: start");
    }


    public void TriggerEventAtIndex(int aIndex)
    {
        if (aIndex >= m_Events.Count)
        {
            TriggerEventAtIndex(0);
            return;
        }

        m_CurrentEvent = m_Events[aIndex];  // Set the current event to the new event
        m_CurrentEvent.RunEvent();          // Run the new event
        m_Timer = 0;
    }

    public void OnDestroy()
    {
        sub.UnsubscribeAll();
    }


    [System.Serializable]
    public sealed class TriggeredEvent
    {
        public string EventName;
        public UnityEngine.Events.UnityEvent TriggerEvent;

        public int RunEvent()
        {
            TriggerEvent.Invoke();
            return 0;
        }
    }
}
