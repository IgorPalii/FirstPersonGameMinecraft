using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, 
    IEndDragHandler, IPointerDownHandler, IPointerUpHandler, 
    IPointerEnterHandler, IPointerExitHandler
{
    private Transform tempParentForSlots;
    private InventoryManager inventoryManager;    
    private PlayerController pController;
    private string parentName;
    public ItemData itemData;

    private void Start()
    {
        tempParentForSlots = GameObject.Find("TempParentForSlots").transform;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        parentName = transform.parent.name;
    }

    public void OnPointerEnter(PointerEventData data) {
        if(itemData != null) { //записываем имя итема, на который навели курсор
            pController.itemYouCanEquipeName = itemData.name;
        }
    }

    public void OnPointerExit(PointerEventData data) {
        pController.itemYouCanEquipeName = PlayerController.EQUIPE_NOT_SELECTED_TEXT;
    }

    public void OnPointerDown(PointerEventData data) {
        inventoryManager.descriptionPanel.SetActive(true);
        inventoryManager.descriptionPanel.transform.position = transform.position;
        if(itemData != null) {
        inventoryManager.descriptionPanel.transform.GetChild(0).GetComponent<Text>().text = 
                itemData.description;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        inventoryManager.descriptionPanel.SetActive(false);
    }

    public void OnDrag(PointerEventData data) {
        if(inventoryManager.canDrag) {
            inventoryManager.descriptionPanel.SetActive(false);
            transform.SetParent(tempParentForSlots);
            transform.position = data.position;
        }
    }

    public void OnEndDrag(PointerEventData data) {
        if (inventoryManager.canDrag) {
            float slotInventoryContentdistance = Vector3.Distance(transform.position, inventoryManager.invContent.transform.position);
            float slotChestContentdistance = Vector3.Distance(transform.position, inventoryManager.chestContent.transform.position);
            if (slotInventoryContentdistance < slotChestContentdistance) {
                if (parentName == "InventoryContent") {
                    transform.SetParent(inventoryManager.invContent.transform);
                }
                else {                    
                    inventoryManager.currentChestSlots.Remove(gameObject);
                    pController.currentChestItems.Remove(itemData);
                    AddToListOnDrag(inventoryManager.inventorySlots, pController.inventoryItems, inventoryManager.invContent.transform);
                }
            }
            else {
                if (parentName == "ChestContent") {
                    transform.SetParent(inventoryManager.chestContent.transform);
                }
                else {                    
                    inventoryManager.inventorySlots.Remove(gameObject);
                    pController.inventoryItems.Remove(itemData);
                    AddToListOnDrag(inventoryManager.currentChestSlots, pController.currentChestItems, inventoryManager.chestContent.transform);
                }
            }            
        }
        inventoryManager.canDrag = false;
    }

    private void AddToListOnDrag(List<GameObject> slots, List<ItemData> items, Transform parent) {
        if (itemData.isUniq || slots.Count == 0) {
            slots.Add(gameObject);
            items.Add(itemData);
            transform.SetParent(parent);
            parentName = transform.parent.name;
        }
        else if (!itemData.isUniq) {
            for (int i = 0; i < slots.Count; i++) {
                if (slots[i].GetComponent<Slot>().itemData.id == itemData.id) {
                    items[i].count += itemData.count;
                    slots[i].transform.Find("ItemCountText").GetComponent<Text>().text =
                    slots[i].GetComponent<Slot>().itemData.count.ToString();
                    Destroy(gameObject);
                    break;
                }
                else if (i == slots.Count - 1) {
                    slots.Add(gameObject);
                    items.Add(itemData);
                    transform.SetParent(parent);
                    parentName = transform.parent.name;
                    break;
                }
            }
        }
    }
}

