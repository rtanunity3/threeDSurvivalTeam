using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    public static CraftManager instance;

    public CraftSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public CraftData[] craftDataList; // 아이템ID , 레시피

    public GameObject recipeWindow;

    [Header("CraftList")]
    public GameObject craftRecipeListContent;
    public GameObject craftItemPrefab;

    [Header("Selected Item")]
    public GameObject reqItemList;

    [Header("Button Group")]
    public GameObject buttonGroup;
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button useButton;
    [Header("Button Group")]
    public GameObject dialog;
    public TextMeshProUGUI dialogText;

    private PlayerController controller;
    private CraftData selectedItem;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        controller = GetComponent<PlayerController>();

        //CraftListDic = new Dictionary<int, CraftRecipe>();

        //CraftListDic.Add(1, new CraftRecipe(1, "화살", "활 소모품", "", 1, 1, 100, 1, 0, 0, 0, 0));
        //CraftListDic.Add(2, new CraftRecipe(2, "도끼", "자원을 채취할때 사용합니다.", "", 1, 1, 100, 1, 0, 0, 0, 0));
    }
    private void Start()
    {
        recipeWindow.SetActive(false);
        buttonGroup.SetActive(false);
        useButton.onClick.AddListener(CraftItem);
        uiSlots = new CraftSlotUI[craftDataList.Length];

        Init();
    }


    private void Init()
    {
        for (int i = 0; i < craftDataList.Length; i++)
        {
            CraftSlotUI craftSlot = Instantiate(craftItemPrefab, craftRecipeListContent.transform).GetComponent<CraftSlotUI>();
            craftSlot.Set(craftDataList[i]);
            uiSlots[i] = craftSlot;
        }
    }

    private void unSelect()
    {
        selectedItem = null;

        foreach (CraftSlotUI craftSlot in uiSlots)
        {
            craftSlot.UnRelease();
        }

        CraftSlotUI[] tmp = reqItemList.GetComponentsInChildren<CraftSlotUI>();
        foreach (CraftSlotUI slot in tmp)
        {
            Destroy(slot.gameObject);
        }

        buttonGroup.SetActive(false);

    }

    public void OnCraftButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started && !dialog.activeInHierarchy)
        {
            Toggle();
        }
    }


    public void Toggle()
    {
        if (recipeWindow.activeInHierarchy)
        {
            recipeWindow.SetActive(false);
            controller.ToggleCursor(false);
            GameManager.instance.UIDepth--;
        }
        else if (GameManager.instance.UIDepth == 0)
        {
            unSelect();
            recipeWindow.SetActive(true);
            controller.ToggleCursor(true);
            GameManager.instance.UIDepth++;
        }
    }


    public void SetSelectedItemInfo(CraftData recipe)
    {
        selectedItem = recipe;
        foreach (CraftSlotUI craftSlot in uiSlots)
        {
            // TODO:  비교 itemid 로 변경
            if (selectedItem.targetItem.displayName == craftSlot.curItem.displayName)
            {
                craftSlot.OnRelease();
            }
            else
            {
                craftSlot.UnRelease();
            }
        }

        // 제작재료 있던거 삭제
        CraftSlotUI[] tmp = reqItemList.GetComponentsInChildren<CraftSlotUI>();
        foreach (CraftSlotUI slot in tmp)
        {
            Destroy(slot.gameObject);
        }

        foreach (RequireItem requireItem in recipe.reqItem)
        {
            CraftSlotUI RequireSlot = Instantiate(craftItemPrefab, reqItemList.transform).GetComponent<CraftSlotUI>();
            RequireSlot.Set(requireItem);
        }

        buttonGroup.SetActive(true);
        icon.sprite = recipe.targetItem.icon;
        quantityText.text = recipe.resultCnt.ToString();
    }

    private void CraftItem()
    {
        if (Inventory.instance.CheckCraftRequireItem(selectedItem.reqItem))
        {
            Inventory.instance.CraftItem(selectedItem);
            dialogText.text = $"{selectedItem.targetItem.displayName}를 제작했습니다.";
            dialog.SetActive(true);
        }
        else
        {
            //Debug.LogWarning("재료 부족");
            dialogText.text = "재료가 부족합니다.";
            dialog.SetActive(true);
        }
    }
}

//public class CraftRecipe
//{
//    public int itemID;
//    public string name;
//    public string description;
//    public string prefabFile;

//    public int resItemID;
//    public int resItemCnt;

//    public int reqItemID01;
//    public int reqItemCnt01;
//    public int reqItemID02;
//    public int reqItemCnt02;
//    public int reqItemID03;
//    public int reqItemCnt03;

//    public CraftRecipe(int _itemID, string _name, string _description, string _prefabFile
//        , int _resItemID, int _resItemCnt
//        , int _reqItemID01, int _reqItemCnt01
//        , int _reqItemID02, int _reqItemCnt02
//        , int _reqItemID03, int _reqItemCnt03)
//    {
//        itemID = _itemID;
//        name = _name;
//        description = _description;
//        prefabFile = _prefabFile;
//        resItemID = _resItemID;
//        resItemCnt = _resItemCnt;

//        reqItemID01 = _reqItemID01;
//        reqItemCnt01 = _reqItemCnt01;
//        reqItemID02 = _reqItemID02;
//        reqItemCnt02 = _reqItemCnt02;
//        reqItemID03 = _reqItemID03;
//        reqItemCnt03 = _reqItemCnt03;
//    }
//}
