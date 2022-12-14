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

        currentPlaying+=1;
        if (currentPlaying >= TimelineObjects.Length)
        {
            Debug.Log("ppap");
            OnSequenceEnd.Invoke();
            IsPlayingSequence = false;
            return;
        }

        TimelineObjects[currentPlaying].gameObject.SetActive(true);
        TimelineObjects[currentPlaying].stopped += PlayNextTimeline;
    }
}
