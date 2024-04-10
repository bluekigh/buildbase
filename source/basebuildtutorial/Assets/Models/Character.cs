using UnityEngine;
using System.Collections;

public class Character {
	public float X {
		get {
			return Mathf.Lerp( currTile.X, destTile.X, movementPercentage );
		}
	}

	public float Y {
		get {
			return Mathf.Lerp( currTile.Y, destTile.Y, movementPercentage );
		}
	}
		
	public Tile currTile {
		get; protected set;
	}


	Tile destTile;	// If we aren't moving, then destTile = currTile
	float movementPercentage; // Goes from 0 to 1 as we move from currTile to destTile

	float speed = 2f;	// Tiles per second

	public Character(Tile tile) {
		currTile = destTile = tile;
	}

	public void Update(float deltaTime) {
		// Are we there yet?
		if(currTile == destTile)
			return;

		// What's the total distance from point A to point B?
		float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X-destTile.X, 2) + Mathf.Pow(currTile.Y-destTile.Y, 2));

		// How much distance can be travel this Update?
		float distThisFrame = speed * deltaTime;

		// How much is that in terms of percentage to our destination?
		float percThisFrame = distToTravel / distToTravel;

		// Add that to overall percentage travelled.
		movementPercentage += percThisFrame;

		if(movementPercentage >= 1) {
			// We have reached our destination
			currTile = destTile;
			movementPercentage = 0;
			// FIXME?  Do we actually want to retain any overshot movement?
		}
	}

	public void SetDestination(Tile tile) {
		if(currTile.IsNeighbour(tile, true) == false) {
			Debug.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
		}

		destTile = tile;
	}
}
