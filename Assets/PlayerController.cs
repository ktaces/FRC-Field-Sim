﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	public Camera cam;

	public Camera robotCam;

	private CharacterController controller;

	public float yaw = 0.0f;
	public float pitch = 0.0f;

	private RaycastHit hit;
	private GameObject pickedUpObject;

	public bool robotControl = false;

	public bool robotCamera = false;


	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		Cursor.lockState = CursorLockMode.Locked;
		float mouseSensitivity = 75.0f;
		float z = Input.GetAxis("Vertical") /** Time.deltaTime*/ * 5.0f;
		float x = Input.GetAxis("Horizontal") /** Time.deltaTime*/ * 5.0f;
		if(Input.GetKeyDown(KeyCode.Tab)){
			robotControl = !robotControl;
		}

		if(Input.GetKeyDown(KeyCode.C)){
			robotCamera = !robotCamera;
		}
		
		if(Input.GetKey(KeyCode.LeftControl)){
			x/=2;
			z/=2;
			mouseSensitivity/=2;
		}
		

		if (!robotControl) {
			float whee = 0;
			if (Input.GetKey (KeyCode.Space)) {
				whee = 2;
			}	
			controller.SimpleMove(transform.TransformDirection(new Vector3(x,whee,z)));
			if(Input.GetMouseButton(0)){
				if(pickedUpObject == null && Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,100)){
	        
		        	if(hit.collider.gameObject.tag=="Power Cube"){ //add collider reference otherwise you can't access gameObject!
		            
			            pickedUpObject=hit.collider.gameObject; 
			            hit.collider.gameObject.transform.SetParent(cam.transform,true); 
			            //hit.collider.gameObject.transform.position=cam.transform.position-cam.transform.forward; 
			            pickedUpObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		            }
		        }
			} else { //i think a regular else statement is fine
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
			controller.SimpleMove(transform.TransformDirection(new Vector3(0,0,0)));
		}
		
		if(!robotCamera){
			pitch += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
			yaw += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
			pitch = Mathf.Clamp(pitch,-90.0f,90.0f);
			//cam.transform.localEulerAngles = new Vector3(-pitch,0,0);
			
			cam.transform.localEulerAngles = new Vector3(-pitch,0.0f,0.0f);

			transform.localEulerAngles = new Vector3(0.0f,yaw-216,0.0f);
			robotCam.enabled = false;
			cam.enabled = true;
			
		}else{
			robotCam.enabled = true;
			cam.enabled = false;
		}	

		
	    

	}
	public Texture2D crosshairImage;
	void OnGUI() {
		if (!robotControl) {
			float xMin = (Screen.width / 2) - ((crosshairImage.width/4) / 2);
			float yMin = (Screen.height / 2) - ((crosshairImage.height/4) / 2);
	     	GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width/4, crosshairImage.height/4), crosshairImage);
		}
	}
}
