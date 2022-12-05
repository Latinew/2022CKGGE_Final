using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmInertia : MonoBehaviour
{
    [SerializeField] private float mult;
    private Vector3 prevDirection;

    private void Start()
    {
        prevDirection = Camera.main.transform.forward;
    }

    private void Update()
    {
        Vector3 subst = (prevDirection - Camera.main.transform.forward)*mult;
       // transform.forward = Camera.main.transform.forward + subst;

        prevDirection = Camera.main.transform.forward;
    }

}
