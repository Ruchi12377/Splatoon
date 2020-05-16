using UnityEngine;

public class Paint : MonoBehaviour
{
    public Color color = default;
    public float Size = 0;
    private Rigidbody rb = default;

    private float time = 0;
    private bool isChanged = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 20 + Vector3.down * Random.Range(0, 10), ForceMode.Impulse);
        Destroy(gameObject, 3);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 0.5f && !isChanged)
        {
            var colider = GetComponents<Collider>();
            if (colider.Length > 0)
            {
                foreach (var col in colider)
                {
                    col.isTrigger = false;
                }
                isChanged = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        ColliderEnter(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        ColliderEnter(collision);
    }

    private void ColliderEnter<T>(T component)
    {
        var direction = normal(5 * (rb.velocity + transform.forward) / 2);

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            if (hit.transform.tag != "Player" && hit.transform.name.IndexOf("(Clone)") != 0)
            {
                var hitObj = hit.transform.gameObject.GetComponent<PaintObject>();
                if (hitObj != null)
                {
                    var state = hitObj.Draw(hit.textureCoord, color, Size);
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
        var x = vec.x > 0 ? 1 : -1;
        var y = vec.y > 0 ? 1 : -1;
        var z = vec.z > 0 ? 1 : -1;
        return new Vector3(x, y, z);
    }
}