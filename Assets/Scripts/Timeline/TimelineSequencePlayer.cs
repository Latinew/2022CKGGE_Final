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
        //�׽�Ʈ �÷��̰� ����Ǹ� �Ʒ� ���� ó�� X
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
    [Button("�׽�Ʈ �ǴϽ�")]
    private void TestFinish()
    {
        //�׽�Ʈ �ǴϽ��� Ȱ��ȭ �մϴ�.
        _isTestFinish = true;

        //��ü Ÿ�Ӷ����� �����մϴ�. (�ȸ����ݾ�~!~!)
        for (int i = 0; i < TimelineObjects.Length; i++)
        {
            TimelineObjects[i].Stop();
            TimelineObjects[i].gameObject.SetActive(false);
        }

        //���� �÷��� �ð��� ������ ó����.
        currentPlaying = TimelineObjects.Length;
        
        OnSequenceEnd.Invoke();
        IsPlayingSequence = false;
        gameObject.SetActive(false);
    }
    #endif
}
