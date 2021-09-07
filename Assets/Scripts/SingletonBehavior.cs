using UnityEngine;

public abstract class SingletonBehavior<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && !ReferenceEquals(Instance, this))
            Destroy(gameObject);
        else
            Instance = this as T;
    }
}