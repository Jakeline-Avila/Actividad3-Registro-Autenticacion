using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float destroyX = -10f;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}