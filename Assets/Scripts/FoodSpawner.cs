using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public BoxCollider2D gridArea; 

    [Header("Settings")]
    public float moveStep = 0.5f;

    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        if (gridArea == null) return;

        Bounds bounds = this.gridArea.bounds;

        float x = Random.Range(bounds.min.x + 1f, bounds.max.x - 1f);
        float y = Random.Range(bounds.min.y + 1f, bounds.max.y - 1f);

        x = Mathf.Round(x / moveStep) * moveStep;
        y = Mathf.Round(y / moveStep) * moveStep;

        this.transform.position = new Vector3(x, y, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            RandomizePosition();
        }
    }
}