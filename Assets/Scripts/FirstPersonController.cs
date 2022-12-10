using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")] [Tooltip("캐릭터의 이동 속도(ms)")]
        public float MoveSpeed = 4.0f;

        [Tooltip("캐릭터의 질주 속도(ms)")] public float SprintSpeed = 6.0f;
        [Tooltip("캐릭터의 회전 속도")] public float RotationSpeed = 1.0f;
        [Tooltip("가속 및 감속")] public float SpeedChangeRate = 10.0f;

        [Space(10)] [Tooltip("플레이어가 점프할 수 있는 높이")]
        public float JumpHeight = 1.2f;

        [Tooltip("캐릭터는 자체 중력 값을 사용합니다. 엔진 기본값은 -9.81f입니다.")]
        public float Gravity = -15.0f;

        [Space(10)] [Tooltip("다시 점프할 수 있으려면 시간이 필요합니다. 즉시 다시 점프하려면 0f로 설정하십시오.")]
        public float JumpTimeout = 0.1f;

        [Tooltip("가을 상태에 들어가기 전에 경과하는 데 필요한 시간입니다. 계단을 내려갈 때 유용합니다.")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")] [Tooltip("캐릭터가 땅에 닿았는지 여부. 기본 확인에 내장된 CharacterController의 일부가 아닙니다.")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")] [Tooltip("카메라가 따라갈 Cinemachine 가상 카메라에 설정된 추적 대상")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("카메라를 얼마나 위로 올릴 수 있습니까?")] public float TopClamp = 90.0f;
        [Tooltip("카메라를 얼마나 아래로 움직일 수 있습니까?")] public float BottomClamp = -90.0f;

        public Transform BulletSpawnPoint;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

        [Inject] private DiContainer _container;

        private void Awake()
        {
            // 기본 카메라에 대한 참조를 얻습니다.
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            //시작 시 시간 제한 재설정
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            Attack();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // 오프셋으로 구 위치 설정
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.Look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.Look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.Look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.AnalogMovement ? _input.Move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.Move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.Move.x + transform.forward * _input.Move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        
        private void Attack()
        {
            if (_input.Attack)
            {
                #region 날아갈 타겟 지점

                //날아갈 끝 지점
                Vector3 targetPoint = _mainCamera.transform.position + _mainCamera.transform.forward * 10;

                //오른손의 책이 타겟으로 날가가기 위한 방향
                Vector3 targetDirection = (targetPoint - BulletSpawnPoint.position).normalized;

                #endregion

                #region 생성

                GameObject bullet = _container.ResolveId<GameObject>("Bullet");
                var bookBullet = Instantiate(bullet, BulletSpawnPoint.position,Quaternion.identity).GetComponent<BookBullet>();
                bookBullet.Direction = targetDirection;

                #endregion

                _input.Attack = false;
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
    }
}