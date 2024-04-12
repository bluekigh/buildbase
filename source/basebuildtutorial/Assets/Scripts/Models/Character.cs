using UnityEngine;
using System.Collections;
using System;

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

	Action<Character> cbCharacterChanged;

	Job myJob;

	public Character(Tile tile) {
		currTile = destTile = tile;
	}

	public void Update(float deltaTime) {
		//Debug.Log("Character Update");


		// Do I have a job?
		if(myJob == null) {
			// Grab a new job.
			myJob = currTile.world.jobQueue.Dequeue();

			if(myJob != null) {
				// We have a job!
				destTile = myJob.tile;
				myJob.RegisterJobCompleteCallback(OnJobEnded);
				myJob.RegisterJobCancelCallback(OnJobEnded);
			}
		}

		// Are we there yet?
		if(currTile == destTile) {
			if(myJob != null) {
				myJob.DoWork(deltaTime);
			}

			return;
		}

		// What's the total distance from point A to point B?
		// We are going to use Euclidean distance FOR NOW...
		// But when we do the pathfinding system, we'll likely
		// switch to something like Manhattan or Chebyshev distance
		float distToTravel = Mathf.Sqrt(
			Mathf.Pow(currTile.X-destTile.X, 2) + 
			Mathf.Pow(currTile.Y-destTile.Y, 2)
		);


		// How much distance can be travel this Update?
		float distThisFrame = speed * deltaTime;

		// How much is that in terms of percentage to our destination?
		float percThisFrame = distThisFrame / distToTravel;

		// Add that to overall percentage travelled.
		movementPercentage += percThisFrame;

		if(movementPercentage >= 1) {
			// We have reached our destination

			// TODO: Get the next tile from the pathfinding system.
			//       If there are no more tiles, then we have TRULY
			//       reached our destination.

			currTile = destTile;
			movementPercentage = 0;
			// FIXME?  Do we actually want to retain any overshot movement?
		}

		if(cbCharacterChanged != null)
			cbCharacterChanged(this);
	}

	public void SetDestination(Tile tile) {
		if(currTile.IsNeighbour(tile, true) == false) {
			Debug.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
		}

		destTile = tile;
	}

	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}

	void OnJobEnded(Job j) {
		// Job completed or was cancelled.

		if(j != myJob) {
			Debug.LogError("Character being told about job that isn't his. You forgot to unregister something.");
			return;
		}

		myJob = null;
	}
}
