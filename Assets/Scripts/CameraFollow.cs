using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Transform anchor2; 
    public Transform mainStage; 
    
    public float initialMinZoom = 5f; 
    public float initialMaxZoom = 15f; 
    public float zoomLerpSpeed = 5f; 
    public float followSpeed = 5f; 
    public float maxDistanceFromAnchor = 5f; 

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

        // midpoint between the two players
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        // Calculate the distance between the two players
        float distance = Vector2.Distance(player1.position, player2.position);

        // Set the camera's position to the midpoint, clamped to the main stage boundary or anchor points
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(midpoint.x, Mathf.Min(mainStage.position.x, anchor2.position.x) - maxDistanceFromAnchor, Mathf.Max(mainStage.position.x, anchor2.position.x) + maxDistanceFromAnchor), // Adjust the X coordinate within the main stage boundary or anchor points
            Mathf.Clamp(midpoint.y, Mathf.Min(mainStage.position.y, anchor2.position.y) - maxDistanceFromAnchor, Mathf.Max(mainStage.position.y, anchor2.position.y) + maxDistanceFromAnchor), // Adjust the Y coordinate within the main stage boundary or anchor points
            transform.position.z
        );
        transform.position = Vector3.Lerp(transform.position, clampedPosition, Time.deltaTime * followSpeed);

        // Adjust the camera's size (orthographic size) based on the distance between the players
        float targetZoom = Mathf.Lerp(_minZoom, _maxZoom, Mathf.InverseLerp(0, 10, distance));
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }
}
