using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    public string cardName;
    public string effectText;
    public int grade;
    public int power;
    public int crit;
    public string countary;
    public bool trigger;

}
