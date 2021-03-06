﻿using System;
using System.Collections;
using System.Collections.Generic;
using SubterfugeCore.Core;
using SubterfugeCore.Core.Entities;
using SubterfugeCore.Core.Entities.Positions;
using SubterfugeCore.Core.GameEvents;
using SubterfugeCore.Core.GameEvents.Base;
using SubterfugeCore.Core.Network;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool showLaunchHud = false;
    public GameObject launchHud;
    public Outpost launchOutpost;
    public Outpost destinationOutpost;
    public Slider drillerSlider;
    public Api api = new Api();
    
    // Start is called before the first frame update
    async void Start()
    {
        launchHud.SetActive(false);
        List<GameEvent> gameEvents = await api.GetGameEvents(ApplicationState.currentGameRoom.RoomId);
        
        // Parse game events here.
        foreach(GameEvent gameEvent in gameEvents)
        {
            Game.TimeMachine.AddEvent(gameEvent);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the pressed location was an outpost. If it was, the user is trying to launch a sub.
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "Outpost")
            {
                // Clicked object is an outpost, don't move the camera.
                launchOutpost = hit.collider.gameObject.GetComponent<OutpostManager>().outpost;
                return;
            }   
        }
        // If the mouse button is released, apply velocity to the map to scroll
        if (Input.GetMouseButtonUp(0))
        {
            // If the first click was on an outpost, check if the second is on another outpost for a launch.
            if (launchOutpost != null && showLaunchHud == false)
            {
                // Check if the pressed location was an outpost. If it was, the user is trying to launch a sub.
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.tag == "Outpost")
                {
                    // Clicked object is an outpost, don't move the camera.
                    destinationOutpost = hit.collider.gameObject.GetComponent<OutpostManager>().outpost;
                    
                    // only show the hud if the souce outpost is owned by the current player & the destination is not the source.
                    if (launchOutpost != destinationOutpost &&
                        launchOutpost.GetOwner().GetId() == ApplicationState.player.GetId())
                    {

                        SouceLaunchInformation sourcePanel = launchHud.GetComponentInChildren<SouceLaunchInformation>();
                        sourcePanel.source = launchOutpost;
                        SubLaunchInformation informationPanel = launchHud.GetComponentInChildren<SubLaunchInformation>();
                        informationPanel.destination = destinationOutpost;
                        informationPanel.sourceOutpost = launchOutpost;
                        drillerSlider.maxValue = launchOutpost.GetDrillerCount();

                        this.SetLaunchHub(true);
                    }
                    else
                    {
                        launchOutpost = null;
                    }
                }
            } else if (showLaunchHud)
            {
                // Determine if the click was in the panel
                if (EventSystem.current.IsPointerOverGameObject()) return;
                this.SetLaunchHub(false);
            }
        }
    }

    public void AdvanceTimemachine(int ticks)
    {
        Game.TimeMachine.Advance(ticks);
    }

    public void SetLaunchHub(bool state)
    {
        showLaunchHud = state;
        launchHud.SetActive(state);
    }

    public void launchSub()
    {
        LaunchEvent launchEvent = new LaunchEvent(Game.TimeMachine.CurrentTick, launchOutpost, (int)drillerSlider.value, destinationOutpost);
        Game.TimeMachine.AddEvent(launchEvent);
        api.SubmitGameEvent(launchEvent, ApplicationState.currentGameRoom.RoomId);
        this.SetLaunchHub(false);
    }
    
    
}
