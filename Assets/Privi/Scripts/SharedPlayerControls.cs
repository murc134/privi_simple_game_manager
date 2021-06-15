using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPlayerControls : MonoBehaviour
{
    [SerializeField] 
    protected Camera cam;                  // A reference to the main camera in the scenes transform

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (cam == null)
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                cam = Camera.main;
            }

        }
    }

}
