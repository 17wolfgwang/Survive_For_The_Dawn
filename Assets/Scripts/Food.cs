using UnityEngine;

public class Food : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHunger playerHunger = collision.GetComponent<PlayerHunger>();

            if (playerHunger != null)
            {
                // 태그에 따라 효과 적용
                if (CompareTag("LightFood"))
                {
                    playerHunger.IncreaseHunger(3f); // 가벼운 먹이: 포만감 3 증가
                }
                else if (CompareTag("HeavyFood"))
                {
                    playerHunger.IncreaseHunger(10f); // 무거운 먹이: 포만감 10 증가
                }
                else if (CompareTag("WeirdFood"))
                {
                    playerHunger.ActivateWeirdEffect(); // 이상한 먹이: 포만감 감소 속도 증가
                }
            }

            // 먹이 제거
            Destroy(gameObject);
        }
    }
}