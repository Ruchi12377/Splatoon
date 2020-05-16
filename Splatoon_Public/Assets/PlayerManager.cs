using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10;
    [SerializeField]
    private Color color = default;
    [SerializeField, Range(1,7)]
    private int Amount = 1;
    [SerializeField]
    private GameObject Ball = default;
    [SerializeField]
    private Transform point = default;
    [SerializeField, Range(1, 50)]
    private int Size = 10;

    [SerializeField]
    private GameObject panel = null;
    [SerializeField]
    private Text text = null;

    private Vector3 moveDirection = Vector2.zero;
    private Vector3 targetDirection = Vector3.zero;
    private float horizontal = 0;
    private float vartical = 0;
    private Rigidbody rb = null;
    private List<PaintObject> paintObjects = new List<PaintObject>();
    private int AllPixcelCount = 0;
    private int PaintPixcelCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        panel.SetActive(false);
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            // シーン上に存在するオブジェクトならば処理.
            if (obj.activeInHierarchy && obj.GetComponent<PaintObject>() != null)
            {
                paintObjects.Add(obj.GetComponent<PaintObject>());
            }
        }
    }

    async void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < Amount; i++)
            {
                var Shake = transform.right * Random.Range(-1, 1) + transform.up * Random.Range(-1, 1) * 2;
                var ball = Instantiate(Ball, point.position + Shake, Camera.main.transform.rotation);
                var paint = ball.GetComponent<Paint>();
                ball.GetComponent<MeshRenderer>().material.color = color;
                paint.color = color;
                paint.Size = Size * Random.Range(0.5f, 1f);
                await Task.Delay(Random.Range(10, 100));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                return;
            }

            panel.SetActive(false);
            AllPixcelCount = 0;
            PaintPixcelCount = 0;
            foreach (var paint in paintObjects)
            {
                var buffer = paint.GetSubTexColor();
                AllPixcelCount += buffer.Length;
                foreach(var col in buffer)
                {
                    if(col == color)
                    {
                        PaintPixcelCount += 1;
                    }
                }
            }
            float par = (float)PaintPixcelCount / (float)AllPixcelCount * 100f;
            text.text = par.ToString();
        }
    }

    private void FixedUpdate()
    {
        PlayerRotation();
        PlayerMove();
    }

    private void PlayerMove()
    {
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right;
        horizontal = Input.GetAxis("Horizontal");
        vartical = Input.GetAxis("Vertical");
        targetDirection = horizontal * right + vartical * forward;
        moveDirection = targetDirection * Speed;
        rb.AddForce(moveDirection);
    }

    private void PlayerRotation()
    {
        Vector3 rotateDirection = moveDirection;
        rotateDirection.y = 0;

        if (rotateDirection.sqrMagnitude > 0.01)
        {
            float step = 15 * Time.deltaTime;
            Vector3 newDir = Vector3.Slerp(transform.forward, rotateDirection, step);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
}
