using UnityEngine;
using UnityEngine.UI;


public class RayCastSystem : MonoBehaviour
{

    [SerializeField] private GameObject uiPrefab; // 생성할 UI 프리팹 (또는 활성화할 UI 객체)
    [SerializeField] private Transform canvasTransform; // UI가 들어갈 Canvas의 Transform
    [SerializeField] private Canvas targetCanvas;
    //void Update()
    //{
       
    //    MouseClick();
    //}

    //public void MouseClick()
    //{
    //    // 1. 마우스 왼쪽 버튼(0) 클릭 순간 감지
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // 2. 카메라에서 마우스 위치를 향하는 광선 생성
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit; // 충돌 정보를 담을 바구니

    //        // 3. 광선을 쏘아(Raycast) 충돌 여부 확인
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            // 충돌한 대상의 이름을 출력하여 상호작용 확인
    //            Debug.Log("Hit Object : " + hit.collider.name);
    //            if (hit.collider.name == "BindZone")
    //            {
    //                // 1. 단순 화면 중앙이나 지정된 위치에 UI 생성
    //                if (uiPrefab != null && canvasTransform != null)
    //                {
    //                    SpawnUIAtMouse();
    //                }
    //                // 응용 : hit.point를 활용해 클릭한 지점에 이펙트 생성 가능
    //            }

    //        }
    //    }
    //}
    //private void SpawnUIAtMouse()
    //{
    //    if (uiPrefab == null || targetCanvas == null) return;

    //    // 1. 버튼 프리팹 생성
    //    GameObject spawnedUI = Instantiate(uiPrefab, targetCanvas.transform);
    //    RectTransform uiRect = spawnedUI.GetComponent<RectTransform>();

    //    // 2. 마우스 스크린 좌표를 Canvas 내부 로컬 좌표로 변환
    //    Vector2 localPoint;
    //    RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();

    //    // Canvas의 Render Mode에 맞춰 카메라 매개변수 설정
    //    Camera uiCamera = (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : targetCanvas.worldCamera;

    //    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, uiCamera, out localPoint))
    //    {
    //        // 3. 변환된 좌표를 anchoredPosition에 대입
    //        uiRect.anchoredPosition = localPoint;
    //    }
    //}
    //public void OnMouseOver()
    //{
       
    //        // 2. 카메라에서 마우스 위치를 향하는 광선 생성
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit; // 충돌 정보를 담을 바구니

    //        // 3. 광선을 쏘아(Raycast) 충돌 여부 확인
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            // 충돌한 대상의 이름을 출력하여 상호작용 확인
    //            Debug.Log("Hit Object : " + hit.collider.name);

    //            // 응용 : hit.point를 활용해 클릭한 지점에 이펙트 생성 가능
    //        }

        
    //}
}
