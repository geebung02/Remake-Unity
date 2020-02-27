﻿using System;
using System.Collections;
using System.Collections.Generic;
using SubterfugeCore.Core;
using SubterfugeCore.Core.Entities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SubManager : MonoBehaviour
{
    public Sub sub;

    // Start is called before the first frame update
    void Start()
    {
        // Update the position and rotation of the sub.
        Vector3 location = new Vector3(sub.getCurrentLocation().X, sub.getCurrentLocation().Y, 0);
        Transform transform = GetComponent<Transform>();
        transform.localPosition = location;
        transform.rotation = Quaternion.Euler(0, 0, (int)(sub.getRotation() * (360 / (2 * Math.PI))) - 135);
        
        // Set color based on the owner
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        int playerId = 0;
        if (sub.getOwner() != null)
        {
            playerId = sub.getOwner().getId();
        }
        switch (playerId)
        {
            case 0:
                renderer.color = Color.white;
                break;
            case 1:
                renderer.color = Color.red;
                break;
            case 2:
                renderer.color = Color.blue;
                break;
            case 3:
                renderer.color = Color.green;
                break;
            case 4:
                renderer.color = Color.cyan;
                break;
            case 5:
                renderer.color = Color.magenta;
                break;
            case 6:
                renderer.color = Color.yellow;
                break;
            case 7:
                renderer.color = Color.black;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (Game.timeMachine.getState().subExists(sub))
        {
            // Update the position and rotation of the sub.
            Vector3 location = new Vector3(sub.getCurrentLocation().X, sub.getCurrentLocation().Y, 0);
            Transform transform = GetComponent<Transform>();
            transform.localPosition = location;
            transform.rotation = Quaternion.Euler(0, 0, (int) (sub.getRotation() * (360 / (2 * Math.PI))) - 135);
        }
        else
        {
            if ( PrefabUtility.IsPartOfPrefabInstance(transform) )
            {
                //if a part of a prefab instance then get the instance handle
                Object prefabInstance = PrefabUtility.GetPrefabInstanceHandle(transform);
                //destroy the handle
                GameObject.DestroyImmediate(prefabInstance);
            }
            //the usual destroy immediate to clean up scene objects
            GameObject.DestroyImmediate(transform.gameObject,true);
        }
    }
}