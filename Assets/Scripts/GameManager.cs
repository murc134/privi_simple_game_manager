using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SharedPlayerControls[] PlayerControllerPrefabs;
    [SerializeField] // Remove Serialize Field
    private SharedPlayerControls[] PlayerController;


}
