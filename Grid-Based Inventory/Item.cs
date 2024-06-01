using UnityEngine;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @date 2024-04-10
*/

[System.Serializable]
public struct Item
{
    public int x, y;
    public int prefabID;
    public bool isVertical;
    public int useAmount;
    public string data;

    public Item(int prefabID, int x, int y, bool isVertical, int useAmount, string data)
    {
        this.prefabID = prefabID;
        this.x = x;
        this.y = y;
        this.isVertical = isVertical;
        this.useAmount = useAmount;
        this.data = data;
    }

    public Item(int x, int y, bool isVertical, int useAmount, string data)
    {
        prefabID = -1;
        this.x = x;
        this.y = y;
        this.isVertical = isVertical;
        this.useAmount = useAmount;
        this.data = data;
    }

    public Item(ItemController itemController)
    {
        prefabID = itemController.item.ID;
        x = Mathf.RoundToInt(itemController.item.dimensions.x);
        y = Mathf.RoundToInt(itemController.item.dimensions.y);
        isVertical = itemController.isVertical;
        useAmount = itemController.currentAmount;
        data = itemController.data;
    }
}
