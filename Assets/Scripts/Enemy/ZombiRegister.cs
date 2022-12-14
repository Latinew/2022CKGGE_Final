using AutoManager;
using Manager;
using NaughtyAttributes;
using UnityEngine;

public class ZombiRegister : MonoBehaviour
{
    [SerializeField] private Zombie[] zombies;

    void Start()
    {
        var zombieCount = zombies.Length;
        AutoManager.Manager.Get<GameManager>().ZombieCount= zombieCount;    
    }
}

