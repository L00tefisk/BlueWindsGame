﻿using UnityEngine;
using System.Collections;

public class CameraFollow2 : MonoBehaviour 
{

	public float xSmooth = 10f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 10f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public float speedFactor = 10f;
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.
	public bool facingRight;

	private Transform player;		// Reference to the player's transform.


	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag("Player").transform;

	}
	void FixedUpdate ()
	{
		TrackPlayer();
	}
	

	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		//Debug.Log("X: "+player.position.x+", Y: "+player.position.x);

			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
		targetX = Mathf.Lerp(
			transform.position.x, 
			player.position.x+Mathf.Clamp(player.rigidbody2D.velocity.x*(speedFactor*0.1f), -7f, 7f), 
			xSmooth * Time.deltaTime
			);
		Debug.Log("Vel: "+player.rigidbody2D.velocity.x);
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
		targetY = Mathf.Lerp(
			transform.position.y, 
			2f + player.position.y+Mathf.Clamp(player.rigidbody2D.velocity.y*(speedFactor*0.05f),-1000, 0), 
			ySmooth * Time.deltaTime
			);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		//targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		//targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

}