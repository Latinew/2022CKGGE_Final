using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputResponser: MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private GameObject FootstepSoundEmitter;

    [SerializeField] private float Stop_Amplitude;
    [SerializeField] private float Stop_Frequency;
    [SerializeField, Space(20f)] private float Move_Amplitude;
    [SerializeField] private float Move_Frequency;

    private PlayerInput _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _input.actions.FindAction("Move").started += OnPlayerMove;
        _input.actions.FindAction("Move").canceled += OnPlayerStop;
    }

    private void OnDestroy()
    {
        _input.actions.FindAction("Move").started -= OnPlayerMove;
        _input.actions.FindAction("Move").canceled -= OnPlayerStop;
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
