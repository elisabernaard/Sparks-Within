using UnityEngine;

public class FloatingEntities : MonoBehaviour
{
    public float speed = 1.0f;
    public float changeDirectionTime = 2.0f;
    public float movementRadius = 5.0f;     // 플레이어 중심 반경
    public float verticalOffset = 0.3f;     // 눈높이 위아래 허용 범위
    public Transform playerCamera;          // 플레이어 참조

    public bool isFrozen = false;

    private Vector3 targetDirection;
    private float timer;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        PickNewDirection();
    }

    void Update()
    {
        if (isFrozen || playerCamera == null) return;

        // 현재 위치에서 새로운 위치 계산
        Vector3 newPos = transform.position + targetDirection * speed * Time.deltaTime;

        // 플레이어로부터의 거리 계산 (Y축 포함)
        float distanceFromPlayer = Vector3.Distance(newPos, playerCamera.position);

        // movementRadius를 벗어나면 방향 전환
        if (distanceFromPlayer > movementRadius)
        {
            // 플레이어 방향으로 부드럽게 전환
            Vector3 directionToPlayer = (playerCamera.position - transform.position).normalized;
            targetDirection = Vector3.Lerp(targetDirection, directionToPlayer, 0.5f).normalized;
        }

        // Y축 제한 적용
        float targetY = Mathf.Clamp(newPos.y, playerCamera.position.y - verticalOffset, playerCamera.position.y + verticalOffset);
        transform.position = new Vector3(newPos.x, targetY, newPos.z);

        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            PickNewDirection();
            timer = 0f;
        }
    }

    void PickNewDirection()
    {
        // 더 넓은 범위의 랜덤 방향 생성
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-0.3f, 0.3f);
        float z = Random.Range(-1f, 1f);
        targetDirection = new Vector3(x, y, z).normalized;
    }
}
