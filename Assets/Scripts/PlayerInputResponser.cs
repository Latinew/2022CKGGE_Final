using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FMODUnityResonance;
using FMOD;
using FMODUnity;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputResponser: MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private GameObject FootstepSoundEmitter;

    [SerializeField] private float Stop_Amplitude;
    [SerializeField] private float Stop_Frequency;
    [SerializeField, Space(20f)] private float Move_Amplitude;
    [SerializeField] private float Move_Frequency;

    PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        input.actions.FindAction("Move").started += OnPlayerMove;
        input.actions.FindAction("Move").canceled += OnPlayerStop;
    }

    private void OnDestroy()
    {
        input.actions.FindAction("Move").started -= OnPlayerMove;
        input.actions.FindAction("Move").canceled -= OnPlayerStop;
    }

    public void OnPlayerMove(InputAction.CallbackContext cont)
    {
        FootstepSoundEmitter.SetActive(true);

        if (vCamera == null) return;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Move_Amplitude;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Move_Frequency;
    }

    public void OnPlayerStop(InputAction.CallbackContext cont)
    {
        FootstepSoundEmitter.SetActive(false);

        if (vCamera == null) return;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Stop_Amplitude;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Stop_Frequency;
    }
}
