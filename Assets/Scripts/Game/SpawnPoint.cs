using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // ✨ 이 변수가 플레이어 시작 위치를 결정하는 "Order" 변수입니다.
    [Tooltip("Order 값이 가장 높은 스폰 포인트가 플레이어의 시작 위치가 됩니다.")]
    public int spawnOrder = 0; 
    
    // 이 스크립트는 Order 변수를 제공하는 역할만 하고, Start/Update 함수를 비워둡니다.
    // 위치 설정 로직은 'GameManager' 스크립트에서 처리할 것입니다.

    // 외부에서 위치를 쉽게 가져갈 수 있도록 프로퍼티를 제공합니다.
    public Vector3 SpawnPosition
    {
        get { return transform.position; }
    }
}