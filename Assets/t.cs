using UnityEngine;
using System.Collections;

public class t : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Debug.Log ("start");
		Texture2D levelBitmap = Resources.Load( "t0" ) as Texture2D;

		Debug.LogWarning( levelBitmap.GetPixel( 55 , 100 ).r );
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
