using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public Sprite craftImage; // 이미지
    public string craftDescription; // 설명
    public string[] craftNeedItem;  // 필요한 아이템
    public int[] craftNeedItemCount; // 필요한 아이템의 개수
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    private bool isUIActive = false;  // CraftManual UI 활성 상태
    private bool isPreviewActive = false; // 미리 보기 활성화 상태

    [SerializeField]
    private GameObject baseUI; // 기본 베이스 UI

    private int tabNumber = 0; // 탭
    private int page = 1; // 탭 페이지
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;

    [SerializeField]
    private Craft[] craft_build;  // 건축용 탭

    // 필요한 UI Slot 요소
    [SerializeField]
    private GameObject[] go_Slots;
    [SerializeField]
    private Image[] image_Slot;
    [SerializeField]
    private Text[] text_SlotName;
    [SerializeField]
    private Text[] text_SlotDescription;
    [SerializeField]
    private Text[] text_SlotNeedItem;

    private GameObject currentPreview; // 현재 미리보기 프리팹
    private GameObject currentPrefab; // 현재 생성될 프리팹

    [SerializeField]
    private Transform playerTransform;  // 플레이어 위치

    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    void Start()
    {
        TabSetting(0);
    }

    public void TabSetting(int tabSettingNumber)
    {
        tabNumber = tabSettingNumber;
        page = 1;

        switch (tabNumber)
        {
            case 0:
                craft_SelectedTab = craft_build;
                break;
        }
        TabSlotSetting();
    }

    private void ClearSlot()
    {
        foreach (var slot in go_Slots)
        {
            slot.SetActive(false);
        }
    }

    private void TabSlotSetting()
    {
        ClearSlot();

        int startSlotNumber = (page - 1) * go_Slots.Length; // 4 의 배수

        for (int i = startSlotNumber; i < startSlotNumber + go_Slots.Length && i < craft_SelectedTab.Length; i++)
        {
            int slotIndex = i - startSlotNumber;
            go_Slots[slotIndex].SetActive(true);
            image_Slot[slotIndex].sprite = craft_SelectedTab[i].craftImage;
            text_SlotName[slotIndex].text = craft_SelectedTab[i].craftName;
            text_SlotDescription[slotIndex].text = craft_SelectedTab[i].craftDescription;

            for (int j = 0; j < craft_SelectedTab[i].craftNeedItem.Length; j++)
            {
                text_SlotNeedItem[slotIndex].text += craft_SelectedTab[i].craftNeedItem[j];
                text_SlotNeedItem[slotIndex].text += " x " + craft_SelectedTab[i].craftNeedItemCount[j] + "\n";
            }
        }
    }
    /*
    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotName[i].text = "";
            text_SlotDescription[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);
        }
    }
    */
    public void SlotClick(int slotNumber)
    {
        selectedSlotNumber = slotNumber + (page - 1) * go_Slots.Length;
        Craft selectedCraft = craft_SelectedTab[selectedSlotNumber];
        // ResourceManager 인스턴스를 사용하여 프리팹 인스턴스화
        GameObject previewPrefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/{selectedCraft.go_PreviewPrefab.name}");
        if (previewPrefab != null)
        {
            currentPreview = ResourceManager.Instance.Instantiate(previewPrefab);
            currentPreview.transform.position = playerTransform.position + playerTransform.forward;
            currentPreview.transform.rotation = Quaternion.identity;
        }
        GameObject prefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/{selectedCraft.go_prefab.name}");
        if (prefab != null)
        {
            currentPrefab = prefab;
        }

        isPreviewActive = true;
        ToggleWindow();
        //baseUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isPreviewActive)
            ToggleWindow();

        if (isPreviewActive)
            UpdatePreviewPosition();

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }

    private void UpdatePreviewPosition()
    {
        // 캐릭터가 바라보는 방향으로 레이캐스트
        Vector3 rayStart = playerTransform.position + Vector3.up;
        Vector3 rayDirection = playerTransform.forward;
        float maxDistance = 50.0f; // 최대 거리

        if (Physics.Raycast(rayStart, rayDirection, out hitInfo, maxDistance, layerMask))
        {
            // 지면과 충돌한 경우
            currentPreview.transform.position = hitInfo.point;
        }
        else
        {
            // 지면과 충돌하지 않았을 경우, 아래 방향으로 지면 찾기
            if (Physics.Raycast(rayStart, Vector3.down, out hitInfo, maxDistance, layerMask))
            {
                currentPreview.transform.position = hitInfo.point;
            }
            else
            {
                // 지면을 찾지 못한 경우, 처리 로직
                // 예: 프리뷰를 숨기거나, 사용자에게 알림 등
            }
        }

        /*
        float forwardDistance = 10.0f; // 캐릭터로부터 앞으로 얼마나 떨어뜨릴지 결정하는 거리
        float sphereRadius = 0.5f; // SphereCast의 반지름
        Vector3 castStartPosition = playerTransform.position + Vector3.up + (playerTransform.forward * forwardDistance);
        if (Physics.SphereCast(castStartPosition, sphereRadius, Vector3.down, out hitInfo, range + 1, layerMask))
        {
            currentPreview.transform.position = hitInfo.point;
        }

        /*
        if (Physics.Raycast(playerTransform.position, playerTransform.forward, out hitInfo, range, layerMask))
        {
            currentPreview.transform.position = hitInfo.point;
        }
        */
    }

    private void Build()
    {
        if (isPreviewActive && currentPreview.GetComponent<PreviewObject>().isBuildable())
        {
            Instantiate(currentPrefab, hitInfo.point, currentPreview.transform.rotation);
            ResetCraftingProcess();
        }
    }
    private void ToggleWindow()
    {
        isUIActive = !isUIActive;
        baseUI.SetActive(isUIActive);
        PlayerController.instance.ToggleCursor(isUIActive);
    }
    private void ResetCraftingProcess()
    {
        if (isPreviewActive)
        {
            Destroy(currentPreview);
        }

        isUIActive = false;
        isPreviewActive = false;
        currentPreview = null;
        currentPrefab = null;
    }
    private void Cancel()
    {
        ResetCraftingProcess();
        baseUI.SetActive(false);
    }

    public void RightPageSetting()
    {
        if (page < craft_SelectedTab.Length / go_Slots.Length + 1) // 1페이지부터 시작하니까 +1
            page++;
        else
            page = 1;

        TabSlotSetting();
    }

    public void LeftPageSetting()
    {
        if (page != 1)
            page--;
        else
            page = craft_SelectedTab.Length / go_Slots.Length + 1; // 1페이지부터 시작하니까 +1

        TabSlotSetting();
    }
}
