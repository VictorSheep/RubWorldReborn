using DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
	[Dependency(false)]
    [CreateAssetMenu(menuName = "GameEvent", order = -100)]
    public class GameEvent : ScriptableObject
    {
	    private UnityAction _unityAction;

	    public void AddEventListener(UnityAction unityAction)
	    {
		    _unityAction += unityAction;
	    }

        public void RemoveEventListener(UnityAction unityAction)
        {
	        _unityAction -= unityAction;
        }

        public void Trigger()
        {
	        if (_unityAction != null)
				_unityAction.Invoke();
        }
    }

    public class GameEvent<T1> : ScriptableObject
    {
	    private UnityAction<T1> _unityAction;

	    public void AddEventListener(UnityAction<T1> unityAction)
	    {
		    _unityAction += unityAction;
	    }

	    public void RemoveEventListener(UnityAction<T1> unityAction)
	    {
		    _unityAction -= unityAction;
	    }

	    public void Trigger(T1 arg1)
	    {
		    if (_unityAction != null)
			    _unityAction.Invoke(arg1);
	    }
    }

    public class GameEvent<T1, T2> : ScriptableObject
    {
	    private UnityAction<T1, T2> _unityAction;

	    public void AddEventListener(UnityAction<T1, T2> unityAction)
	    {
		    _unityAction += unityAction;
	    }

	    public void RemoveEventListener(UnityAction<T1, T2> unityAction)
	    {
		    _unityAction -= unityAction;
	    }

	    public void Trigger(T1 arg1, T2 arg2)
	    {
		    if (_unityAction != null)
			    _unityAction.Invoke(arg1, arg2);
	    }
    }
	
	public class GameEvent<T1,T2,T3> : ScriptableObject
	{
		private UnityAction<T1,T2,T3> _unityAction;

		public void AddEventListener(UnityAction<T1,T2,T3> action)
		{
			_unityAction += action;
		}

		public void RemoveEventListener(UnityAction<T1,T2,T3> action)
		{
			_unityAction -= action;
		}

		public virtual void Trigger(T1 arg1, T2 arg2, T3 arg3)
		{
			if (_unityAction != null)
				_unityAction.Invoke(arg1, arg2, arg3);
		}
	}

	public class GameEvent<T1,T2,T3, T4> : ScriptableObject
	{
		private UnityAction<T1,T2,T3, T4> _unityAction;

		public void AddEventListener(UnityAction<T1,T2,T3, T4> action)
		{
			_unityAction += action;
		}

		public void RemoveEventListener(UnityAction<T1,T2,T3, T4> action)
		{
			_unityAction -= action;
		}

		public virtual void Trigger(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (_unityAction != null)
				_unityAction.Invoke(arg1, arg2, arg3, arg4);
		}
	}
}