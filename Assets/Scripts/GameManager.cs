using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Prefabs of the player controllers that will be instantiated
    /// </summary>
    public SharedPlayerControls[] PlayerControllerPrefabs;

    /// <summary>
    /// Instances of the player controllers
    /// </summary>
    private SharedPlayerControls[] playerController;

    /// <summary>
    /// Keeps track of the players position when switching controllers
    /// Will also be the initial spawn position
    /// </summary>
    private Vector3 lastPosition = new Vector3(0, 1, 0);

    public Vector3 LastPosition
    {
        private get
        {
            return lastPosition;
        }
        set
        {
            lastPosition = value;
        }
    }

    /// <summary>
    /// Keeps track of the players rotation in Euler space when switching controllers
    /// </summary>
    private Vector3 lastRotationEuler = new Vector3(0, 0, 0);

    /// <summary>
    /// Getter and Setter for players euler space rotation when switching controllers
    /// </summary>
    public Quaternion LastRotation
    {
        private get
        {
            return Quaternion.Euler(lastRotationEuler);
        }
        set
        {
            lastRotationEuler = value.eulerAngles;
        }
    }

    /// <summary>
    /// The index of the used PlayerController
    /// </summary>
    [SerializeField]
    private int currentControllerIndex = 0;

    /// <summary>
    /// Maximum index of usable PlayerController
    /// </summary>
    private int MaxControllerIndex
    {
        get
        {
            return PlayerControllerPrefabs.Length-1;
        }
    }

    /// <summary>
    /// Returns the currently active controller while making sure that the <see cref="currentControllerIndex"/> has a valid value
    /// </summary>
    public SharedPlayerControls ActiveController
    {
        get
        {
            if (MaxControllerIndex <= 0) { return null; }
            
            if(currentControllerIndex > MaxControllerIndex)
            {
                currentControllerIndex = 0;
            }
            else if (currentControllerIndex < 0)
            {
                currentControllerIndex = MaxControllerIndex;
            }

            return playerController[currentControllerIndex];
        }
    }

    private void Start()
    {
        // First initialize the player controller instance array with the count of available player controller prefabs
        playerController = new SharedPlayerControls[PlayerControllerPrefabs.Length];
        
        for (int i=0;i< PlayerControllerPrefabs.Length;i++)
        {
            // First create instances of PlayerController
            playerController[i] = Instantiate<SharedPlayerControls>(PlayerControllerPrefabs[i]);

            // Set start position to initial value
            playerController[i].transform.position = LastPosition;

            // Set start rotation to initial value
            playerController[i].transform.rotation = LastRotation;

            // Only the first controller will be active
            playerController[i].gameObject.SetActive(i==currentControllerIndex);
            
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            NextController();
        }
    }

    /// <summary>
    /// Switch to the next player controller
    /// </summary>
    public void NextController()
    {
        setControllerIndex(currentControllerIndex + 1);
    }

    /// <summary>
    /// Switch to the previous player controller
    /// </summary>
    public void PreviousController()
    {
        setControllerIndex(currentControllerIndex - 1);
    }

    /// <summary>
    /// Sets the controller index to the desired value
    /// </summary>
    /// <param name="value"></param>
    private void setControllerIndex(int value)
    {
        LastPosition = ActiveController.transform.position;
        LastRotation = ActiveController.transform.rotation;
        ActiveController.gameObject.SetActive(false);
        currentControllerIndex = value;
        ActiveController.transform.position = LastPosition;
        ActiveController.transform.rotation = LastRotation;
        ActiveController.gameObject.SetActive(true);
    }
}
