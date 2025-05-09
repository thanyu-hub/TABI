using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class MRUKRaycaster : MonoBehaviour
{
    public Transform rayStartPoint;
    public float rayLength = 5f;
    public Material previewMaterial; // Material for preview during dragging
    public MRUKAnchor.SceneLabels labelFilter; // Target surface type (e.g., floor, wall)

    public EffectMesh roomGuardianEffectMesh;

    public TextMeshPro debugText; // Debug text display

    public Transform buttonsPanel;
    public Transform textPanel;

    private Vector3 endNormal;


    private bool isLaycastMode = false; // Whether raycast mode is active

    private Vector3 startPoint; // Drag start point
    private Vector3 endPoint; // Drag end point
    private bool isDragging = false; // Whether dragging is in progress
    private GameObject previewPlane; // Plane for preview display
    private MRUKRoom room; // Current MRUK room information

    private LineRenderer lineRenderer;

    private bool guardianVisible = false;

    // public GameObject panelObject;
    // public Transform canvasPanelTransform;

    public GameObject simulatorObject;

    private Vector3 fixedCameraForward;

    private bool isinit = false;


    public LeftHandTextManager leftHandTextManager; // Reference to the script for text display

    public GameObject hitCursorPrefab;
    private GameObject hitCursor;




    public void activateRayCastMode()
    {
        Debug.Log("Raycast mode activated");
        textPanel.gameObject.SetActive(false);
        buttonsPanel.gameObject.SetActive(false);
        leftHandTextManager.SetTextState(LeftHandTextManager.State.RayCastMode);

        if (simulatorObject != null)
        {
            simulatorObject.SetActive(false);
        }
        if (roomGuardianEffectMesh != null && !guardianVisible)
        {
            roomGuardianEffectMesh.HideMesh = false;
            guardianVisible = true;
            debugText.gameObject.SetActive(true);
            isLaycastMode = true;
            lineRenderer.gameObject.SetActive(true);
        }
    }

    public void deActivateRayCastMode()
    {
        Debug.Log("Raycast mode deactivated");

        leftHandTextManager.SetTextState(LeftHandTextManager.State.None);

        if (isinit)
        {
            textPanel.gameObject.SetActive(true);
            buttonsPanel.gameObject.SetActive(true);
            if (simulatorObject != null)
            {
                simulatorObject.SetActive(true);
            }
        }

        if (roomGuardianEffectMesh != null && guardianVisible)
        {
            roomGuardianEffectMesh.HideMesh = true;
            guardianVisible = false;
            debugText.gameObject.SetActive(false);
            lineRenderer.gameObject.SetActive(false);
            isLaycastMode = false;

            if (previewPlane != null)
            {
                Destroy(previewPlane);
                Debug.Log("Preview plane destroyed");
            }
        }
    }
    void Start()
    {
        debugText.gameObject.SetActive(false);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.003f;
        lineRenderer.endWidth = 0.003f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.gameObject.SetActive(false);

        if (hitCursorPrefab != null)
        {
            hitCursor = Instantiate(hitCursorPrefab);
            hitCursor.SetActive(false);
        }
        else
        {
            Debug.LogError("hitCursorPrefab is not assigned.");
        }
    }

    // void Start()
    // {
    //     debugText.gameObject.SetActive(false);
    //     lineRenderer = gameObject.AddComponent<LineRenderer>();
    //     lineRenderer.startWidth = 0.003f;
    //     lineRenderer.endWidth = 0.003f;
    //     lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //     lineRenderer.startColor = Color.red;
    //     lineRenderer.endColor = Color.red;
    //     lineRenderer.gameObject.SetActive(false);

    //     hitCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //     hitCursor.transform.localScale = Vector3.one * 0.05f;
    //     hitCursor.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
    //     hitCursor.GetComponent<Renderer>().material.color = Color.yellow;
    //     Destroy(hitCursor.GetComponent<Collider>());
    //     hitCursor.SetActive(false);
    // }
    bool IsSurfaceFacingUp(Vector3 normal, float threshold = 0.9f)
    {
        return Vector3.Dot(normal.normalized, Vector3.up) >= threshold;
    }

    void Update()
    {
        if (rayStartPoint == null || roomGuardianEffectMesh == null || isLaycastMode == false)
        {
            return;
        }
        activateRayCastMode();
        Debug.Log("Raycasting in progress...");

        room = MRUK.Instance.GetCurrentRoom();

        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);


        bool hasHit = room.Raycast(ray, rayLength, LabelFilter.FromEnum(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, hasHit ? hit.point : ray.origin + ray.direction * rayLength);
        if (hasHit)
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;

            string label = anchor.AnchorLabels[0];

            debugText.transform.position = hitPoint;
            debugText.transform.rotation = Quaternion.LookRotation(-hitNormal);

            debugText.text = "ANCHOR = " + label;
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("Trigger pressed!");
            if (hasHit)
            {
                startPoint = hit.point;
                isDragging = true;
                Debug.Log($"Drag start point: {startPoint}");

                fixedCameraForward = Camera.main.transform.forward;

                if (previewPlane == null)
                {
                    previewPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    previewPlane.GetComponent<Renderer>().material = previewMaterial;
                    Destroy(previewPlane.GetComponent<Collider>());
                    Debug.Log("Preview plane created");
                }
            }
            else
            {
                Debug.LogWarning("Drag start point not found (Raycast failed)");
            }
        }

        if (isDragging && OVRInput.Get(OVRInput.Button.One))
        {
            if (hasHit && IsSurfaceFacingUp(hit.normal))
            {
                endPoint = hit.point;
                endNormal = hit.normal;
                hitCursor.SetActive(true);
                hitCursor.transform.position = hit.point + hit.normal * 0.001f;
                hitCursor.transform.rotation = Quaternion.LookRotation(hit.normal);
                Debug.Log($"Dragging, current point: {endPoint}");
                UpdatePreviewPlane(startPoint, endPoint, hit.normal);
            }
            else
            {
                hitCursor.SetActive(false);
                Debug.LogWarning("Surface is not facing up, dragging disabled");
                Debug.LogWarning("Raycast failed during dragging");
            }
        }

        if (isDragging && OVRInput.GetUp(OVRInput.Button.One))
        {
            Debug.Log("Trigger released!");
            isDragging = false;
            hitCursor.SetActive(false);

            if (previewPlane != null)
            {
                Destroy(previewPlane);
                Debug.Log("Preview plane destroyed");
            }

            if (hasHit)
            {
                CreateFinalPanel(startPoint, endPoint, endNormal);
                Debug.Log($"Final plane created: Center {(startPoint + endPoint) / 2}");
                deActivateRayCastMode();
            }
            else
            {
                Debug.LogWarning("Drag end point not found (Raycast failed)");
            }
        }
    }
    void UpdatePreviewPlane(Vector3 start, Vector3 end, Vector3 surfaceNormal)
    {
        if (previewPlane == null)
        {
            previewPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            previewPlane.GetComponent<Renderer>().material = previewMaterial;
            Destroy(previewPlane.GetComponent<Collider>());
        }

        Vector3 center = (start + end) / 2f;

        // 軸計算：z = テーブル面法線, y = カメラ方向（短辺）
        Vector3 zAxis = -surfaceNormal.normalized;
        Vector3 camForward = fixedCameraForward;
        Vector3 yAxis = Vector3.ProjectOnPlane(camForward, zAxis).normalized;
        if (yAxis.sqrMagnitude < 1e-4f) yAxis = Vector3.up;
        Vector3 xAxis = Vector3.Cross(yAxis, zAxis).normalized;

        Quaternion rotation = Quaternion.LookRotation(zAxis, yAxis);

        Vector3 delta = end - start;
        float width = Mathf.Abs(Vector3.Dot(delta, xAxis));
        float height = Mathf.Abs(Vector3.Dot(delta, yAxis));

        previewPlane.transform.position = center;
        previewPlane.transform.rotation = rotation;
        previewPlane.transform.localScale = new Vector3(width, height, 1f);

        Debug.Log($"Preview updated: center={center}, width={width}, height={height}");
    }

    void CreateFinalPanel(Vector3 start, Vector3 end, Vector3 surfaceNormal)
    {
        Vector3 center = (start + end) / 2f;

        // 少し浮かせる
        float lift = 0.1f;
        center += surfaceNormal.normalized * lift;

        // テーブル面に垂直な軸（面の法線、Z軸扱い）
        Vector3 zAxis = -surfaceNormal.normalized;

        // カメラ方向を zAxis に直交な平面に投影 → これを「短辺＝yAxis」とする
        Vector3 camForward = fixedCameraForward;
        Vector3 yAxis = Vector3.ProjectOnPlane(camForward, zAxis).normalized;
        if (yAxis.sqrMagnitude < 1e-4f) yAxis = Vector3.up;

        // 残りの軸（長辺方向）
        Vector3 xAxis = Vector3.Cross(yAxis, zAxis).normalized;

        // 回転の設定（forward: z, up: y）
        Quaternion rotation = Quaternion.LookRotation(zAxis, yAxis);
        Quaternion tilt_rotation = Quaternion.AngleAxis(-30f, xAxis) * rotation;

        // サイズ計算（短辺 = カメラ向き, 長辺 = 横方向）
        Vector3 delta = end - start;
        float width = Mathf.Abs(Vector3.Dot(delta, xAxis));
        float height = Mathf.Abs(Vector3.Dot(delta, yAxis));

        // --- 両端に ButtonsPanel, TextPanel を配置（長辺方向） ---
        if (buttonsPanel != null && textPanel != null)
        {
            float panelOffset = width * 0.5f * 0.9f;  // 少し内側に配置
            Vector3 leftPos = center - xAxis * panelOffset;
            Vector3 rightPos = center + xAxis * panelOffset;

            buttonsPanel.position = leftPos;
            buttonsPanel.rotation = tilt_rotation;

            textPanel.position = rightPos;
            textPanel.rotation = tilt_rotation;

            Debug.Log($"ButtonsPanel at {leftPos}, TextPanel at {rightPos}");
        }

        // Simulator配置（テーブル面から浮かせてカメラ高さに）
        if (simulatorObject != null && Camera.main != null)
        {
            Vector3 eye = Camera.main.transform.position;
            Vector3 forward = zAxis;
            float offset = 0.3f;

            Vector3 simPos = center + forward * offset;
            simPos.y = eye.y;

            simulatorObject.transform.position = simPos;
            simulatorObject.transform.rotation = rotation;

            Debug.Log($"Simulator placed at {simPos}, looking {rotation.eulerAngles}");
        }

        isinit = true;
    }


}
