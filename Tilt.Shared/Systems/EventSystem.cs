using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Systems
{
    public interface IEventChanged
    {
        void OnEventChanged(object sender, IGameEventArgs e);

        void RegisterEvent();

        void UnRegisterEvent();
    }


    public enum EventType
    {
        PauseChanged,
        PauseButtonPressed,
        ReloadLevel,
        MapChanged,
        TowerAdded,
        TowerRemoved,
        SelectionModeChanged,
        SellObject,
        UnitDestroyed,
        GameOver,
        BaseDamage,
        LevelRecap,
        StartMenu,
        MapScrolled,
        SelectedTileChanged,
        TowerSelected,
        TowerDeselected,
        TowerChanged,
        TowerDestroyed,
        SoundEffect,
        MusicChanged,
        MuteSFX,
        MuteMusic,
        SlidingPanelClose,
        NotificationWindowOpened,
        NotificationWindowClosed,
        PausePanelOpen,
        PausePanelClose,
        InfoPanelOpen,
        InfoPanelClose,
        LevelComplete,
        MineralsChanged
    }

    public interface IGameEventArgs
    {
        EventType EventType { get; }
    }

    public class MapChangedArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.MapChanged; } }
    }

    public class LevelRecapArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.LevelRecap; } }

        public int TowerRefund { get; set; }

        public int UnitsDestroyedOverLevel { get; set; }

    }

    public class TowerSelectedArgs : IGameEventArgs
    {
        public EventType EventType { get {return EventType.TowerSelected; } }

        public Entity Object { get; set; }
    }

    public class SoundEffectArgs : IGameEventArgs
    {
        private float mVolume = 1.0f;

        public EventType EventType { get { return EventType.SoundEffect; } }

        public EventType TriggeredEventType { get; set; }

        public string SoundEffect { get; set; }

        public bool IsLooping { get; set; }

        public bool Play { get; set; }

        public bool Pause { get; set; }

        public bool Stop { get; set; }

        public bool Resume { get; set; }
        
        public ulong Id { get; set; }
        
        public float Volume
        {
            get { return mVolume; }
            set { mVolume = value; }
        }
    }

    public class MusicChangedArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.MusicChanged; } }

        public EventType TriggeredEventType { get; set; }

        public string SongName { get; set; }

        public bool IsLooping { get; set; }

        public bool Play { get; set; }

        public bool Pause { get; set; }

        public bool Stop { get; set; }

    }

    public class MuteSFXArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.MuteSFX; } }

        public bool Mute { get; set; }
    }

    public class MuteMusicArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.MuteMusic; } }

        public bool Mute { get; set; }
    }

    public class NotificationArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.NotificationWindowOpened; } }

        public string Text { get; set; }
    }

    public class UnitDestroyedArgs : IGameEventArgs
    {
        public EventType EventType { get { return EventType.UnitDestroyed; } }

        public Entity DestroyingEntity { get; set; }
    }

    public class MineralsChangedArgs  : IGameEventArgs
    {
        public EventType EventType { get { return EventType.MineralsChanged; } }

        public uint OldValue { get; set; }

        public uint NewValue { get; set; }
    }

    /// We use this component when we want to wire up to events
    /// but we dont want the events to continue being listened for
    /// when we remove the layer from the LayerManager.
    /// This component will dispose of events once the EventSystem.Clear is called
    public class EventComponent : Component
    {
        public EventComponent(Entity owner, bool register = true) : base(owner, register)
        {
        }

        public override void Register()
        {
            EventSystem.Register(this);
        }

        public override void UnRegister()
        {
            EventSystem.UnRegister(this);
        }
    }


    /*
     * The EventSystem is responsible for handling all events at the end of a frame.
     * To Subscribe to an event, we use the public SubScribe method and set the EventType to listen for.
     * To Queue an event to be fired at the end of frame, we use the EnqueueEvent method, which
     * then gets fired during the UpdateMethod.
     */
    public static class EventSystem
    {
        public delegate void CallbackMethod(object sender, IGameEventArgs e);
        private static Dictionary<EventType, List<CallbackMethod>> mSubscribers = new Dictionary<EventType, List<CallbackMethod>>();
        private static Dictionary<EventType, List<Tuple<object, IGameEventArgs>>> mEventParameters = new Dictionary<EventType, List<Tuple<object, IGameEventArgs>>>();
        private static Queue<EventType> mEvents = new Queue<EventType>();
        private static List<EventComponent> mComponents = new List<EventComponent>();

        public static void Register(EventComponent eventComponent)
        {
            mComponents.Add(eventComponent);
        }

        public static void UnRegister(EventComponent eventComponent)
        {
            mComponents.Remove(eventComponent);
        }

        public static void Clear()
        {
            foreach (EventComponent component in mComponents.ToList())
            {
                component.UnRegister();
            }

            mComponents.Clear();
        }

        public static void SubScribe(EventType eventType, CallbackMethod method)
        {
            if (!mSubscribers.ContainsKey(eventType))
            {
                
                List<CallbackMethod> methods = new List<CallbackMethod>();
                methods.Add(method);
                mSubscribers[eventType] = methods; 
                return;
            }

            List<CallbackMethod> events = mSubscribers[eventType];
            events.Add(method);

        }

        public static void UnSubScribe(EventType eventType, CallbackMethod method)
        {
            if (mSubscribers.ContainsKey(eventType))
            {
                List<CallbackMethod> methods = mSubscribers[eventType];
                if (methods == null)
                    return;

                methods.Remove(method);

            }
        }

        public static void EnqueueEvent(EventType eventType, object sender = null, IGameEventArgs e = null)
        {
            mEvents.Enqueue(eventType);

            if (!mEventParameters.ContainsKey(eventType))
            {
                List<Tuple<object, IGameEventArgs>> eventParams = new List<Tuple<object, IGameEventArgs>>();
                eventParams.Add(new Tuple<object, IGameEventArgs>(sender, e));

                mEventParameters.Add(eventType, eventParams);
            }
            else
            {
                List<Tuple<object, IGameEventArgs>> eventParams = null;
                mEventParameters.TryGetValue(eventType, out eventParams);

                eventParams.Add(new Tuple<object, IGameEventArgs>(sender, e));
            }
        }

        private static void Notify_(EventType eventType)
        {
            if (!mSubscribers.ContainsKey(eventType))
                return;

            List<CallbackMethod> events = mSubscribers[eventType];
            if (events == null)
                return;

            List<Tuple<object, IGameEventArgs>> par = null;
            mEventParameters.TryGetValue(eventType, out par);

            if (par == null)
                return;

            foreach (Tuple<object, IGameEventArgs> tuple in par)
            {
                foreach (CallbackMethod method in events.ToList())
                {
                    method(tuple.Item1, tuple.Item2);
                }
            }
        }

        public static void Update()
        {
            while (mEvents.Count > 0)
            {
                EventType eventType = mEvents.Dequeue();
                Notify_(eventType);
                mEventParameters.Remove(eventType);
            }
        }
    }
}
