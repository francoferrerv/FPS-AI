using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class IceCreamSpawner : MonoBehaviour
{
    // Reference to the prefab to be spawned
    public Transform objectReference;
    private float x = 18.07f;
    private float y = 1.8f;
    private float z = -50.184f;
    public AudioSource audioSource;
    public GameObject particleEffect;
    Vector3 position;


    private void Start()
    {
        position = new Vector3(x, y, z);
    }
    // Method to spawn the prefab at a given position
    public IEnumerator SpawnIceCream(GameObject iceCream)
    {
            Effects();
            yield return new WaitForSeconds(0.5f);
            Instantiate(iceCream, objectReference.position + position, Quaternion.identity);
            yield return new WaitForSeconds(particleEffect.GetComponent<ParticleSystem>().main.duration);
            DestroyImmediate(particleEffect,true);
    }

    void Effects()
    {
        audioSource.Play();
        if (particleEffect != null)
        {
            Instantiate(particleEffect, objectReference.position + position, Quaternion.identity);
        }
    }
}
