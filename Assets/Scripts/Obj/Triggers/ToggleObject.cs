using UnityEngine;

[RequireComponent(typeof(GroupID))]
public class ToggleObject : MonoBehaviour
{
    // ActivationTrigger로부터 Toggle 명령을 받았을 때 실행
    public void ActivateToggle(bool isMultiActive, float delayTime)
    {
        // isMultiActive는 현재 이 스크립트에서 사용되지 않지만, 
        // ActivationTrigger의 인자로 넘어오는 것을 유지하여 유연성을 확보합니다.
        
        Invoke("PerformToggle", delayTime);
    }
    
    // Toggle (활성 상태 반전)
    private void PerformToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    // ActivationTrigger로부터 SetActive 명령을 받았을 때 실행
    public void SetToggleState(bool activateState, float delayTime)
    {
        // 코루틴을 사용하여 지연 시간을 처리합니다.
        StartCoroutine(PerformSetState(activateState, delayTime));
    }
    
    // SetActive (특정 상태로 설정)
    private System.Collections.IEnumerator PerformSetState(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(state);
    }
}