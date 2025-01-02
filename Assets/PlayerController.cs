using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f; // 기본 이동 속도
    public float dashSpeed = 10f; // 대쉬 속도
    public float dashDuration = 0.2f; // 대쉬 지속 시간
    public float dashCooldown = 2f; // 대쉬 쿨타임
    public bool canDash = true; // 대쉬 가능 여부

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing = false;

    private PlayerHunger playerHunger; // 포만감 관리 스크립트 참조
    private Animator animator; // Animator 참조

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHunger = GetComponent<PlayerHunger>();
        animator = GetComponent<Animator>();

        if (playerHunger == null)
        {
            Debug.LogError("PlayerHunger 스크립트가 Player 오브젝트에 연결되지 않았습니다.");
        }
    }

    void Update()
    {
        // 이동 입력
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 대각선 입력 처리
        if (horizontal != 0 && vertical != 0)
        {
            // 둘 다 입력된 경우, 하나만 유지
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                vertical = 0; // 좌우 우선
            }
            else
            {
                horizontal = 0; // 상하 우선
            }
        }

        moveInput = new Vector2(horizontal, vertical);

        // 애니메이션 상태 업데이트
        animator.SetBool("isMoving", moveInput != Vector2.zero && !isDashing);

        // 대쉬 입력
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing && playerHunger != null)
        {
            // 포만감 확인 후 대쉬 가능 여부 결정
            if (playerHunger.GetCurrentHunger() >= 10)
            {
                StartCoroutine(Dash());
                playerHunger.DecreaseHunger(10f); // 대쉬 시 포만감 10 감소
            }
            else
            {
                Debug.Log("포만감이 부족하여 대쉬할 수 없습니다.");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.velocity = moveInput * moveSpeed;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        // 대쉬 애니메이션 활성화
        animator.SetBool("isDashing", true);

        rb.velocity = moveInput * dashSpeed;

        yield return new WaitForSeconds(dashDuration); // 대쉬 지속 시간

        isDashing = false;
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashCooldown); // 대쉬 쿨타임
        canDash = true;
    }


    public void Death()
    {
        // 죽음 애니메이션 실행
        animator.SetBool("isDead", true);

        // 캐릭터 제어 중지
        rb.velocity = Vector2.zero;
        enabled = false; // PlayerController 스크립트 비활성화
    }
}