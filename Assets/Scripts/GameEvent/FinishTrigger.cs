using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 0.5f;
    [SerializeField] private LayerMask playerLayer;

    private SoundManager soundManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!IsPlayerGroundedOnFinish(collision.transform))
        {
            return;
        }

        CompleteLevel();
    }

    private bool IsPlayerGroundedOnFinish(Transform player)
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null) return false;

        Vector2 rayOrigin = player.position;
        rayOrigin.y -= playerCollider.bounds.extents.y;
        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            raycastDistance,
            ~playerLayer);

        Debug.DrawRay(rayOrigin, Vector2.down * raycastDistance, Color.red, 1f);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    private void CompleteLevel()
    {
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null || levelManager.winCanvas == null) return;

        levelManager.PlayFinishParticles();

        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        soundManager?.PlayWin();

        if (levelManager.IsLastLevel)
        {
            levelManager.ShowFinalWinScreen();
        }
        else
        {
            levelManager.ShowWinScreen();
        }

        var winScreen = levelManager.winCanvas.GetComponent<PauseScreenController>();
        winScreen?.Open();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 boxSize = new Vector3(GetComponent<Collider2D>().bounds.size.x, 0.1f, 0);
        Gizmos.DrawWireCube(transform.position + Vector3.down * raycastDistance * 0.5f, boxSize);
    }
}