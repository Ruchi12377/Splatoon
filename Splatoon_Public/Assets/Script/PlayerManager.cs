using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10;
    [SerializeField]
    private Color color = Color.white;
    [SerializeField, Range(1,7)]
    private int Amount = 1;
    [SerializeField]
    private GameObject Ball = null;
    [SerializeField]
    private Transform point = default;//発射する場所
    [SerializeField, Range(1, 50)]
    private int Size = 10;
    [SerializeField]
    private GameObject panel = null;
    [SerializeField]
    private Text text = null;

    private Vector3 moveDirection = Vector2.zero;
    private Rigidbody rb = null;
    private float horizontal = 0;
    private float vartical = 0;
    private List<PaintObject> paintObjects = new List<PaintObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        panel.SetActive(false);
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.activeInHierarchy && obj.GetComponent<PaintObject>() != null)
            {
                paintObjects.Add(obj.GetComponent<PaintObject>());//ぬれるオブジェクトを全部取得
            }
        }
    }

    async private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < Amount; i++)//Amountの数 玉を生成
            {
                Vector3 right = transform.right * Random.Range(-1, 1);
                Vector3 up = transform.up * Random.Range(-1, 1) * 2;
                Vector3 Shake = right + up;//揺らぎ
                GameObject ball = Instantiate(Ball, point.position + Shake, Camera.main.transform.rotation);
                Paint paint = ball.GetComponent<Paint>();
                ball.GetComponent<MeshRenderer>().material.color = color;
                paint.color = color;//色を指定
                paint.Size = Size * Random.Range(0.5f, 1f);//大きさのゆらぎ
                await Task.Delay(Random.Range(10, 100));//少し時間をずらす
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
            float AllPixcelCount = 0;
            float PaintPixcelCount = 0;
            foreach (PaintObject paint in paintObjects)
            {
                Color[] buffer = paint.GetSubTexColor();
                AllPixcelCount += buffer.Length;//全体のピクセル
                foreach(Color col in buffer)
                {
                    if(col == color)
                    {
                        PaintPixcelCount += 1;//塗られているピクセル
                    }
                }
            }
            float par = PaintPixcelCount / AllPixcelCount * 100f;//パーセンテージに変換
            text.text = par.ToString();
        }

        horizontal = Input.GetAxis("Horizontal");
        vartical = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        PlayerMove();
        PlayerRotation();
    }

    private void PlayerMove()
    {
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right;
        Vector3 targetDirection = horizontal * right + vartical * forward;//進む方向の決定
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
