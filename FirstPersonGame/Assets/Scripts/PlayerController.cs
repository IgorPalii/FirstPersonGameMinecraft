using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float gravityScale = 9.8f, speedScale = 5f, jumpForce = 8f;
    private float verticalSpeed = 0f, turnSpeed = 150f;
    private CharacterController controller;
    [SerializeField] private Camera goCamera;
    private float mouseX, mouseY, currentAngleX;

    [SerializeField]
    private GameObject particleObject;
    private const float hitScaleSpeed = 15f;
    private float hitLastTime = 0f;

    private InventoryManager inventoryManager;
    [HideInInspector]
    public List<ItemData> inventoryItems, currentChestItems;
    private Transform itemParent;
  
    public const string EQUIPE_NOT_SELECTED_TEXT = "EquipeNotSelected";
    [HideInInspector] 
    public string itemYouCanEquipeName = EQUIPE_NOT_SELECTED_TEXT;
    [SerializeField]
    private GameObject[] equipableItems;
    private GameObject currentEquipedItem;
    private RaycastHit hit;

    public int health { get; set; } = 100;


    void Awake() {
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();        
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        itemParent = GameObject.Find("InventoryContent").transform;
        inventoryManager.CreateItem(0, inventoryItems);
        inventoryManager.CreateItem(5, inventoryItems);
    }

    private void Start() {
        EquipItem("Pickaxe");
    }

    private void FixedUpdate() {
        if (!inventoryManager.inventoryPanel.activeSelf 
            && !inventoryManager.chestPanel.activeSelf) {
            RotateCharacter();
            MoveCharacter();
        }
        else {
            Cursor.visible = true;
        }
    }

    private void Update() {      
        if (Physics.Raycast(goCamera.transform.position, goCamera.transform.forward, out hit, 5f)) {
            if (!inventoryManager.chestPanel.activeSelf &&
            !inventoryManager.inventoryPanel.activeSelf) {
                if (Input.GetMouseButton(0)) {
                    ObjectInteraction(hit.transform.gameObject);
                } 
            }
        }
        if (Input.GetMouseButtonDown(1)) {//выносим сюда
            ItemAbility();
        }

        if (Input.GetKeyDown(KeyCode.E) && 
            !inventoryManager.inventoryPanel.activeSelf) {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            CloseInventoryPanels();
        }
        else if (Input.GetKeyDown(KeyCode.E) && 
            inventoryManager.inventoryPanel.activeSelf &&
            itemYouCanEquipeName != EQUIPE_NOT_SELECTED_TEXT) {
            EquipItem(itemYouCanEquipeName);
        }
    }

    private void ObjectInteraction(GameObject tempObject) {        
        switch (tempObject.tag) {
            case "Block":
                Dig(tempObject.GetComponent<Block>());
                break;
            case "Enemy":
                AttackEnemy(tempObject.GetComponent<EnemyController>());
                break;
            case "Chest":
                currentChestItems = tempObject.GetComponent<Chest>().chestItems;
                OpenChest();
                break;
        }        
    }

    private void ItemAbility() {
        switch (currentEquipedItem.name) {
            case "Ground":
                if(hit.transform != null) {
                    CreateBlock();
                }                
                break;
            case "Meat":
                EatMeat();
                break;
            case "Bow":
                if (CheckAvailabilityItem("Arrow")) {
                    currentEquipedItem.GetComponent<Bow>().isDrawn = true;
                }                
                break;
            default:
                break;
        }
    }

    private void AttackEnemy(EnemyController enemy) {
        if (Time.time - hitLastTime > 1 / hitScaleSpeed) {
            currentEquipedItem.GetComponent<Animator>().SetTrigger("attack");
            Tool currentToolInfo;
            if (currentEquipedItem.TryGetComponent<Tool>(out currentToolInfo)) {
                enemy.health -= currentToolInfo.damageToEnemy;
            }
            else {
                enemy.health -= 1;
            }
            if (enemy.health < 1) {
                Destroy(enemy.gameObject);
            }
        }
    }

    private bool CheckAvailabilityItem(string itemName)
    {
        foreach (ItemData item in inventoryItems) {
            if (item.name == itemName && item.count > 0) {
                return true;
            }
        }
        return false;
    }

    private void CreateBlock() {
        GameObject blockPref = Resources.Load<GameObject>("Ground");
        Vector3 tempPos = hit.transform.gameObject.transform.position;
        if (hit.transform.gameObject.tag == "Block") {
            GameObject currentBlock = Instantiate(blockPref);
            if (hit.point.y == tempPos.y + 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x, tempPos.y + 1, tempPos.z);
            }
            else if (hit.point.y == tempPos.y - 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x, tempPos.y - 1, tempPos.z);
            }
            else if (hit.point.z == tempPos.z + 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z + 1);
            }
            else if (hit.point.z == tempPos.z - 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z - 1);
            }
            else if (hit.point.x == tempPos.x + 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x + 1, tempPos.y, tempPos.z);
            }
            else if (hit.point.x == tempPos.x - 0.5f) {
                currentBlock.transform.position = new Vector3(tempPos.x - 1, tempPos.y, tempPos.z);
            }
            ModifyItemCount("Ground");
        }
    }

    public void ModifyItemCount(string itemName) {
        foreach (ItemData item in inventoryItems) {
            if (item.name == itemName) {
                item.count--;
                if (item.count <= 0) {
                    inventoryItems.Remove(item);
                    if (currentEquipedItem.name != "Bow") { //добавляем это условие
                        EquipItem(inventoryItems[0].name);
                    }
                }
                break;
            }
        }
    }

    private void EatMeat() { 
        Meat meat;
        if (currentEquipedItem.TryGetComponent<Meat>(out meat)) {
            health += meat.healthScore;
            Mathf.Clamp(health, 0, 100); 
        }
        ModifyItemCount("Meat");
    }

    private void EquipItem(string toolName) {
        foreach (GameObject tool in equipableItems) {
            if (tool.name == toolName) {
                tool.SetActive(true);
                currentEquipedItem = tool;
                toolName = EQUIPE_NOT_SELECTED_TEXT;
            }
            else {
                tool.SetActive(false);
            }
        }
    }
    
    private void OpenInventory() {
        inventoryManager.inventoryPanel.SetActive(true);
        if (inventoryItems.Count > 0) {
            for (int i = 0; i < inventoryItems.Count; i++) {
                inventoryManager.InstantiatingItem(inventoryItems[i], itemParent, inventoryManager.inventorySlots);
            }
        }
    }

    private void OpenChest() {
        if (!inventoryManager.chestPanel.activeSelf) {
            inventoryManager.chestPanel.SetActive(true);
            Transform itemParent = GameObject.Find("ChestContent").transform;
            for (int i = 0; i < currentChestItems.Count; i++) {
                inventoryManager.InstantiatingItem(currentChestItems[i], itemParent, inventoryManager.currentChestSlots);
            }
        }
    }

    private void CloseInventoryPanels() {
        foreach (GameObject slot in inventoryManager.currentChestSlots) {
            Destroy(slot);
        }
        foreach (GameObject slot in inventoryManager.inventorySlots) {
            Destroy(slot);
        }
        inventoryManager.currentChestSlots.Clear();
        inventoryManager.inventorySlots.Clear();
        inventoryManager.inventoryPanel.SetActive(false);
        inventoryManager.chestPanel.SetActive(false);
    }

    private void Dig(Block block) {
        if (Time.time - hitLastTime > 1 / hitScaleSpeed) {
            currentEquipedItem.GetComponent<Animator>().SetTrigger("attack");
            hitLastTime = Time.time;//пишем код между этой строкой
            Tool currentToolInfo;
            if (currentEquipedItem.TryGetComponent<Tool>(out currentToolInfo)) {
                block.health -= currentToolInfo.damageToBlock;
            }
            else {
                block.health -= 1;
            }            
            GameObject go = Instantiate(particleObject, block.gameObject.transform);// и этой
            go.GetComponent<ParticleSystemRenderer>().material =
            block.gameObject.GetComponent<MeshRenderer>().material;
            if (block.health <= 0) {
                block.DestroyBehaviour();
            }
        }
    }

    private void RotateCharacter() {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(new Vector3(0f,
            mouseX * turnSpeed * Time.fixedDeltaTime, 0f));
        currentAngleX += mouseY * turnSpeed * Time.fixedDeltaTime * -1f;
        currentAngleX = Mathf.Clamp(currentAngleX, -60f, 60f);
        goCamera.transform.localEulerAngles = new Vector3(currentAngleX, 0f, 0f);
    }

    private void MoveCharacter() {
        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        velocity = transform.TransformDirection(velocity) * speedScale;
        if (controller.isGrounded) {
            verticalSpeed = 0f;
            if (Input.GetKeyDown(KeyCode.Space)) {
                verticalSpeed = jumpForce;
            }
        }
        verticalSpeed -= gravityScale * Time.fixedDeltaTime;
        velocity.y = verticalSpeed;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name.StartsWith("mini"))
        {
            inventoryManager.CreateItem(2, inventoryItems);
            Destroy(col.gameObject);
        }
    }
}
