using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterBehavior : MonoBehaviour
{
    public float patrolSpeed = 2.0f; // 배회 속도
    public float chaseSpeed = 3.5f; // 추격 속도
    public float attackRange = 1.5f; // 공격 범위
    public float detectionRange = 5.0f; // 추격 시작 범위
    public float damage = 10.0f; // 공격력
    public float damageInterval = 1.0f; // 대미지 입히는 간격 (초)
    public float wanderRadius = 10.0f; // 배회 범위
    public float wanderInterval = 3.0f; // 배회 지점 변경 간격
    public float viewAngle = 120.0f; // 시야각

    private Vector3 wanderPoint;
    private bool isChasing = false;
    private PlayerHealth targetPlayerHealth;
    private float wanderTimer;
    private bool isAttacking = false;

    private List<Transform> players = new List<Transform>(); // 모든 플레이어들의 Transform 목록
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            players.Add(playerObject.transform);
        }

        SetRandomWanderPoint();
        wanderTimer = wanderInterval;

        Debug.Log("Found " + players.Count + " players with tag 'Player'.");
    }

    void Update()
    {
        Transform closestPlayer = GetClosestPlayerInView();
        if (closestPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, closestPlayer.position);

            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
                targetPlayerHealth = closestPlayer.GetComponent<PlayerHealth>();
                Debug.Log("Chasing player: " + closestPlayer.name);
            }
            else if (distanceToPlayer > detectionRange * 1.5f)
            {
                isChasing = false;
                targetPlayerHealth = null;
                Debug.Log("Stopped chasing player: " + closestPlayer.name);
            }

            if (isChasing)
            {
                ChasePlayer(closestPlayer);
            }
            else
            {
                Wander();
            }

            if (distanceToPlayer <= attackRange)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            else
            {
                isAttacking = false;
            }
        }
        else
        {
            Wander();
        }

        UpdateViewAngle();
    }

    Transform GetClosestPlayerInView()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.up, directionToPlayer);
            if (angle < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestPlayer = player;
                }
            }
        }

        if (closestPlayer != null)
        {
            Debug.Log("Closest player in view: " + closestPlayer.name);
        }
        else
        {
            Debug.Log("No players in view.");
        }

        return closestPlayer;
    }

    void SetRandomWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.z = 0; // 2D 게임인 경우 z 좌표를 유지
        wanderPoint = randomDirection;
    }

    void Wander()
    {
        if (Vector3.Distance(transform.position, wanderPoint) < 0.1f || wanderTimer <= 0)
        {
            SetRandomWanderPoint();
            wanderTimer = wanderInterval;
        }
        else
        {
            wanderTimer -= Time.deltaTime;
        }

        transform.position = Vector3.MoveTowards(transform.position, wanderPoint, patrolSpeed * Time.deltaTime);
    }

    void ChasePlayer(Transform player)
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        while (targetPlayerHealth != null && Vector3.Distance(transform.position, targetPlayerHealth.transform.position) <= attackRange)
        {
            targetPlayerHealth.TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
        isAttacking = false;
    }

    void UpdateViewAngle()
    {
        if (lineRenderer == null) return;

        int segments = 20;
        float angle = viewAngle / 2;
        float step = viewAngle / segments;
        float length = detectionRange;

        lineRenderer.positionCount = segments + 2;
        lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (transform.eulerAngles.z - angle + step * i);
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            lineRenderer.SetPosition(i + 1, transform.position + direction * length);
        }
    }
}
