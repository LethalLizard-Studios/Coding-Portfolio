using UnityEngine;
using UnityEngine.EventSystems;

/* All rights reserved to Leland Carter, LethalLizard Studios.
 * @status SEMI-COMPLETE
 * @date 2024-01-07
*/

public class Grid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int x;
    public int y;
    public bool isFilled = false;
    public ItemController item;
    public InventoryController inventoryController;

    private ItemController _itemController;

    private void Start()
    {
        if (inventoryController._currentHeldItem != null)
            _itemController = inventoryController._currentHeldItem.GetComponent<ItemController>();
    }

    public void FetchItem(GameObject prefab, bool isAutonomous)
    {
        if (inventoryController.isMerchant)
        {
            //Check if item for sale, if affordable delete and buy
            if (inventoryController.otherControllers[0]._currentHeldItem == null && isFilled && item != null && item.isForSale)
            {
                TryPurchaseItemForSale();
                return;
            }

            //Check if item holding to sell 
            if (inventoryController.otherControllers[0]._currentHeldItem != null)
            {
                MoneyManager.Instance.SellHeldItem();
                return;
            }
        }

        //Check if items can swap
        if (isFilled && inventoryController._currentHeldItem != null && !isAutonomous && prefab == null)
        {
            //Dont allow guns to swap for edge case reasons
            if (item.item.type == ScriptableItem.Type.Gun)
                return;

            ItemController itemHolding = inventoryController._currentHeldItem.GetComponent<ItemController>();

            //If both items ammo combine
            if (item.item.type == ScriptableItem.Type.Ammo
                && itemHolding.item.type == ScriptableItem.Type.Ammo)
            {
                //Make sure ammo not holding has space for more
                if (item.currentAmount < item.item.useAmount)
                {
                    int difference = item.item.useAmount - item.currentAmount;

                    if (itemHolding.currentAmount <= difference)
                    {
                        //Put into other stack and delete
                        item.currentAmount += itemHolding.currentAmount;
                        item.DisplayQuantity();

                        Destroy(inventoryController._currentHeldItem);
                        inventoryController._currentHeldItem = null;

                        return;
                    }
                    else
                    {
                        //Fill other stack to max
                        item.currentAmount += difference;
                        itemHolding.currentAmount -= difference;
                        item.DisplayQuantity();
                        itemHolding.DisplayQuantity();

                        return;
                    }
                }
            }

            if (item.item.dimensions.x >= itemHolding.item.dimensions.x
                    && item.item.dimensions.y >= itemHolding.item.dimensions.y)
            {
                ItemController prevItem = item;

                prevItem.SetPlaced(false, isAutonomous);
                prevItem.UpdateColor(0);
                RemoveItemFromGrid(prevItem, false);

                _itemController = itemHolding;
                PlaceItem(itemHolding.gameObject, isAutonomous);

                return;
            }
        }

        if ((inventoryController._currentHeldItem != null || prefab != null) && !isFilled && item == null)
            PlaceItem(prefab, isAutonomous);
        else if (!isAutonomous && isFilled && inventoryController._currentHeldItem == null && item != null && prefab == null)
            TakeItem(isAutonomous);
    }

    private void TryPurchaseItemForSale()
    {
        int cost = item.item.costValue;

        if (item.specialDeal > 0)
            cost = Mathf.RoundToInt(item.item.costValue * Mathf.Abs(((float)item.specialDeal / 100.0f) - 1.0f));

        if (Reputation.Instance.GetRep(item.merchantID) < item.reputationRequired || MoneyManager.HasInstance)
            return;

        MoneyManager money = MoneyManager.Instance;

        if (money.Balance() < cost)
        {
            money.InsufficientFunds();
            return;
        }

        money.BuyItem(item.item.ID, cost);
    }

    private void PlaceItem(GameObject prefab, bool isAutonomous)
    {
        //Needed for CheckGridClick!!!
        _itemController = (prefab == null) ? inventoryController._currentHeldItem.GetComponent<ItemController>() : prefab.GetComponent<ItemController>();

        if (CheckGridClick(false))
        {
            if ((isAutonomous || !inventoryController.isLoot || inventoryController.GetInteractionManager().currentContainer.TypeAllowed(_itemController.item.type))
                && (inventoryController.specificType == ScriptableItem.Type.None || inventoryController.specificType == _itemController.item.type))
                PlaceItemInGrid(prefab, isAutonomous);
            else
                EmitOneShot2D.Play(inventoryController.deniedEvent);
        }
    }

    public void QuickLoot()
    {
        InventoryController destination = inventoryController.quickLootDestination;

        if (inventoryController.quickPrimaryDestination != null 
            && inventoryController.quickPrimaryDestination.transform.GetChild(0).gameObject.activeSelf)
            destination = inventoryController.quickPrimaryDestination;

        //Check inventory specific type restrictions
        if (destination.specificType != ScriptableItem.Type.None && destination.specificType != item.item.type)
            return;

        //Check loot box specific type restrictions
        if (destination.isLoot && !destination.GetInteractionManager().currentContainer.TypeAllowed(item.item.type))
            return;

        Item itemTemp = new Item(item);

        if (inventoryController.loot.PlaceItemInSpace(itemTemp, destination, false))
        {
            RemoveItemFromGrid(item, true);
            return;
        }

        item.SetPlaced(false, false);
        item.UpdateColor(0);
        RemoveItemFromGrid(item, false);
    }

    public void TakeItem(bool isAutonomous)
    {
        if (InventoryInputs.Instance.isQuickLooting)
        {
            StartCoroutine(item.QuickLootProgress(this));
            return;
        }

        item.SetPlaced(false, isAutonomous);
        item.UpdateColor(0);
        RemoveItemFromGrid(item, false);
    }

    public void RemoveItemFromGrid(ItemController item, bool destroyItem)
    {
        inventoryController.AddItem(item);
        ItemController tempItem = item;

        foreach (Grid g in tempItem.grids)
        {
            g.isFilled = false;
            g.item = null;
        }

        if (inventoryController.isHotbar && item.item.type.Equals(ScriptableItem.Type.Gun))
        {
            Inventory_Hotbar.Instance.UpdateHotbar();
            AnimationController.Instance.Dequip();
        }

        tempItem.grids.Clear();
        isFilled = false;
        this.item = null;

        if (destroyItem)
            Destroy(item.gameObject);
    }

    private void PlaceItemInGrid(GameObject prefab, bool isAutonomous)
    {
        if (prefab == null)
        {
            item = inventoryController._currentHeldItem.GetComponent<ItemController>();

            inventoryController._currentHeldItem.transform.position = transform.position;
            inventoryController.InsertCurrentHeldItem(item.gameObject);
        }
        else
        {
            item = prefab.GetComponent<ItemController>();

            prefab.transform.position = transform.position;
        }

        if (inventoryController.isHotbar && item.item.type.Equals(ScriptableItem.Type.Gun))
        {
            Inventory_Hotbar.Instance.UpdateHotbar();
        }


        CheckGridClick(true);

        item.SetPlaced(true, isAutonomous);
        item.UpdateColor(0);
        item.SetCoords(x, y);
        isFilled = true;
    }

    private bool CheckGridClick(bool fillIn)
    {
        if (_itemController == null) { _itemController = inventoryController._currentHeldItem.GetComponent<ItemController>(); }

        bool temp = false;

        if (!_itemController.isVertical && (x - (_itemController.GetWidth() - 1)) >= 0 
            && y + (_itemController.GetHeight() - 1) <= inventoryController.GetHeight() - (_itemController.GetHeight() - 1))
            temp = inventoryController.CheckSurroundings(x, y, _itemController, fillIn);
        else if (_itemController.isVertical && x + (_itemController.GetHeight() - 1) <= inventoryController.GetWidth() - 1 
            && y + (_itemController.GetWidth() - 1) <= inventoryController.GetHeight() - 1)
            temp = inventoryController.CheckSurroundings(x, y, _itemController, fillIn);

        return temp;
    }

    public void ValidatePosition()
    {
        if (CheckGridClick(false))
            _itemController.UpdateColor(0);
        else
            _itemController.UpdateColor(1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryContextMenu.Instance.Close();
            FetchItem(null, false);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null && !item.isForSale)
                InventoryContextMenu.Instance.Open(this);
            else
                InventoryContextMenu.Instance.Close();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryController._currentHeldItem != null && inventoryController.selectedItem)
        {
            inventoryController.lastGrid = this;
            ValidatePosition();
        }
        else if (inventoryController._currentHeldItem == null && item != null)
        {
            if (item.isForSale)
            {
                if (item.specialDeal > 0)
                    inventoryController.tooltips.Enable(item.GetDisplayName(), "Price $" + Mathf.RoundToInt((item.item.costValue) * Mathf.Abs(((float)item.specialDeal/100.0f)-1.0f))+" ("+item.specialDeal+"%)", item);
                else
                    inventoryController.tooltips.Enable(item.GetDisplayName(), "Price $" + item.item.costValue, item);
            }
            else
            {
                inventoryController.tooltips.Enable(item.GetDisplayName(), "$" + item.item.sellValue, item);
            }

            item.UpdateOutline(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item != null)
            item.UpdateOutline(false);

        inventoryController.tooltips.Disable();

        if (_itemController != null)
            _itemController.UpdateColor(1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Cursor")
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            OnPointerEnter(pointer);

            GamepadCursor.Instance.selectedGrid = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Cursor")
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            OnPointerExit(pointer);
        }
    }

    public void OnDisable()
    {
        if (item != null)
            item.UpdateOutline(false);
    }
}
