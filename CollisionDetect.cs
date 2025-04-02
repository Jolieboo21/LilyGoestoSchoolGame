using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CollisionDetect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnim;
    [SerializeField] AudioSource collisionFX;
    [SerializeField] GameObject mainCam;

     [SerializeField] GameObject fadeOut;
    void OnTriggerEnter(Collider other) {
        StartCoroutine(CollisionEnd());
       
    }

    IEnumerator CollisionEnd()
    {
        collisionFX.Play();
       thePlayer.GetComponent<PlayerMovement>().enabled = false;
       playerAnim.GetComponent<Animator>().Play("Stumble Backwards");
       mainCam.GetComponent<Animator>().Play("CollisionCam");
       yield return new WaitForSeconds(3);
       fadeOut.SetActive(true);
       yield return new WaitForSeconds(2);
       SceneManager.LoadScene(0);
    }
}
