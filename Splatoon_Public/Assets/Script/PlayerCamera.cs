using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform Target { set; private get; }

    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] LayerMask DetectLayer = 0;
    [SerializeField] private float RotateSpeed = 10.0f;
    [SerializeField] private float distance = 10.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private Vector3 lookAt = Vector3.zero;

    private const float YAngle_MIN = -45;
    private const float YAngle_MAX = 45;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        currentX += Input.GetAxis("Mouse X") * RotateSpeed;
        currentY += Input.GetAxis("Mouse Y") * RotateSpeed;
        currentY = Mathf.Clamp(currentY, YAngle_MIN, YAngle_MAX);
    }

    void LateUpdate()
    {
        if (Target != null)
        {
            lookAt = Target.position + offset;
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);

            transform.position = lookAt + rotation * dir;
            transform.LookAt(lookAt);

            //減り込み判定
            if (Physics.Linecast(lookAt, transform.position, out RaycastHit hit, DetectLayer))
            {
                transform.position = hit.point;
            }
        }

    }
}
