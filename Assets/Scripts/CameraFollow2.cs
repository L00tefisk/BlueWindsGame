using UnityEngine;
using System.Collections;

public class CameraFollow2 : MonoBehaviour 
{
	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 4f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 4f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.
	public bool facingRight = false;
	public float cameraOffset = -6;

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
		//if (CheckXMargin()) 
		//Debug.Log("Vel: "+Mathf.RoundToInt(player.rigidbody2D.velocity.x) + ", FR = "+facingRight + ", CO = "+Mathf.Abs(player.rigidbody2D.velocity.x/2) * Time.deltaTime);
			if((player.rigidbody2D.velocity.x > 0 && !facingRight) || (player.rigidbody2D.velocity.x < 0 && facingRight))  {
			facingRight = !facingRight;
			cameraOffset *= -1; 
			}
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
		targetX = Mathf.Lerp(
			transform.position.x, 
			cameraOffset+player.position.x
			//+Mathf.Clamp(player.rigidbody2D.velocity.x*(speedFactor*0.1f), -6f, 6f)
			,(Mathf.Abs(player.rigidbody2D.velocity.x/4)+1.5f) * Time.deltaTime
			);


			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
		targetY = Mathf.Lerp(
			transform.position.y, 
			2f + player.position.y+Mathf.Clamp(player.rigidbody2D.velocity.y*(ySmooth*0.05f),-10, 0), 
			ySmooth * Time.deltaTime
			);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		//targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		//targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}
	
	
	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}
}
