using UnityEngine;
using Unity.Cinemachine;

public class FollowCamera : MonoBehaviour
{
    public CinemachineCamera followCamera;  
    public SpriteRenderer playerRenderer;
    public Vector2 baseOffset = new Vector2(2f, 0f);

    private CinemachinePositionComposer composer;

    void Start()
    {
        composer = followCamera.GetComponent<CinemachinePositionComposer>();
    }

    void LateUpdate()
    {
        float direction = playerRenderer.flipX ? -1f : 1f;
        composer.TargetOffset = new Vector3(baseOffset.x * direction, baseOffset.y, 0f);

    }
}