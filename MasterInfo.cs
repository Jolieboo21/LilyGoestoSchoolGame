using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    [SerializeField] GameObject coinDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      // Update is called once per frame
    void Update()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "COINS: " + coinCount;
    }
}
