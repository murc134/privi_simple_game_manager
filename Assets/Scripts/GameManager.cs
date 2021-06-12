using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text nextControllerNameUI;

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

    [SerializeField]
    private float maxFallingDist = -5;

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

            // Reparent transform of PlayerController to this transform
            playerController[i].transform.SetParent(transform);


        }
        // Set start position to initial value
        playerController[currentControllerIndex].transform.position = LastPosition;

        // Set start rotation to initial value
        playerController[currentControllerIndex].transform.rotation = LastRotation;

        // Update the text on the button so that is displays the name of the next controller
        updateNextControllerText();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            NextController();
        }
        if(ActiveController.transform.localPosition.y < maxFallingDist)
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
        updateNextControllerText();
    }

    private void updateNextControllerText()
    {
        if (nextControllerNameUI != null)
        {
            int nextIndex = validateControllerIndex(currentControllerIndex + 1);
            nextControllerNameUI.text = playerController[nextIndex].GetType().Name;
        }
        else
        {
            Debug.LogWarning("Cannot display next controller name because Text reference is not set", this);
        }
    }

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
        resetLastTransformSettings();

        // Set start position to initial value
        playerController[currentControllerIndex].transform.position = LastPosition;

        // Set start rotation to initial value
        playerController[currentControllerIndex].transform.rotation = LastRotation;

    }
}
