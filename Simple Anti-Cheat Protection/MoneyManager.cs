using UnityEngine;
using TMPro;

/* All rights reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @date 2022-07-19
*/

public class MoneyManager : Singleton<MoneyManager>
{
    //Protected against Cheat Engine
    private ProjectedInt32Value money = new(0);

    public int Balance() { return money.GetValue(); }

    [SerializeField] private TextMeshProUGUI moneyTxt;
    [SerializeField] private InventoryController inventory;
    [SerializeField] private LootManager loot;

    [SerializeField]
    private FMODUnity.EventReference purchaseEvent;
    [SerializeField]
    private FMODUnity.EventReference sellEvent; 
    [SerializeField]
    private FMODUnity.EventReference insufficientEvent;

    public InventoryController merchantInventory;
    public TextMeshProUGUI nameTxt;

    [HideInInspector] public Merchant lastMerchant;

    public void LoadMoney(int amount)
    {
        money.SetValue(Mathf.Clamp(amount, 0, 9999));
        moneyTxt.text = money.GetValue().ToString("C");
    }

    public void Earn(int amount)
    {
        int addAmount = Mathf.Clamp(amount, 0, 256);

        int value = money.GetValue();
        value += Mathf.Clamp(addAmount, 0, 9999 - (value + addAmount));
        money.SetValue(value);

        moneyTxt.text = money.GetValue().ToString("C");

        Notification.Instance.Send("+" + amount.ToString("C"), Color.green);
    }

    public void Spend(int amount)
    {
        int value = money.GetValue();
        value -= Mathf.Clamp(amount, 0, value);
        money.SetValue(value);

        moneyTxt.text = money.GetValue().ToString("C");

        Notification.Instance.Send("-" + amount.ToString("C"), Color.red);
    }

    public void InsufficientFunds()
    {
        EmitOneShot2D.Play(insufficientEvent);
        Notification.Instance.Send("Insufficient Funds", Color.red);
    }

    public void SellHeldItem()
    {
        if (inventory.selectedItem && inventory._currentHeldItem != null)
        {
            GameObject item = inventory._currentHeldItem;
            int sellValue = item.GetComponent<ItemController>().item.sellValue;
            if (sellValue > 0)
            {
                Earn(sellValue);
                inventory.DeleteCurrentHeldItem();
                Destroy(item);
                EmitOneShot2D.Play(sellEvent);

                Reputation.Instance.GainRep(lastMerchant, Mathf.Clamp(sellValue, 1, 10));
            }
        }
    }

    public void BuyItem(int itemID, int cost)
    {
        bool hasSpace = loot.PlaceItemInSpace(itemID, inventory, false);

        if (hasSpace)
        {
            Spend(cost);
            EmitOneShot2D.Play(purchaseEvent);
            Reputation.Instance.GainRep(lastMerchant, Mathf.Clamp(cost, 1, 25));
        }
    }
}
