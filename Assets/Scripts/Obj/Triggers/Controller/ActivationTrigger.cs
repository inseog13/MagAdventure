using UnityEngine;

public enum ControlType { None, Move, Rotate, Toggle, SetActive }

public class ActivationTrigger : MonoBehaviour
{
    [Header("Target Group Settings")]
    [Tooltip("The ID of the group this trigger will control.")]
    public int targetGroupID = 1;

    [Header("Trigger Activation")]
    [Tooltip("If checked, the trigger can be activated multiple times (Multi-Active).")]
    public bool isMultiActive = false;
    [Tooltip("The type of action this trigger will perform.")]
    public ControlType controlType = ControlType.Move;
    [Tooltip("Delay before the action starts.")]
    public float delayTime = 0f;

    [Header("Move/Rotate Control Settings")]
    [Tooltip("The time taken for the action (Move or Rotate).")]

    public float duration = 1f;
    [Tooltip("The easing function used for movement interpolation.")]

    public MoveObject.EasingMode easingMode = MoveObject.EasingMode.None;

    [Header("Move Settings (ControlType: Move)")]
    [Tooltip("The relative distance the target will move. e.g., (0, 3, 0) moves 3 units up.")]
    public Vector3 moveOffset = Vector3.zero;

    [Header("Rotate Settings (ControlType: Rotate)")] // 👈 Rotate 설정 추가
    [Tooltip("The rotation to apply in Euler angles (degrees). e.g., (0, 0, 90) rotates 90 degrees on Z.")]
    public Vector3 rotationOffset = Vector3.zero;

    [Header("SetActive Control Settings")]
    [Tooltip("If true, targets are set active. If false, targets are set inactive.")]
    public bool activateTarget = true; 

    // Single-Active mode tracking
    private bool hasTriggered = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isMultiActive && hasTriggered) return;
            if (!isMultiActive) hasTriggered = true;

            // FindObjectsByType is used to avoid CS0618 warning.
            GroupID[] groupObjects = FindObjectsByType<GroupID>(FindObjectsSortMode.None);

            foreach (GroupID groupObject in groupObjects)
            {
                if (groupObject.groupID == targetGroupID)
                {
                    switch (controlType)
                    {
                        case ControlType.Move:
                            MoveObject moveScript = groupObject.GetComponent<MoveObject>();
                            if (moveScript != null)
                            {
                                // Pass all movement data from the trigger to the object
                                moveScript.ActivateMovement(moveOffset, duration, delayTime, easingMode);
                            }
                            break;

                        case ControlType.Rotate:
                            MoveObject rotateScript = groupObject.GetComponent<MoveObject>();
                            if (rotateScript != null)
                            {
                                // MoveObject의 ActivateRotation 메서드를 호출
                                rotateScript.ActivateRotation(rotationOffset, duration, delayTime, easingMode);
                            }
                            break;
                            
                        case ControlType.Toggle:
                            ToggleObject toggleScript = groupObject.GetComponent<ToggleObject>();
                            if (toggleScript != null)
                            {
                                // Pass only delay and MultiActive context
                                toggleScript.ActivateToggle(isMultiActive, delayTime);
                            }
                            break;
                            
                        case ControlType.SetActive:
                            ToggleObject setScript = groupObject.GetComponent<ToggleObject>();
                            if (setScript != null)
                            {
                                // Pass target state and delay
                                setScript.SetToggleState(activateTarget, delayTime);
                            }
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isMultiActive)
        {
            // Reset trigger state for Multi-Active mode only
            hasTriggered = false;
        }
    }
}