using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public GameObject BookBullet;
    public GameObject Bomb;

    public override void InstallBindings()
    {
        Container.Bind<GameObject>().WithId("Bullet").FromInstance(BookBullet);
        Container.Bind<GameObject>().WithId("Bomb").FromInstance(Bomb);
    }
}