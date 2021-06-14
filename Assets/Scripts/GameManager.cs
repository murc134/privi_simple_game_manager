using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//public delegate void SwitchPlayerControllerEvent(string controllername);

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of GameManager
    /// </summary>
    private static GameManager instance;

    /// <summary>
    /// getter for Singleton Instance of GameManager
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Event when player controller switches
    /// </summary>
    // Could alternatively be either delegate or UnityEvent. In the latter case, it would be possible to assign event via inspector
    public Action OnSwitchPlayerControllerEvent;

    /// <summary>
    /// The name of the next player controller
    /// </summary>
    public string NextControllerName
    {
        get
        {
            int nextIndex = validateControllerIndex(currentControllerIndex + 1);
            return PlayerControllerPrefabs[nextIndex].GetType().Name;
        }
    }

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
    [SerializeField] private Vector3 lastPosition = new Vector3(0, 1, 0);

    /// <summary>
    /// The initial player spawn position
    /// </summary>
    private Vector3 positionOnStart;

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
    [SerializeField] private Vector3 lastRotationEuler = Vector3.zero;

    /// <summary>
    /// The initial playerspawn rotation
    /// </summary>
    private Quaternion rotationEulerOnStart;

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

            currentControllerIndex = validateControllerIndex(currentControllerIndex);

            return playerController[currentControllerIndex];
        }
    }

    /// <summary>
    /// The maximum distance to the ground before resetting 
    /// </summary>
    [SerializeField]
    private float maxFallingDist = -5;

    /// <summary>
    /// A reference to the plattform
    /// </summary>
    [SerializeField]
    private Transform ground;

    /// <summary>
    /// If true, the fire1 button will be ignored
    /// </summary>
    public bool PreventFire1 { get; set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogError($"There seem to be multiple instances of {GetType().Name} in the scene. Please make sure to only have one instance");
        }
    }

    private void Start()
    {
        // First initialize the player controller instance array with the count of available player controller prefabs
        playerController = new SharedPlayerControls[PlayerControllerPrefabs.Length];

        positionOnStart = LastPosition;
        rotationEulerOnStart = LastRotation;

        for (int i=0;i< PlayerControllerPrefabs.Length;i++)
        {
            // First create instances of PlayerController
            playerController[i] = Instantiate<SharedPlayerControls>(PlayerControllerPrefabs[i]);

            // Only the first controller will be active
            playerController[i].gameObject.SetActive(i==currentControllerIndex);

            // Only if ground is defined
            if(ground != null)
            {
                // Reparent transform of PlayerController to ground
                playerController[i].transform.SetParent(ground);
            }
        }
        // Set start position to initial value
        playerController[currentControllerIndex].transform.position = LastPosition;

        // Set start rotation to initial value
        playerController[currentControllerIndex].transform.rotation = LastRotation;

    }

    private void Update()
    {
        // switch Playercontroller on Fire1 Down
        if (!PreventFire1 && Input.GetButtonDown("Fire1"))
        {
            NextController();
        }

        // Reset when falling off platform
        if (ActiveController.transform.localPosition.y < maxFallingDist)
        {
            ResetPlayerPositions();
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
        OnSwitchPlayerControllerEvent.Invoke();
    }

    /// <summary>
    /// Checks if the given index is within the range
    /// </summary>
    /// <param name="index">The index that is being checked</param>
    /// <returns>returns a valid index value</returns>
    private int validateControllerIndex(int index)
    {
        if (index > MaxControllerIndex)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = MaxControllerIndex;
        }
        return index;
    }
    /// <summary>
    /// Resets the <see cref="LastPosition"/> and <see cref="LastRotation"/> to the default value on Startup
    /// </summary>
    private void resetLastTransformSettings()
    {
        LastPosition = positionOnStart;
        LastRotation = rotationEulerOnStart;
    }

    /// <summary>
    /// Resets the position and rotation of the player to the default value on Startup
    /// </summary>
    public void ResetPlayerPositions()
    {

        // Reset to Startup values
        resetLastTransformSettings();

        // Disable Player Controller 
        // This is done in order to fix the freezing when using the default fps controller
        playerController[currentControllerIndex].enabled = false;

        // Set start position to initial value
        playerController[currentControllerIndex].transform.position = LastPosition;

        // Set start rotation to initial value
        playerController[currentControllerIndex].transform.rotation = LastRotation;

        // Reactivate Player Controller in the next frame
        StartCoroutine(enablePC());
    }

    // Used to 
    private IEnumerator enablePC()
    {
        yield return new WaitForFixedUpdate();
        playerController[currentControllerIndex].enabled = true;
    }

}
