using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour 
{
    [SerializeField]
    private GameObject slotPref;
    public GameObject inventoryPanel, chestPanel, descriptionPanel;
    public GameObject invContent, chestContent;
    public ItemData[] items;
    public List<GameObject> inventorySlots = new List<GameObject>();
    public List<GameObject> currentChestSlots = new List<GameObject>();

    public bool canDrag = false;


    private void Awake() {
        inventoryPanel = GameObject.Find("CharInventoryPanel");
        chestPanel = GameObject.Find("ChestPanel");
        invContent = GameObject.Find("InventoryContent");
        chestContent = GameObject.Find("ChestContent");
        descriptionPanel = GameObject.Find("DescriptionPanel");
    }
    private void Start() {
        inventoryPanel.SetActive(false);
        chestPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }

    private void Update()
    {
        canDrag = inventoryPanel.activeSelf && chestPanel.activeSelf;
    }

    public void CreateItem(int itemId, List<ItemData> itemsList) {
        //ItemData item = items[itemId]; так нельзя
        ItemData item = new ItemData(items[itemId].name, items[itemId].id,
            items[itemId].count, items[itemId].isUniq, items[itemId].description);
        if (!item.isUniq && itemsList.Count > 0) {
            for (int i = 0; i < itemsList.Count; i++) {
                if (item.id == itemsList[i].id) {
                    itemsList[i].count += 1;
                    break;
                }
                else if (i == itemsList.Count - 1) {
                    itemsList.Add(item);
                    break;
                }
            }
        }
        else if (item.isUniq || (!item.isUniq && itemsList.Count == 0)) {
            itemsList.Add(item);
        }
    }

    public void InstantiatingItem(ItemData item, Transform parent, List<GameObject> itemsList) {
        GameObject go = Instantiate(slotPref);
        go.transform.SetParent(parent.transform);
        go.AddComponent<Slot>();
        go.GetComponent<Slot>().itemData = item;
        go.transform.Find("ItemNameText").GetComponent<Text>().text = item.name;
        go.transform.Find("ItemImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(item.name);
        go.transform.Find("ItemCountText").GetComponent<Text>().text = item.count.ToString();
        go.transform.Find("ItemCountText").GetComponent<Text>().color =
            item.isUniq ? Color.clear : Color.white; //используем тернанрый оператор
        itemsList.Add(go);
    }
}
