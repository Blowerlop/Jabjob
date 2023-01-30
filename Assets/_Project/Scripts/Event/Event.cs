using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Project
{
    public class Event
    {
        private Action _action = delegate {  };
        private List<Action> _actionsTrackList = new List<Action>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private Dictionary<Action, object> _actionTrackListExpend = new Dictionary<Action, object>();
        #endif
        
        private readonly string _eventName;
        
        public void Invoke(object sender) 
        {
            Debug.Log($"<color=#00FF00>{sender} invoked {_eventName}</color>");
            _action.Invoke();
            
             #if UNITY_EDITOR || DEVELOPMENT_BUILD
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<color=#00FF00>Methods called by {_eventName}:</color> \n");

            foreach (var kvp in _actionTrackListExpend)
            {
                stringBuilder.Append($"<color=#00FF00>- {kvp.Key.Method.Name} --- by {kvp.Value}</color> \n");

            }
            Debug.Log(stringBuilder);
            #endif
            
        }

        public Event(string eventName)
        {
            _eventName = eventName;
        }

        public void Subscribe(Action action, object subcriber)
        {
            if (IsListenerAlreadySubscribe(action))
            {
                Debug.LogError($"Method - {action.Method.Name} - is already registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action += action;
                _actionTrackListExpend.Add(action, subcriber);
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #else
                _action += action;
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #endif
                
                
            }
        }

        public void Unsubscribe(Action action)
        {
            if (IsListenerAlreadySubscribe(action) == false)
            {
                Debug.LogError($"Method - {action.Method.Name} - is not registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action -= action;
                _actionsTrackList.Remove(action);
                _actionTrackListExpend.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #else
                _action -= action;
                _actionsTrackList.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #endif
            }
            
        }

        private bool IsListenerAlreadySubscribe(Action action)
        {
            return _actionsTrackList.Contains(action);
        }
    }
    
    public class Event<T>
    {
        private Action<T> _action = delegate {  };
        private List<Action<T>> _actionsTrackList = new List<Action<T>>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private Dictionary<Action<T>, object> _actionTrackListExpend = new Dictionary<Action<T>, object>();
        #endif
        
        private readonly string _eventName;
        
        public void Invoke(object sender, T @object) 
        {
            Debug.Log($"<color=#00FF00>{sender} invoked {_eventName}</color>");
            _action.Invoke(@object);
            
             #if UNITY_EDITOR || DEVELOPMENT_BUILD
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<color=#00FF00>Methods called by {_eventName}:</color> \n");

            foreach (var kvp in _actionTrackListExpend)
            {
                stringBuilder.Append($"<color=#00FF00>- {kvp.Key.Method.Name} --- by {kvp.Value}</color> \n");

            }
            Debug.Log(stringBuilder);
            #endif
            
        }

        public Event(string eventName)
        {
            _eventName = eventName;
        }

        public void Subscribe(Action<T> action, object subscriber)
        {
            if (IsListenerAlreadySubscribe(action))
            {
                Debug.LogError($"Method - {action.Method.Name} - is already registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action += action;
                _actionTrackListExpend.Add(action, subscriber);
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #else
                _action += action;
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #endif
                
                
            }
        }

        public void Unsubscribe(Action<T> action)
        {
            if (IsListenerAlreadySubscribe(action) == false)
            {
                Debug.LogError($"Method - {action.Method.Name} - is not registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action -= action;
                _actionsTrackList.Remove(action);
                _actionTrackListExpend.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #else
                _action -= action;
                _actionsTrackList.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #endif
            }
            
        }

        private bool IsListenerAlreadySubscribe(Action<T> action)
        {
            return _actionsTrackList.Contains(action);
        }
    }
    
    public class Event<T1, T2>
    {
        private Action<T1, T2> _action = delegate {  };
        private List<Action<T1, T2>> _actionsTrackList = new List<Action<T1, T2>>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private Dictionary<Action<T1, T2>, object> _actionTrackListExpend = new Dictionary<Action<T1, T2>, object>();
        #endif
        
        private readonly string _eventName;
        
        public void Invoke(object sender, T1 object1, T2 object2) 
        {
            Debug.Log($"<color=#00FF00>{sender} invoked {_eventName}</color>");
            _action.Invoke(object1, object2);
            
             #if UNITY_EDITOR || DEVELOPMENT_BUILD
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<color=#00FF00>Methods called by {_eventName}:</color> \n");

            foreach (var kvp in _actionTrackListExpend)
            {
                stringBuilder.Append($"<color=#00FF00>- {kvp.Key.Method.Name} --- by {kvp.Value}</color> \n");

            }
            Debug.Log(stringBuilder);
            #endif
            
        }

        public Event(string eventName)
        {
            _eventName = eventName;
        }

        public void Subscribe(Action<T1, T2> action, object subscriber)
        {
            if (IsListenerAlreadySubscribe(action))
            {
                Debug.LogError($"Method - {action.Method.Name} - is already registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action += action;
                _actionTrackListExpend.Add(action, subscriber);
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #else
                _action += action;
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
                #endif
                
                
            }
        }

        public void Unsubscribe(Action<T1, T2> action)
        {
            if (IsListenerAlreadySubscribe(action) == false)
            {
                Debug.LogError($"Method - {action.Method.Name} - is not registered in the event");
            }
            else
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _action -= action;
                _actionsTrackList.Remove(action);
                _actionTrackListExpend.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #else
                _action -= action;
                _actionsTrackList.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
                #endif
            }
            
        }

        private bool IsListenerAlreadySubscribe(Action<T1, T2> action)
        {
            return _actionsTrackList.Contains(action);
        }
    }
}