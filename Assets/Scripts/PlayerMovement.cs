using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float minJumpForce = 8f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private Vector2 jumpDirection = new Vector2(0.7f, 1).normalized;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheckPoint;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float maxChargeScale = 1.3f;
    [SerializeField] private float chargeAnimationSpeed = 2f;

    [Header("Touch Settings")]
    [SerializeField] private float maxTapDuration = 0.2f;
    [SerializeField] private float touchRadius = 50f;

    [Header("Jump Power Bar")]
    [SerializeField] private Image jumpPowerBarFill;
    [SerializeField] private Image jumpPowerBarBackground;
    [SerializeField] private CanvasGroup powerBarCanvasGroup;

    private SoundManager soundManager;
    private Rigidbody2D rb;
    private float chargeStartTime;
    private bool isCharging;
    private bool isGrounded;
    private float currentChargeProgress;
    private int touchFingerId = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

        if (powerBarCanvasGroup != null)
        {
            powerBarCanvasGroup.alpha = 0f;
            powerBarCanvasGroup.blocksRaycasts = false;
            powerBarCanvasGroup.interactable = false;
        }
    }

    private void Update()
    {
        CheckGround();
        HandleTouchInput();
        UpdateChargeProgress();
        UpdateAnimation();
        UpdateJumpPowerBar();
    }

    private void UpdateChargeProgress()
    {
        if (isCharging)
        {
            float chargeTime = Time.time - chargeStartTime;
            currentChargeProgress = Mathf.Clamp01(chargeTime / maxChargeTime);
        }
        else
        {
            currentChargeProgress = 0f;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && isGrounded && touchFingerId == -1)
            {
                if (touch.position.y < Screen.height * 0.4f)
                {
                    touchFingerId = touch.fingerId;
                    StartCharging();
                    break;
                }
            }

            if (touch.fingerId == touchFingerId)
            {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    StopCharging();
                    Jump();
                    touchFingerId = -1;
                    break;
                }
            }
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        playerAnimator.SetTrigger("StartCharge");

        if (powerBarCanvasGroup != null)
        {
            powerBarCanvasGroup.alpha = 1f;
        }

        if (jumpPowerBarFill != null)
        {
            jumpPowerBarFill.fillAmount = 0f;
        }
    }

    private void StopCharging()
    {
        isCharging = false;
        playerAnimator.SetTrigger("Jump");

        if (powerBarCanvasGroup != null)
        {
            powerBarCanvasGroup.alpha = 0f;
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime);
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, currentChargeProgress);

        Vector2 force = jumpDirection * jumpForce;
        rb.velocity = force;
        soundManager?.PlayJump();
    }

    private void UpdateAnimation()
    {
        if (isCharging)
        {
            float scale = Mathf.Lerp(1f, maxChargeScale, currentChargeProgress);
            playerAnimator.transform.localScale = new Vector3(scale, scale, 1f);
            playerAnimator.SetFloat("ChargeProgress", currentChargeProgress);
        }
        else if (isGrounded)
        {
            playerAnimator.transform.localScale = Vector3.one;
        }

        playerAnimator.SetBool("IsGrounded", isGrounded);
        playerAnimator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    private void UpdateJumpPowerBar()
    {
        if (jumpPowerBarFill != null)
        {
            jumpPowerBarFill.fillAmount = currentChargeProgress;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}