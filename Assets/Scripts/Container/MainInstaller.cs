using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public GameObject BookBullet;
    
    public override void InstallBindings()
    {
        Container.Bind<GameObject>().WithId("Bullet").FromInstance(BookBullet);
    }
}