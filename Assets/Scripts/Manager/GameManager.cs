using AutoManager;
using NaughtyAttributes;

namespace Manager
{
    [ManagerDefaultPrefab("GameManager")]
    public class GameManager : AutoManager.Manager
    {
        [ReadOnly]
        public int ZombieCount;

        private void Awake()
        {
            Zombie[] Zombies = FindObjectsOfType<Zombie>();
            ZombieCount = Zombies.Length;
        }
    }
}