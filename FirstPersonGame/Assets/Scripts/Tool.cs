using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField]
    private ToolTypes type;
    [SerializeField]
    private ToolMaterials material;
    public int damageToEnemy;
    public int damageToBlock; 
    
    void Start()
    {
        damageToEnemy = (int)type * (int)material;
        switch (type)
        {
            case ToolTypes.PICKAXE:
                damageToBlock = 4 * (int)material;
                break;
            case ToolTypes.SWORD:
                damageToBlock = 1 * (int)material;
                break;
        }
    }
}
