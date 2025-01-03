using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // 이동 속도
    public float chaseSpeed = 3f; // 추적 속도
    public int damage = 10; // 플레이어에게 줄 피해
    public float detectionRange = 5f; // 플레이어 추적 거리
    public float returnRange = 8f; // 원래 자리 복귀 거리
    public float roamInterval = 2f; // 로밍 간격
    public float idleChance = 0.3f; // Idle 상태로 전환될 확률 (30%)
    public float idleDurationMin = 1f; // Idle 상태 최소 지속 시간
    public float idleDurationMax = 3f; // Idle 상태 최대 지속 시간

    private Transform player;
    private Vector2 startPosition; // 초기 위치
    private Vector2 roamTarget; // 이동 목표
    private float roamTimer; // 로밍 타이머
    private float idleTimer; // Idle 상태 지속 시간 타이머
    private bool isIdle = false; // Idle 상태 여부

    private enum EnemyState { Roaming, Chasing, Returning }
    private EnemyState currentState = EnemyState.Roaming; // 초기 상태는 Roaming

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다. Player 태그를 확인하세요.");
        }

        startPosition = transform.position; // 초기 위치 저장
        SetNewRoamTarget();
    }

    public virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 상태 전환 로직
        switch (currentState)
        {
            case EnemyState.Roaming:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chasing; // 플레이어 추적 시작
                }
                else
                {
                    HandleRoaming(); // 로밍 동작
                }
                break;

            case EnemyState.Chasing:
                if (distanceToPlayer > returnRange)
                {
                    currentState = EnemyState.Returning; // 원래 자리 복귀
                }
                else
                {
                    ChasePlayer(); // 플레이어 추적
                }
                break;

            case EnemyState.Returning:
                if (Vector2.Distance(transform.position, player.position) <= detectionRange)
                {
                    currentState = EnemyState.Chasing; // 플레이어 감지 시 추적 시작
                }
                else if (Vector2.Distance(transform.position, startPosition) < 0.1f)
                {
                    currentState = EnemyState.Roaming; // 복귀 완료 후 다시 Roaming 상태로 전환
                }
                else
                {
                    ReturnToStart(); // 원래 자리로 복귀
                }
                break;
        }
    }

    private void HandleRoaming()
    {
        // Idle 상태 처리
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false; // Idle 상태 종료
                SetNewRoamTarget(); // 새로운 목표 설정
            }
            return;
        }

        // Roaming 동작
        roamTimer += Time.deltaTime;

        if (roamTimer >= roamInterval)
        {
            // 일정 확률로 Idle 상태로 전환
            if (Random.value < idleChance)
            {
                isIdle = true;
                idleTimer = Random.Range(idleDurationMin, idleDurationMax); // Idle 상태 지속 시간 설정
                return;
            }

            SetNewRoamTarget(); // 새로운 목표 설정
            roamTimer = 0f;
        }

        MoveTowards(roamTarget, speed);

        // 목표 위치에 도달하면 새로운 목표 설정
        if (Vector2.Distance(transform.position, roamTarget) < 0.1f)
        {
            SetNewRoamTarget();
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            MoveTowards(player.position, chaseSpeed);
        }
    }

    private void ReturnToStart()
    {
        MoveTowards(startPosition, speed);
    }

    private void MoveTowards(Vector2 target, float moveSpeed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void SetNewRoamTarget()
    {
        // 상하좌우 중 랜덤 방향 선택
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 randomDirection = directions[Random.Range(0, directions.Length)];

        // 범위 내에서 목표 위치 설정
        roamTarget = (Vector2)transform.position + randomDirection * Random.Range(1f, 3f);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHunger playerHunger = collision.GetComponent<PlayerHunger>();
            if (playerHunger != null)
            {
                playerHunger.DecreaseHunger(damage); // 포만감 감소
                Debug.Log("Enemy collided with player! Damage: " + damage);
            }
        }
    }
}