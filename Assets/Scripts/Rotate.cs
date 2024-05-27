using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float degreesPerSecond = 30.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around the Z axis
        transform.Rotate(0, 0, degreesPerSecond * Time.deltaTime);
    }
}
