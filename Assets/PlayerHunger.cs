using UnityEngine;
using UnityEngine.UI;

public class PlayerHunger : MonoBehaviour
{
    public Slider hungerBar;
    public float maxHunger = 100f;
    public float hungerDecreaseRate = 1f;

    public float baseSpeed = 5f; // 기본 이동 속도
    public float speedPenaltyPerLevel = 1f; // 포만감 단계별 속도 감소량
    public Sprite[] playerSprites; // 살찐 상태별 스프라이트 배열 (0: 기본, 1: 30, 2: 60, 3: 90)

    private float currentHunger;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    // 이상한 먹이 효과 관련
    private bool isWeirdEffectActive = false;
    private float weirdEffectDuration = 5f; // 지속 시간
    private float weirdEffectTimer = 0f;

    void Start()
    {
        currentHunger = maxHunger; // 초기 포만감을 최대값으로 설정
        hungerBar.maxValue = maxHunger;
        hungerBar.value = currentHunger;

        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerController == null)
            Debug.LogError("PlayerController가 연결되지 않았습니다.");
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer가 연결되지 않았습니다.");
    }

    void Update()
    {
        // 포만감 감소
        if (isWeirdEffectActive)
        {
            // 이상한 먹이 효과 적용 중일 때 포만감 감소 속도 2배
            currentHunger -= (hungerDecreaseRate * 2f) * Time.deltaTime;
            weirdEffectTimer += Time.deltaTime;

            if (weirdEffectTimer >= weirdEffectDuration)
            {
                // 이상한 먹이 효과 종료
                isWeirdEffectActive = false;
                weirdEffectTimer = 0f;
            }
        }
        else
        {
            // 기본 포만감 감소
            currentHunger -= hungerDecreaseRate * Time.deltaTime;
        }

        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger); // 0~최대값 사이로 제한
        hungerBar.value = currentHunger;

        // 상태 업데이트
        UpdatePlayerState();

        // 포만감 0일 때 게임 오버 처리
        if (currentHunger <= 0)
        {
            Debug.Log("Game Over: Hunger Depleted");
            // 게임 오버 처리
        }
    }

    public void IncreaseHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
    }

    public void DecreaseHunger(float amount)
    {
        currentHunger -= amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        hungerBar.value = currentHunger;
    }
    public void ActivateWeirdEffect()
    {
        // 이상한 먹이 효과 활성화
        isWeirdEffectActive = true;
        weirdEffectTimer = 0f; // 타이머 초기화
    }


    public float GetCurrentHunger()
    {
        return currentHunger;
    }

    private void UpdatePlayerState()
    {
        // 1. 포만감에 따른 스프라이트 변경
        if (currentHunger < 30)
        {
            spriteRenderer.sprite = playerSprites[0]; // 기본 스프라이트
        }
        else if (currentHunger < 60)
        {
            spriteRenderer.sprite = playerSprites[1]; // 30 이상 스프라이트
        }
        else if (currentHunger < 90)
        {
            spriteRenderer.sprite = playerSprites[2]; // 60 이상 스프라이트
        }
        else
        {
            spriteRenderer.sprite = playerSprites[3]; // 90 이상 스프라이트
        }

        // 2. 포만감에 따른 이동 속도 감소
        float speedPenalty = Mathf.Floor(currentHunger / 30) * speedPenaltyPerLevel;
        playerController.moveSpeed = Mathf.Max(baseSpeed - speedPenalty, 0); // 속도가 0 이하로 내려가지 않도록

        // 3. 대쉬 가능 여부 설정
        playerController.canDash = currentHunger < 60; // 포만감이 60 이상이면 대쉬 불가능
    }
}