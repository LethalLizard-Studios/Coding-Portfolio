using UnityEngine;
using TMPro;
using UnityEditor;

/* This is a code snippet from a full system inside of SANCTION
-- LICENSE MIT
-- @author: Leland Carter
*/

public class MoneyManager : MonoBehaviour
{
    // Protected against Cheat Engine
    private ProjectedInt32Value _money = new(0);
    private string _balanceHash;

    public int Balance => _money.GetValue();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI sendersNameText;

    [Header("Inventory")]
    [SerializeField] private LootManager lootManager;
    [SerializeField] private InventoryController merchantInventory;
    [SerializeField] private InventoryController playerInventory;

    [HideInInspector] public Merchant mostRecentMerchant;

    // FMOD sound effect references
    [Header("Sounds")]
    [SerializeField] private FMODUnity.EventReference purchaseEvent;
    [SerializeField] private FMODUnity.EventReference sellEvent;
    [SerializeField] private FMODUnity.EventReference insufficientEvent;

    private static readonly Color GAIN_COLOR = Color.green;
    private static readonly Color LOSS_COLOR = Color.red;

    private const int MAX_BALANCE = 9999;
    private const int MAX_EARN_AMOUNT = 256;

    private const int MAX_REP_GAIN_ON_SELL = 10;
    private const int MAX_REP_GAIN_ON_PURCHASE = 25;

    private Notification _notification;
    private Reputation _reputation;

    private void Awake()
    {
        _notification = Notification.Instance;
        _reputation = Reputation.Instance;
    }

    public void LoadSavedBalance(int amount)
    {
        SetBalance(amount);

        // Initialize the hash
        _balanceHash = HashUtility.GenerateHash(_money.GetValue());
        if (!VerifyBalanceIntegrity())
        {
            LogicLogger.Warning(this, "Saved balance has been tampered with.");
        }
    }

    public void Earn(int amount)
    {
        if (!VerifyBalanceIntegrity())
        {
            LogicLogger.Warning(this, "Balance integrity check failed. Possible tampering detected.");
        }

        if (amount <= 0)
        {
            LogicLogger.Warning(this, "Attempted to earn a non-positive amount.");
            return;
        }

        // Capped to 256 to prevent cheating in large amounts
        int amountEarned = Mathf.Clamp(amount, 0, MAX_EARN_AMOUNT);
        int newBalance = Mathf.Clamp(Balance + amountEarned, 0, MAX_BALANCE);

        SetBalance(newBalance);
        _notification.Send($"+{amountEarned:C}", GAIN_COLOR);
    }

    public void Spend(int amount)
    {
        if (!VerifyBalanceIntegrity())
        {
            LogicLogger.Warning(this, "Balance integrity check failed. Possible tampering detected.");
        }

        if (amount <= 0)
        {
            LogicLogger.Warning(this, "Attempted to spend a non-positive amount.");
            return;
        }

        if (amount > Balance)
        {
            InsufficientFunds();
            return;
        }

        SetBalance(Balance - amount);
        _notification.Send($"-{amount:C}", LOSS_COLOR);
    }

    public void InsufficientFunds()
    {
        EmitOneShot2D.Play(insufficientEvent);
        _notification.Send("Insufficient Funds", LOSS_COLOR);
    }

    public void SellCurrentlyHeldItem()
    {
        if (playerInventory == null || !playerInventory.HasSelectedItem || playerInventory.currentHeldItem == null)
        {
            LogicLogger.Warning(this, "Attempted to sell held item but no item was found.");
            return;
        }

        GameObject heldItem = playerInventory.currentHeldItem;
        if (heldItem.TryGetComponent<ItemController>(out ItemController itemController))
        {
            int sellValue = itemController.item.sellValue;

            if (sellValue > 0)
            {
                ProcessItemSale(sellValue, heldItem);
            }
        }
        else
        {
            LogicLogger.Warning(this, $"{heldItem.name} does not have an ItemController component.");
        }
    }

    private void ProcessItemSale(int sellValue, GameObject item)
    {
        Earn(sellValue);
        playerInventory.DeleteCurrentHeldItem();
        Destroy(item);
        EmitOneShot2D.Play(sellEvent);

        // Capped at 10 so expensive items have a max rep gain
        _reputation.GainRep(mostRecentMerchant, Mathf.Clamp(sellValue, 1, MAX_REP_GAIN_ON_SELL));
    }

    public void PurchaseItem(int itemID, int cost)
    {
        if (cost > Balance)
        {
            InsufficientFunds();
            return;
        }

        bool hasSpaceForItem = lootManager.PlaceItemInSpace(itemID, playerInventory, false);
        if (hasSpaceForItem)
        {
            Spend(cost);
            EmitOneShot2D.Play(purchaseEvent);

            // Higher cap to allow for larger rep gain comapred to purchasing items
            _reputation.GainRep(mostRecentMerchant, Mathf.Clamp(cost, 1, MAX_REP_GAIN_ON_PURCHASE));
        }
    }

    private void SetBalance(int amount)
    {
        _money.SetValue(Mathf.Clamp(amount, 0, MAX_BALANCE));
        _balanceHash = HashUtility.GenerateHash(_money);
        RefreshBalanceUI();
    }

    private bool VerifyBalanceIntegrity()
    {
        // Verify against the hash
        return HashUtilities.VerifyHash(_money, _balanceHash);
    }

    private void RefreshBalanceUI()
    {
        balanceText.text = Balance.ToString("C");
    }
}
