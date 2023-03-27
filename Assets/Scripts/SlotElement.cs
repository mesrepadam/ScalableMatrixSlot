using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotElement : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;

    public void SetNumberText(int value) => numberText?.SetText($"{value}");
    
}
