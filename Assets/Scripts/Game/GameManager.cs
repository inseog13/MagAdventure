using UnityEngine;
using System.Linq; // LINQ를 사용하여 정렬합니다.

public class GameManager : MonoBehaviour
{
    // 인스펙터에서 플레이어를 직접 할당합니다.
    public Transform playerTransform;

    private void Start()
    {
        // 1. 플레이어 오브젝트 유효성 검사
        if (playerTransform == null)
        {
            // 플레이어 태그를 사용하여 찾기 시도 (선택 사항)
            GameObject playerGO = GameObject.FindWithTag("Player"); 
            if (playerGO != null)
            {
                playerTransform = playerGO.transform;
            }

            if (playerTransform == null)
            {
                Debug.LogError("GameManager: 플레이어 Transform이 할당되지 않았거나 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
                return;
            }
        }

        // 2. 씬의 모든 SpawnPoint (깃발) 오브젝트를 찾습니다.
        SpawnPoint[] allSpawnPoints = FindObjectsOfType<SpawnPoint>();

        if (allSpawnPoints.Length == 0)
        {
            Debug.LogError("GameManager: 씬에서 SpawnPoint 스크립트를 가진 오브젝트(깃발)를 찾을 수 없습니다.");
            return;
        }

        // 3. ✨ Order 변수(spawnOrder)를 기준으로 내림차순 정렬하여 가장 높은 값을 가진 깃발을 찾습니다.
        // LINQ의 OrderByDescending을 사용합니다.
        SpawnPoint highestOrderSpawnPoint = allSpawnPoints
            .OrderByDescending(sp => sp.spawnOrder)
            .FirstOrDefault(); // 가장 높은 Order를 가진 첫 번째 요소를 가져옵니다.

        // 4. 플레이어를 최종 스폰 위치로 이동시킵니다.
        if (highestOrderSpawnPoint != null)
        {
            playerTransform.position = highestOrderSpawnPoint.SpawnPosition;
            Debug.Log($"가장 높은 Order ({highestOrderSpawnPoint.spawnOrder})를 가진 깃발로 플레이어 위치를 설정했습니다: {highestOrderSpawnPoint.gameObject.name}");
        }
        else
        {
            Debug.LogError("가장 높은 Order를 가진 SpawnPoint를 찾을 수 없습니다.");
        }
        
        // 이 스크립트는 초기화만 수행하므로 비활성화합니다.
        enabled = false;
    }
}