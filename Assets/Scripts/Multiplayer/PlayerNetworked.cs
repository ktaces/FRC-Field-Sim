﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworked : NetworkBehaviour {

	public Transform robotPrefab;

	public RobotNetworked robot;

	public RobotNetworkManager networkManager;

	public Camera[] cameras;
	private int cameraIndex = 0;
	private CharacterController controller;

	private float startingRot;

	public float yaw = 0.0f;
	public float pitch = 0.0f;

	private RaycastHit hit;
	private GameObject pickedUpObject;

	public bool robotControl = false;

	public bool robotCamera = false;

	public float inputZ = 0.0f;
	public float inputX = 0.0f;
	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
			return;
		controller = GetComponent<CharacterController>();
		startingRot = transform.localEulerAngles.y;
		Cursor.lockState = CursorLockMode.Locked;
		robot = Instantiate (this.robotPrefab, this.transform.TransformDirection(new Vector3 (0f, 0.17f, 3f)), Quaternion.identity).GetComponent<RobotNetworked>();
		robot.player = this;
		cameras [1] = robot.cam1;
		cameras [2] = robot.cam2;
		cameras [3] = robot.cam3;


	}

	private bool hasDone = false;

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			for(var i = 0; i < cameras.Length; i++){
				cameras[i].enabled = false;
			}
			return;
		}
		Cursor.lockState = CursorLockMode.Locked;
		float mouseSensitivity = 75.0f;
		inputZ = Input.GetAxis ("Vertical");
		inputX = Input.GetAxis ("Horizontal");
		float z = inputZ /** Time.deltaTime*/ * 5.0f;
		float x = inputX /** Time.deltaTime*/ * 5.0f;

		if(Input.GetKeyDown(KeyCode.Tab)){
			robotControl = !robotControl;
		}

		if(Input.GetKeyDown(KeyCode.C)){
			cameraIndex++;
			cameraIndex = cameraIndex % cameras.Length;
		}
		
		if(Input.GetKey(KeyCode.LeftControl)){
			x/=2;
			z/=2;
			mouseSensitivity/=2;
		}
		

		if (!robotControl) {
			float whee = 0f;
			if (Input.GetKey (KeyCode.Space)) {
				whee = 2f;
			}	
			if(!hasDone){
				controller.SimpleMove(Vector3.zero);
				hasDone = true;
			}else{
				Vector3 direction = transform.TransformDirection(new Vector3(x,whee,z));
				controller.SimpleMove(direction);
			}
			if(Input.GetMouseButton(0)){
				Camera cam = cameras[0];
				if(pickedUpObject == null && Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,100)){
	        
		        	if(hit.collider.gameObject.tag=="Power Cube"){ //add collider reference otherwise you can't access gameObject!
		            
			            pickedUpObject=hit.collider.gameObject; 
			            hit.collider.gameObject.transform.SetParent(cam.transform,true); 
			            //hit.collider.gameObject.transform.position=cam.transform.position-cam.transform.forward; 
			            pickedUpObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		            }
		        }
			} else {
				if(pickedUpObject != null){
					Vector3 tempPos = pickedUpObject.transform.position;
					Quaternion tempRot = pickedUpObject.transform.rotation;
		    		pickedUpObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			        pickedUpObject.transform.SetParent(null,false);
			        pickedUpObject.transform.position = tempPos;
			        pickedUpObject.transform.rotation = tempRot;
			        pickedUpObject=null;
	        	}
	        }

		}else{
			controller.SimpleMove(Vector3.zero);
		}

		if(cameraIndex == 0){
			pitch += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
			yaw += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
			pitch = Mathf.Clamp(pitch,-90.0f,90.0f);
			//cam.transform.localEulerAngles = new Vector3(-pitch,0,0)
			cameras[0].transform.localEulerAngles = new Vector3(-pitch,0.0f,0.0f);

			transform.localEulerAngles = new Vector3(0.0f,yaw+startingRot,0.0f);
			
		}

		for(var i = 0; i < cameras.Length; i++){
			cameras[i].enabled = (i == cameraIndex);
		}

		
	    

	}
	public Texture2D crosshairImage;
	void OnGUI() {
		if(!isLocalPlayer) return;
		if (!robotControl) {
			float xMin = (Screen.width / 2) - ((crosshairImage.width/4) / 2);
			float yMin = (Screen.height / 2) - ((crosshairImage.height/4) / 2);
	     	GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width/4, crosshairImage.height/4), crosshairImage);
		}
	}
}
