using System.Collections.Generic;
using UnityEngine;

public class InventoryContents : MonoBehaviour
{
    public Transform parent;
    public List<Item> items = new List<Item>();
    public List<GameObject> currentlyStoredItems = new List<GameObject>();
}
