using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlotUI : MonoBehaviour
{

    public Button button;
    public Image icon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI reqCntText;
    public ItemData curItem;

    public GameObject selected;

    private void Awake()
    {
        //outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        //outline.enabled = equipped;
    }

    public void Set(CraftData recipe)
    {
        curItem = recipe.targetItem;
        gameObject.SetActive(true);
        icon.sprite = curItem.icon;
        itemNameText.text = recipe.targetItem.displayName;
        quantityText.text = recipe.resultCnt.ToString();

        button.onClick.AddListener(() => OnButtonClick(recipe));
    }
    public void Set(RequireItem reqItem)
    {
        curItem = reqItem.reqItem;
        gameObject.SetActive(true);
        icon.sprite = curItem.icon;
        itemNameText.text = reqItem.reqItem.displayName;

        // TODO : itemID 생기면 해당 값으로 검색해서 현재 소지량 넣기
        ItemSlot invenItem = Inventory.instance.GetItemStack(reqItem.reqItem);
        quantityText.text = invenItem != null ? invenItem.quantity.ToString() : "0";
        reqCntText.text = $"x {reqItem.reqItemCnt}";
        reqCntText.gameObject.SetActive(true);
    }


    public void OnRelease()
    {
        selected.SetActive(true);
    }
    public void UnRelease()
    {
        selected.SetActive(false);
    }

    public void Clear()
    {
        curItem = null;
        gameObject.SetActive(false);

        itemNameText.text = string.Empty;
        quantityText.text = string.Empty;
        reqCntText.gameObject.SetActive(false);
    }

    public void OnButtonClick(CraftData recipe)
    {
        //Debug.Log("craftslotUI OnButtonClick");

        CraftManager.instance.SetSelectedItemInfo(recipe);
    }
}
