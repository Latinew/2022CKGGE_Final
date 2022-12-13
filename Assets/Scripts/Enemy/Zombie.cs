using System;
using Manager;
using NaughtyAttributes;
using StarterAssets;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Zenject;

public class Zombie : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private Vector3 _destination;
    private Transform _player;
    private CapsuleCollider _collider;

    [ReadOnly] public IntReactiveProperty HP = new(1);
    public float DestroyDelay = 1;
    public UnityEvent OnDead;

    [Inject(Id = "Bomb")] private GameObject _bombEffect;

    private static readonly int MoveID = Animator.StringToHash("Move");
    private static readonly int OnDeadID = Animator.StringToHash("OnDead");

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        FirstPersonController playerController = FindObjectOfType<FirstPersonController>();

        if (playerController)
            _player = playerController.transform;
    }

    private void Start()
    {
        HP.Where(value => value == 0)
            .Subscribe(_ =>
            {
                OnDead?.Invoke();
                AutoManager.Manager.Get<GameManager>().ZombieCount -= 1;
                _animator.SetTrigger(OnDeadID);
                _collider.enabled = false;

                Vector3 position = transform.position;
                position.y += 1.58f;
                Instantiate(_bombEffect, position, Quaternion.identity);

                Destroy(gameObject, DestroyDelay);
            }).AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => HP.Value > 0).Subscribe(_ =>
            {
                _animator.SetFloat(MoveID, 1);
                //플레이어가 좀비를 바라보는 방향
                Vector3 zombiePosition = transform.position;
                Vector3 directionToPlayer = (zombiePosition - _player.position).normalized;

                Vector3 newPosition = zombiePosition + directionToPlayer;
                _agent.SetDestination(newPosition);
            })
            .AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shootable"))
        {
            HP.Value -= 1;
            Destroy(other.gameObject);
        }
    }
}