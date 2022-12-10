using System;
using NaughtyAttributes;
using UnityEngine;

public class BookBullet : MonoBehaviour
{
    [ReadOnly] public Vector3 Direction = Vector3.forward;
    public Transform GFXTransform;

    [Header("생명 주기")]
    public float LifeTime = 3;
    
    [Header("이동 속도")]
    public float MoveSpeed = 1;
    
    [Header("회전 속도")]
    public float RotateSpeed = 1;
    
    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void Update()
    {
        transform.Translate(Direction * (MoveSpeed * Time.deltaTime));
        GFXTransform.Rotate(Vector3.forward * (RotateSpeed * 100 * Time.deltaTime));
    }
}