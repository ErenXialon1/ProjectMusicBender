using DG.Tweening;

using System.Collections.Generic;
using UnityEngine;

public class RuneRock : MonoBehaviour
{
    [Header("Engraving Settings")]
    [Tooltip("The LineRenderer used to display the engraving.")]
    public LineRenderer lineRenderer;

    [Tooltip("The spacing between input points.")]
    public float inputSpacing = 1f;

    [Tooltip("Particle effect for the writing animation.")]
    public GameObject writingEffect;
    [Header("Starting Circle Prefab")]
    
    [SerializeField]private List<Vector3> engravePoints = new List<Vector3>(); // Stores the points for the engraving
    [Tooltip("Circle for the starting point of the engraving.")]
    public RectTransform startPointCircle;
    private Vector3 lastPosition; // Last position used for engraving
    public float engravingForce = 2; // It allows to control velocity over time value

    [Tooltip("Indicates if the RuneRock is currently being engraved.")]
    public bool isEngraving = false;
    private void OnEnable()
    {
        // Reset engraving when the RuneRock is reused from the pool
        ResetRuneRock();
    }
    
    /// <summary>
    /// Resets the RuneRock, clearing all engravings and resetting variables.
    /// </summary>
    public void ResetRuneRock()
    {
        engravePoints.Clear();
        lineRenderer.positionCount = 0;
        transform.position = Vector3.zero;
        lastPosition = transform.position; // Reset to initial position
        isEngraving = false;
        writingEffect.SetActive(false);
        startPointCircle.gameObject.SetActive(false);

    }
    /// <summary>
    /// Adds a new engrave point and draws it with animation.
    /// </summary>
    /// <param name="direction">Direction of the input.</param>
    /// <param name="duration">Duration of the engraving animation.</param>
    public void AddEngravePoint(Vector3 direction, float duration)
    {
        // Calculate the new engraving point
        Vector3 directionVector = GetDirectionVector(direction);
        Vector3 newPosition = lastPosition + directionVector * inputSpacing;

        // Add the new point to the engrave list
        engravePoints.Add(newPosition);
        // Place a starting point circle if this is the first input
        if (engravePoints.Count == 1)
        {
            PlaceStartPointCircle(lastPosition);
        }
        // Draw the engraving with animation
        DrawEngravingWithDOTween(lastPosition, newPosition, duration, writingEffect);

        // Update the last position
        lastPosition = newPosition;
    }
    /// <summary>
    /// Places a circle at the starting point of the engraving.
    /// </summary>
    /// <param name="position">The position of the starting point.</param>
    private void PlaceStartPointCircle(Vector3 position)
    {
        startPointCircle.gameObject.SetActive(true);
        startPointCircle.position = position; // Position it at the start point
    }
    /// <summary>
    /// Draws an engraving between two points with animation and effects.
    /// </summary>
    /// <param name="start">Start position of the engraving.</param>
    /// <param name="end">End position of the engraving.</param>
    /// <param name="duration">Duration of the animation.</param>
    /// <param name="particleEffect">Particle effect to move along the engraving.</param>
    private void DrawEngravingWithDOTween(Vector3 start, Vector3 end, float duration, GameObject particleEffect)
    {
        
          
        
        // Activate particle effect
        particleEffect.SetActive(true);
        particleEffect.transform.position = start;
        // Access the Particle System
        ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("Particle System not found on the particleEffect GameObject.");
            return;
        }
        // Add a new point to the LineRenderer but set it initially at the start position
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, start);
        
        // Calculate the engraving direction
        Vector3 engravingDirection = (end - start).normalized;

        // Configure Particle System's Velocity over Lifetime
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;

        // Set the velocity direction dynamically
        velocityOverLifetime.x = engravingDirection.x * engravingForce; // Adjust multiplier for speed
        velocityOverLifetime.y = engravingDirection.y * engravingForce; // Adjust multiplier for speed
        velocityOverLifetime.z = 0; // No Z movement for 2D
        // Animate the particle effect and line drawing
        float elapsedTime = 0f;

        DOTween.To(
            () => elapsedTime,
            t =>
            {
                elapsedTime = t;

                // Interpolate the current position for the drawing animation
                Vector3 currentPosition = Vector3.Lerp(start, end, elapsedTime / duration);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);

                // Move the particle effect along with the line
                particleEffect.transform.position = currentPosition;
            },
            duration,
            duration
        ).SetEase(Ease.Linear).OnComplete(() =>
        {
            // Deactivate the particle effect once the animation is complete
            particleEffect.SetActive(false);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, end); // Ensure the line ends exactly at the target
        });
    }
    /// <summary>
    /// Converts an input direction into a Vector3 direction.
    /// </summary>
    /// <param name="inputDirection">Input direction as a string (e.g., "w", "a", "s", "d").</param>
    /// <returns>Vector3 direction vector.</returns>
    private Vector3 GetDirectionVector(Vector3 inputDirection)
    {
        // Example input direction mapping
        if (inputDirection == Vector3.up)
            return Vector3.up;
        if (inputDirection == Vector3.down)
            return Vector3.down;
        if (inputDirection == Vector3.left)
            return Vector3.left;
        if (inputDirection == Vector3.right)
            return Vector3.right;

        Debug.LogWarning($"Unrecognized input direction: {inputDirection}");
        return Vector3.zero;
    }
    /// <summary>
    /// Returns the current engraving as a combination string.
    /// </summary>
    public string GetEngravingCombination()
    {
        return string.Join("-", engravePoints); // Customize formatting as needed
    }
}
