﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject camera_node;

	private PolyWorldController _polyWorldController;

	void Start () {
		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();
	}

	void Update () {

		if (Input.GetMouseButton(1)) {
			
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			float rotate_scale = 10f;
			//			transform.Rotate(new Vector3(y*rotate_scale, -x*rotate_scale,0));

			camera_node.transform.RotateAround(transform.position, camera_node.transform.right, -y*rotate_scale);
			camera_node.transform.RotateAround(transform.position, Vector3.up, x*rotate_scale);
		}

		float scoll = Input.GetAxis ("Mouse ScrollWheel");
		float scoll_scale = 0.5f;
		if (scoll != 0f) {
			camera_node.transform.localPosition = camera_node.transform.localPosition + (camera_node.transform.forward) * camera_node.transform.localPosition.magnitude * scoll * scoll_scale;
		}

		if (Input.GetKeyDown ("f")) {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
//				transform.position = hit.point;
				GetComponent<TransformSmoother>().SetLocalPosition(hit.point, 0.3f);
			} else {
//				transform.position = _polyWorldController.GetCameraFocusPosition();
				GetComponent<TransformSmoother>().SetLocalPosition(_polyWorldController.GetCameraFocusPosition(), 0.3f);
			}
		}

		if (Input.GetMouseButton (2)) {
			Debug.Log("mouse middle");
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			float move_scale = -1f;

			transform.position = transform.position + camera_node.transform.right * x * move_scale + camera_node.transform.up * y * move_scale;
		}
	}

	public void OnSelectNewLayer()
	{
//		transform.position = _polyWorldController.GetCameraFocusPosition();
	}
	
}
