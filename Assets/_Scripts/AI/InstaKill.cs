using System;
using UnityEngine;

public class InstaKill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<PlayerTank>().TakeDamage(Mathf.Infinity);
    }
}
