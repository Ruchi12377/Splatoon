using UnityEngine;

public class Paint : MonoBehaviour
{
    public Color color { private get; set; }
    public float Size { private get; set; }

    private Rigidbody rb = default;
    private float time = 0;
    private bool isChanged = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 20 + Vector3.down * Random.Range(0, 10), ForceMode.Impulse);//初速
        Destroy(gameObject, 3);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 0.5f && !isChanged)
        {
            Collider[] colider = GetComponents<Collider>();
            if (colider.Length > 0)
            {
                foreach (Collider col in colider)
                {
                    col.isTrigger = false;
                }
                isChanged = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ColliderEnter();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ColliderEnter();
    }

    private void ColliderEnter()
    {
        Vector3 _direction = 5 * (rb.velocity + transform.forward) / 2;
        Vector3 direction = normal(_direction);

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            if (!hit.transform.CompareTag("Player") && hit.transform.name.IndexOf("(Clone)") != 0)
            {
                PaintObject hitObj = hit.transform.gameObject.GetComponent<PaintObject>();
                if (hitObj != null)
                {
                    bool state = hitObj.Draw(hit.textureCoord, color, Size);
                    if (state)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    private Vector3 normal(Vector3 vec)
    {
        int x = vec.x > 0 ? 1 : -1;
        int y = vec.y > 0 ? 1 : -1;
        int z = vec.z > 0 ? 1 : -1;
        return new Vector3(x, y, z);
    }
}