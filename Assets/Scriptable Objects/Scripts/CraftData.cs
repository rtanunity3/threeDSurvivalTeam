using UnityEngine;


[System.Serializable]
public class RequireItem
{
    public ItemData reqItem;
    public int reqItemCnt;
}


[CreateAssetMenu(fileName = "CraftRecipe", menuName = "New CraftRecipe")]
public class CraftData : ScriptableObject
{
    [Header("TargetItem")]
    public ItemData targetItem;
    public int resultCnt; // 제작 결과 갯수

    [Header("reqItem")]
    public RequireItem[] reqItem; // UI상 최대 4개 종류로 제한하기


}
