using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelineSequencePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector[] TimelineObjects;
    [SerializeField] private bool PlayOnStart = true;
    [SerializeField] private UnityEvent OnSequenceEnd;

    int currentPlaying = 0;
    bool IsPlayingSequence = false;

    private void Start()
    {
        if (PlayOnStart)
        {
            PlaySequence();
        }
    }

    public void PlaySequence()
    {
        if (IsPlayingSequence) return;
        IsPlayingSequence = true;
        TimelineObjects[currentPlaying].gameObject.SetActive(true);
        TimelineObjects[currentPlaying].stopped += PlayNextTimeline;
    }

    private void PlayNextTimeline(PlayableDirector obj)
    {
        TimelineObjects[currentPlaying].stopped -= PlayNextTimeline;
        TimelineObjects[currentPlaying].gameObject.SetActive(false);

        currentPlaying++;
        if (currentPlaying >= TimelineObjects.Length)
        {
            OnSequenceEnd.Invoke();
            IsPlayingSequence = false;
            return;
        }

        TimelineObjects[currentPlaying].gameObject.SetActive(true);
        TimelineObjects[currentPlaying].stopped += PlayNextTimeline;
    }
}
