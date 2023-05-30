using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefination : MonoBehaviour
{
    public E_PersistentType persistentType;
    public string ID;

    private void OnValidate()
    {
        if(persistentType == E_PersistentType.ReadWrite)
        {
            if (ID == string.Empty)
                ID = System.Guid.NewGuid().ToString();
        }
        else
        {
            ID = string.Empty;
        }
    }
}
