using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CalcuatingSystem : MonoBehaviour
{


    [SerializeField]
    public int Damage;
    public int Soul;
    public int Count;
    public int EnemyDamage; //임시 
    public int Power;
    public bool hit;
    public void TakeDamage()
    {
        Damage += 1;
    }
    public void HealDamage()
    {
        if (Damage >= EnemyDamage)
        {
            Damage -= 1;
        }
    }

    public void Guard()
    { 
        //상대 파워보다 내 파워가 더 강할시 가드성공 아니라면 Hit
    }


}
