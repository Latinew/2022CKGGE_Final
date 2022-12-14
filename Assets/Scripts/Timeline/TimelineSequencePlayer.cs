using NaughtyAttributes;
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

    private bool _isTestFinish;

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
        //테스트 플레이가 실행되면 아래 내용 처리 X
        if (_isTestFinish) return;

        TimelineObjects[currentPlaying].stopped -= PlayNextTimeline;
        TimelineObjects[currentPlaying].gameObject.SetActive(false);

        currentPlaying += 1;
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

    #if UNITY_EDITOR
    [Button("테스트 피니쉬")]
    private void TestFinish()
    {
        //테스트 피니쉬를 활성화 합니다.
        _isTestFinish = true;

        //전체 타임라인을 정지합니다. (안멈추잖아~!~!)
        for (int i = 0; i < TimelineObjects.Length; i++)
        {
            TimelineObjects[i].Stop();
            TimelineObjects[i].gameObject.SetActive(false);
        }

        //현재 플레이 시간을 끝으로 처리함.
        currentPlaying = TimelineObjects.Length;
        
        OnSequenceEnd.Invoke();
        IsPlayingSequence = false;
        gameObject.SetActive(false);
    }
    #endif
}
