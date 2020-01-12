using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    const float dstBetwwenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        // Bounds는 경계를 얻기 위에 사용되며 Collider,Mesh,Renderer에서 Bounds를 사용한다.
        Bounds bounds = collider.bounds;
        // bounds를 축소한다 skinWidth * -2 만큼
        bounds.Expand(skinWidth * -2);

        // 각 점의 위치를 나타내준다.
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        //horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetwwenRays);
        //verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetwwenRays);

        //horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        // 위부분 왼쪽과 오른쪽
        public Vector2 topLeft, topRight;
        // 아랫부분 왼쪽과 오른쪽
        public Vector2 bottomLeft, bottomRight;
    }
}
