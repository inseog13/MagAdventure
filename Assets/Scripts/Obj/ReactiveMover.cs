using UnityEngine;
using System.Collections;

public class ReactiveMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 targetOffset = new Vector2(3f, 0f); // 시작 위치 기준 상대 거리
    public float moveSpeed = 2f;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip pingSound;

    [Header("Obstacle Settings")]
    public LayerMask obstacleLayer;
    public float detectionRadius = 0.2f;
    public float pauseDuration = 1f;

    private Vector2 startPosition;
    private float t = 0f;
    private bool movingToTarget = true;
    private bool isPaused = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isPaused) return;

        // 장애물 감지
        if (IsObstacleInPath())
        {
            ReverseDirection();
            StartCoroutine(PauseMovement());
            return;
        }

        // 이동 처리
        t += Time.deltaTime * moveSpeed * (movingToTarget ? 1 : -1);
        t = Mathf.Clamp01(t);

        Vector2 targetPosition = startPosition + targetOffset;
        transform.position = Vector2.Lerp(startPosition, targetPosition, t);

        // 끝 도달 시 반전
        if ((movingToTarget && t >= 1f) || (!movingToTarget && t <= 0f))
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
    {
        movingToTarget = !movingToTarget;
        PlaySound();
    }

    private IEnumerator PauseMovement()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }

    private void PlaySound()
    {
        if (audioSource != null && pingSound != null)
        {
            audioSource.PlayOneShot(pingSound);
        }
    }

    private bool IsObstacleInPath()
    {
        Vector2 currentPos = transform.position;
        Vector2 destination = movingToTarget ? (startPosition + targetOffset) : startPosition;
        Vector2 direction = (destination - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, destination);

        RaycastHit2D hit = Physics2D.CircleCast(currentPos, detectionRadius, direction, distance, obstacleLayer);
        return hit.collider != null;
    }
}
