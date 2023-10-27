using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float initialMinZoom = 5f; // Minimum zoom level
    public float initialMaxZoom = 10f; // Maximum zoom level
    public float zoomLerpSpeed = 5f; // Speed at which the camera zooms
    public float followSpeed = 5f; // Speed at which the camera follows the players

    private Camera cam;
    private float _minZoom;
    private float _maxZoom;

    public float MinZoom { get { return _minZoom; } }
    public float MaxZoom { get { return _maxZoom; } }

    private void Start()
    {
        _minZoom = initialMinZoom;
        _maxZoom = initialMaxZoom;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (player1 == null || player2 == null) return;

        // Get the midpoint between the two players
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        // Calculate the distance between the two players
        float distance = Vector2.Distance(player1.position, player2.position);

        // Set the camera's position to the midpoint
        transform.position = Vector3.Lerp(transform.position, new Vector3(midpoint.x, midpoint.y, transform.position.z), Time.deltaTime * followSpeed);

        // Adjust the camera's size (orthographic size) based on the distance between the players
        float targetZoom = Mathf.Lerp(_minZoom, _maxZoom, Mathf.InverseLerp(0, 10, distance));
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }
}
