﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RobotNetworked : NetworkBehaviour {

	public Camera cam1;
	public Camera cam2;
	public Camera cam3;

	public WheelCollider[] leftWheels;
	public WheelCollider[] rightWheels;
	public PlayerNetworked player;
	public GameObject midRail;
	public GameObject upperRail;
	public GameObject hug;
	public GameObject hugRightFinger;
	public GameObject hugLeftFinger;
	public GameObject hugBase;
	public GameObject hugBackboard;

	private double midRailMaxYIncrease = 16.95;
	private double midRailMinY;

	private double upperRailMaxYIncrease = 16.95;
	private double upperRailMinY;

	private double hugMaxYIncrease =  16.43;
	private double hugMinY;

	private float originalBaseX;
	private float baseXRetracted = 0.066f;

	private float originalRightFingerZ;
	private float originalLeftFingerZ;

	private float fingerOffset = 0.4f;

	private float originalBackboardX;
	private float backboardMaxXIncrease = 0.5f;
	private Vector3 startMeasure;
	// Use this for initialization
	void Start () {
		//if(!isLocalPlayer) return;
		midRailMinY = (double)midRail.transform.localPosition.y;
		hugMinY = (double)hug.transform.localPosition.y-1.1;
		originalBaseX = hugBase.transform.localPosition.x;
		originalRightFingerZ = hugRightFinger.transform.localPosition.z;
		originalLeftFingerZ = hugLeftFinger.transform.localPosition.z;
		originalBackboardX = hugBackboard.transform.localPosition.x;
		startMeasure = transform.position;
	}

	// Update is called once per frame
	void Update () {
		//if(!isLocalPlayer) return;
		float z = player.inputZ;
		float x = player.inputX;
		float left = 0;
		float right = 0;
		float speed = 10;
		if (player.robotControl) {
			left = Mathf.Clamp (z + x, -1.0f, 1.0f)*speed;
			right = Mathf.Clamp (z - x, -1.0f, 1.0f)*speed;

			double railSpeed = 0.1;
			if (Input.GetKey (KeyCode.Keypad1)) {
				if (upperRail.transform.localPosition.y > upperRailMinY) {

					upperRail.transform.localPosition = 
						new Vector3 (upperRail.transform.localPosition.x,
							(float)(upperRail.transform.localPosition.y - railSpeed),
							upperRail.transform.localPosition.z);

				}else if (midRail.transform.localPosition.y > midRailMinY) {

					midRail.transform.localPosition = 
						new Vector3 (midRail.transform.localPosition.x,
							(float)(midRail.transform.localPosition.y - railSpeed),
							midRail.transform.localPosition.z);

				}
			} else if (Input.GetKey (KeyCode.Keypad4)) {
				if (midRail.transform.localPosition.y < midRailMinY + midRailMaxYIncrease) {

					midRail.transform.localPosition = 
						new Vector3 (midRail.transform.localPosition.x,
							(float)(midRail.transform.localPosition.y + railSpeed),
							midRail.transform.localPosition.z);

				} else if (upperRail.transform.localPosition.y < upperRailMinY + upperRailMaxYIncrease) {

					upperRail.transform.localPosition = 
						new Vector3 (upperRail.transform.localPosition.x,
							(float)(upperRail.transform.localPosition.y + railSpeed),
							upperRail.transform.localPosition.z);

				}
			}

			double hugSpeed = 0.1;
			if (Input.GetKey (KeyCode.Keypad2) && hug.transform.localPosition.y > hugMinY) {

				hug.transform.localPosition = 
					new Vector3 (hug.transform.localPosition.x,
						(float)(hug.transform.localPosition.y - hugSpeed),
						hug.transform.localPosition.z);

			} else if (Input.GetKey (KeyCode.Keypad5) && hug.transform.localPosition.y < hugMinY + hugMaxYIncrease) {

				hug.transform.localPosition = 
					new Vector3 (hug.transform.localPosition.x,
						(float)(hug.transform.localPosition.y + hugSpeed),
						hug.transform.localPosition.z);

			}

			if (Input.GetKey (KeyCode.Keypad0)) {

				hugBase.transform.localPosition =
					new Vector3 (baseXRetracted,
						hugBase.transform.localPosition.y,
						hugBase.transform.localPosition.z);

			} else {

				hugBase.transform.localPosition =
					new Vector3 (originalBaseX,
						hugBase.transform.localPosition.y,
						hugBase.transform.localPosition.z);

			}
			if (Input.GetKey (KeyCode.KeypadEnter)) {
				startMeasure = transform.position;
			}
			if (Input.GetKey (KeyCode.KeypadPeriod)) {

				hugRightFinger.transform.localPosition = 
					new Vector3 (hugRightFinger.transform.localPosition.x,	
						hugRightFinger.transform.localPosition.y,
						(float)(originalRightFingerZ - fingerOffset));

				hugLeftFinger.transform.localPosition = 
					new Vector3 (hugRightFinger.transform.localPosition.x,	
						hugRightFinger.transform.localPosition.y,
						(float)(originalLeftFingerZ + fingerOffset));

			} else {

				hugRightFinger.transform.localPosition = 
					new Vector3 (hugRightFinger.transform.localPosition.x,	
						hugRightFinger.transform.localPosition.y,
						originalRightFingerZ);

				hugLeftFinger.transform.localPosition = 
					new Vector3 (hugRightFinger.transform.localPosition.x,	
						hugRightFinger.transform.localPosition.y,
						originalLeftFingerZ);

			}

			float backboardSpeed = 0.01f;
			if (Input.GetKey (KeyCode.Keypad3) && hugBackboard.transform.localPosition.x > originalBackboardX) {

				hugBackboard.transform.localPosition = 
					new Vector3 (hugBackboard.transform.localPosition.x - backboardSpeed,
						hugBackboard.transform.localPosition.y,
						hugBackboard.transform.localPosition.z);

			} else if (Input.GetKey (KeyCode.Keypad6) && hugBackboard.transform.localPosition.x < originalBackboardX + backboardMaxXIncrease) {

				hugBackboard.transform.localPosition = 
					new Vector3 (hugBackboard.transform.localPosition.x + backboardSpeed,
						hugBackboard.transform.localPosition.y,
						hugBackboard.transform.localPosition.z);

			}
		}


		for (int i = 0; i < leftWheels.Length; i++) {
			leftWheels [i].motorTorque = left;
			leftWheels [i].gameObject.transform.Rotate (0,leftWheels [i].rpm / 60 * 360 * Time.deltaTime, 0);
		}
		for (int i = 0; i < rightWheels.Length; i++) {
			rightWheels [i].motorTorque = right;
			rightWheels [i].gameObject.transform.Rotate (0,rightWheels [i].rpm / 60 * 360 * Time.deltaTime, 0);
		}
	}
	void OnGUI() {
		if(!isLocalPlayer) return;
		GUI.Label (new Rect (10, 10, 1000, 20), "DIST: " + Mathf.RoundToInt(Vector3.Distance (transform.position, startMeasure)*39.3701f) + "in" );
	}
}
