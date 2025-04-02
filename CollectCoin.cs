using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectCoin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource coinFX;

    void OnTriggerEnter(Collider other) {
        coinFX.Play();
        MasterInfo.coinCount +=1;
        this.gameObject.SetActive(false);
    }
}
