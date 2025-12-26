using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructibleEnvironment : HealthBase
{
    [SerializeField] private GameObject graphic;
    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private float destroyTime = 5;
    [SerializeField] private float afterDestroyPhysicOnTime = 0;
    private bool isDestroy;

    [Space] 
    
    [SerializeField] private float mass;
    [SerializeField] private float drag;
    [SerializeField] private Vector3 massCenter;
    [SerializeField] private BoxCollider boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (!isDestroy && other.gameObject.layer == 3)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();

            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = drag;
            rb.centerOfMass = massCenter;
            
            if(other.gameObject.TryGetComponent(out Rigidbody playerRb))
                rb.velocity = -playerRb.velocity/2;
            
            if(boxCollider != null)
                boxCollider.enabled = true;
            
            gameObject.layer = 1;
            
            StartDestroy();
        }
    }

    private void StartDestroy()
    {
        if(isDestroy)
            return;
            
        isDestroy = true;

        if (destroyEffect != null)
        {
            destroyEffect.Play();
            destroyEffect.transform.parent = transform;
        }

        if (afterDestroyPhysicOnTime <= 0)
            gameObject.layer = 8;
        else
        {
            StartCoroutine(wait());
            IEnumerator wait()
            {
                yield return new WaitForSeconds(afterDestroyPhysicOnTime);
                gameObject.layer = 8;
            }
        }
        
        if(graphic != null)
            Destroy(graphic);

        Destroy(gameObject,destroyTime);
    }

    public override void Died()
    {
        Rigidbody rb = null;
        
        if(!gameObject.TryGetComponent<Rigidbody>(out Rigidbody d))
            rb = gameObject.AddComponent<Rigidbody>();

        if (rb != null)
        {
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = drag;
            rb.centerOfMass = massCenter;
        }
        
        if(boxCollider != null)
            boxCollider.enabled = true;
        gameObject.layer = 1;
        
        StartDestroy();
    }

    private void OnDestroy()
    {
        if(graphic != null)
            Destroy(graphic);
    }

    private void OnEnable()
    {
        if(graphic != null)
            graphic.SetActive(true);
    }
    
    private void OnDisable()
    {
        if(graphic != null)
            graphic.SetActive(false);
    }
}
