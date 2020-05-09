using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum Events
{
    CLICK,
    MOVE_TO,
    INSPECT,
    INTERACT,
}

public class ClickEventFunction : UnityEvent<Object> { }

public class EventManager : MonoBehaviour
{

    private Dictionary<Events, ClickEventFunction> eventDictionary;
    private static EventManager eventManager;
    public static EventManager eventMan
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogWarning("EventManager is not set on scene");
                }
                else
                {
                    eventManager.init();
                }
            }

            return eventManager;
        }
    }

    void init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<Events, ClickEventFunction>();
        }
    }

    public static void addListener(Events eventName, UnityAction<Object> listener)
    {
        ClickEventFunction theEvent = null;
        if (eventMan.eventDictionary.TryGetValue(eventName, out theEvent))
        {
            theEvent.AddListener(listener);
        }
        else
        {
            theEvent = new ClickEventFunction();
            theEvent.AddListener(listener);
            eventMan.eventDictionary.Add(eventName, theEvent);
        }
    }

    public static void removeListener(Events eventName, UnityAction<Object> listener)
    {
        if (eventManager == null) return;
        ClickEventFunction theEvent = null;
        if (eventMan.eventDictionary.TryGetValue(eventName, out theEvent))
        {
            theEvent.RemoveListener(listener);
        }
    }

    public static void triggerEvent(Events eventName, Object arg = null)
    {
        ClickEventFunction theEvent = null;
        if (eventMan.eventDictionary.TryGetValue(eventName, out theEvent))
        {
            //Debug.Log("EVENT TRIGGER " + eventName.ToString());
            theEvent.Invoke(arg);
        }
    }
}
