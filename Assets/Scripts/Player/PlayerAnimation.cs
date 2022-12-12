using System;
using System.Collections;
using StarterAssets;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        public Animator PlayerAnimator;
        public BookEffect Book;
        private ObservableStateMachineTrigger _stateMachineTrigger;

        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int OnAttack = Animator.StringToHash("OnAttack");
        private FirstPersonController _firstPersonController;

        private void Awake()
        {
            _firstPersonController = GetComponent<FirstPersonController>();
        }

        private void Start()
        {
            _stateMachineTrigger = PlayerAnimator.GetBehaviour<ObservableStateMachineTrigger>();

            _stateMachineTrigger.OnStateEnterAsObservable()
                .Where(info => info.StateInfo.IsName("attack03"))
                .Subscribe(_ =>
                {
                    _firstPersonController.ShotBook();
                    Book.OnTriggerHideWithoutAnimation();

                    StartCoroutine(ShowBookTask());
                })
                .AddTo(this);

            _firstPersonController.PlayOnAwakeHasBook
                .Where(value => value)
                .Subscribe(_ => Book.OnTriggerShowWithoutAnimation())
                .AddTo(this);
        }

        private IEnumerator ShowBookTask()
        {
            yield return new WaitForSeconds(_firstPersonController.BookShowDelay);
            _firstPersonController.PlayerState = FirstPersonController.EPlayerState.Nothing;
            Book.Show();
        }

        public void OnTriggerAttack()
        {
            PlayerAnimator.SetTrigger(OnAttack);
        }

        public void SetMoveAnimation(float value)
        {
            PlayerAnimator.SetFloat(Move, value);
        }
    }
}