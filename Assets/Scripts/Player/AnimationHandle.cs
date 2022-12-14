using UnityEngine;
using UnityEngine.Events;

public class AnimationHandle : MonoBehaviour
{
    public UnityEvent[] events;

    public void OnAnimationHandle(int index)
    {
        events[index]?.Invoke();
    }
}
