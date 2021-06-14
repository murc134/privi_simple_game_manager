using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPlayerControls : MonoBehaviour
{
    [SerializeField] 
    protected Camera cam;                  // A reference to the main camera in the scenes transform

    private GameManager gameManager;

    public GameManager GameManager
    {
        protected get
        {
            return gameManager;
        }
        set
        {
            gameManager = value;
        }
    }
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
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
        }
    }

}
