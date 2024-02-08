using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    private Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody _rigidbody;

    public static PlayerController instance; // 플레이어 싱글톤

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FixedUpdate() // 물리적인 처리의 작업들 많이함 
    {
        Move();
    }

    private void LateUpdate() // 모든게 끝나고 동작, 보통카메라 작업시 많이 사용 
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    private void Move() // curMovementInput 을 처리 
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; // y값을 없애는데 velocity의 y값을 가져온다

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // 마우스가 위아래로 움직이면서 카메라 x 값 변동
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context) // Inputaction 안의 콜백함수 
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // 이동처리
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if( context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    // 점프
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (IsGrounded()) // 점프시 땅을 밟고있을때만 하게끔함 , 개인적으로 없는게 더 재밌음 
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse); // impulse 는 질량을 갖고 처리한다 
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        { // 캐릭터 머리를 위에서 본거 기준으로 앞 뒤 좌 우 충돌을 체크한다 
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++) // 배열나오면 반복문, 배열과같은 선형구조는 length, 리스트는 count 
        {
            if(Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false; // 하나라도 땅에 닿으면 true 아니면 false 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
