using System;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BookEffect : MonoBehaviour
{
    private enum BookState
    {
        None,
        Show,
        Hide
    }

    public float ShowSpeed = 1;
    public float HideSpeed = 1;

    private Material _material;
    private MeshRenderer _renderer;
    private BookState _bookState;

    private float _currentRange;
    private static readonly int RangeID = Shader.PropertyToID("_Range");

    private void Awake()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            _renderer = meshRenderer;
            _material = Instantiate(_renderer.material);

            _renderer.material = _material;
        }
    }

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => _bookState == BookState.Show)
            .Subscribe(_ =>
            {
                float range = Mathf.MoveTowards(_currentRange, 1, ShowSpeed * Time.deltaTime);
                _currentRange = range;
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => _bookState == BookState.Hide)
            .Subscribe(_ =>
            {
                float range = Mathf.MoveTowards(_currentRange, 0, HideSpeed * Time.deltaTime);
                _currentRange = range;
            })
            .AddTo(this);
    }

    private void Update()
    {
        _material.SetFloat(RangeID, _currentRange);
    }

    /// <summary>
    /// 책을 애니메이션 없이 보이게합니다.
    /// </summary>
    public void OnTriggerShowWithoutAnimation()
    {
        _currentRange = 1;
        _bookState = BookState.Show;
        _material.SetFloat(RangeID, _currentRange);
    }

    /// <summary>
    /// 책을 애니메이션 없이 안보이게 합니다.
    /// </summary>
    public void OnTriggerHideWithoutAnimation()
    {
        _currentRange = 0;
        _bookState = BookState.Hide;
        _material.SetFloat(RangeID, _currentRange);
    }
    
    [Button("Show")]
    public void Show()
    {
        _bookState = BookState.Show;
    }

    [Button("Hide")]
    public void Hide()
    {
        _bookState = BookState.Hide;
    }
}