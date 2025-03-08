using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUITooltip : MonoBehaviour
{
    #region Fields

    public TextMeshProUGUI text;

    #endregion

    #region MonoBehaviour

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    #endregion

    #region Methods



    #endregion
}
