﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour {

    public List<GameObject> levelPrefabs = new List<GameObject>();
    private List<GameObject> levels = new List<GameObject>();
    private GameObject activeLevel;
    private GameObject nextLevel;
    private Vector3 levelOrigin = new Vector3(0f, 0f, 0f);

    bool openDoors = false;
    bool doorsAnimating = false;
    bool isMoving = false;
    float maxElevatorRideDuration = 10f;
    float halfElevatorRideDuration;
    float timeLeftInMotion = 0f;

    // Use this for initialization
    void Start () {
        Messenger.AddListener("elevator-doors-opened", OpeningDoorComplete);

        halfElevatorRideDuration = maxElevatorRideDuration / 2f;

        foreach (GameObject levelPrefab in levelPrefabs)
        {
            GameObject level = (GameObject)Instantiate(levelPrefab, levelOrigin, Quaternion.identity);
            level.SetActive(false);
            levels.Add(level);
        }
    }


    void Update() {
        MoveElevator();
    }

    public void GoToLevel(int levelKey) {
        if (!isMoving)
        {
            Debug.Log("Go to: " + levelKey);
            GameObject requestedLevel = levels[levelKey - 1];
            bool isNotSameFloor = activeLevel && activeLevel != requestedLevel;
            if (!activeLevel || isNotSameFloor)
            {
                nextLevel = requestedLevel;
                StartMoving();
            }
        }        
    }

    void MoveElevator()
    {
        if (isMoving)
        {
            timeLeftInMotion -= Time.deltaTime;

            if (timeLeftInMotion <= halfElevatorRideDuration && nextLevel)
            {
                if (activeLevel){
                    activeLevel.SetActive(false);                    
                }
                activeLevel = nextLevel;
                nextLevel = null;
                activeLevel.SetActive(true);
                Debug.Log("Half way there");            
            }

            if (timeLeftInMotion <= 0f)
            {
                Debug.Log("Arrived");
                StopMoving();
            }
        }
    }

    void StartMoving() {
        timeLeftInMotion = maxElevatorRideDuration;
        isMoving = true;
        CloseDoors();
    }

    void StopMoving() {
        isMoving = false;
        timeLeftInMotion = 0f;
        OpenDoors();
    }

    public void ToggleDoors() {
        if (!doorsAnimating)
        {
            Debug.Log("Toggle Doors");
            openDoors = !openDoors;
            doorsAnimating = true;
            Messenger.Broadcast("open-elevator-doors");
        }
    }

    public void CloseDoors() {
        if (openDoors)
        {
            ToggleDoors();
        }
    }

    public void OpenDoors() {
        if (!openDoors)
        {
            ToggleDoors();
        }
    }

    void OpeningDoorComplete() {
        doorsAnimating = false;
        Debug.Log("Opening doors complete.");
    }
}

