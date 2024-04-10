//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	// The only tile sprite we have right now, so this
	// it a pretty simple way to handle it.
	public Sprite floorSprite;	// FIXME!
	public Sprite wallSprite;	// FIXME!

	Dictionary<Tile, GameObject> tileGameObjectMap;
	Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

	// The world and tile data
	public World World { get; protected set; }

	// Use this for initialization
	void Start () {
		if(Instance != null) {
			Debug.LogError("There should never be two world controllers.");
		}
		Instance = this;

		// Create a world with Empty tiles
		World = new World();

		World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

		// Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

		// Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				// Get the tile data
				Tile tile_data = World.GetTileAt(x, y);

				// This creates a new GameObject and adds it to our scene.
				GameObject tile_go = new GameObject();

				// Add our tile/GO pair to the dictionary.
				tileGameObjectMap.Add( tile_data, tile_go );

				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);

				// Add a sprite renderer, but don't bother setting a sprite
				// because all the tiles are empty right now.
				tile_go.AddComponent<SpriteRenderer>();

				// Register our callback so that our GameObject gets updated whenever
				// the tile's type changes.
				tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged );
			}
		}

		// Shake things up, for testing.
		World.RandomizeTiles();
	}

	// Update is called once per frame
	void Update () {

	}

	// THIS IS AN EXAMPLE -- NOT CURRENTLY USED
	void DestroyAllTileGameObjects() {
		// This function might get called when we are changing floors/levels.
		// We need to destroy all visual **GameObjects** -- but not the actual tile data!

		while(tileGameObjectMap.Count > 0) {
			Tile tile_data = tileGameObjectMap.Keys.First();
			GameObject tile_go = tileGameObjectMap[tile_data];

			// Remove the pair from the map
			tileGameObjectMap.Remove(tile_data);

			// Unregister the callback!
			tile_data.UnregisterTileTypeChangedCallback( OnTileTypeChanged );

			// Destroy the visual GameObject
			Destroy( tile_go );
		}

		// Presumably, after this function gets called, we'd be calling another
		// function to build all the GameObjects for the tiles on the new floor/level
	}

	// This function should be called automatically whenever a tile's type gets changed.
	void OnTileTypeChanged( Tile tile_data ) {

		if(tileGameObjectMap.ContainsKey(tile_data) == false) {
			Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		GameObject tile_go = tileGameObjectMap[tile_data];

		if(tile_go == null) {
			Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		if(tile_data.Type == TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
		}
		else if( tile_data.Type == TileType.Empty ) {
			tile_go.GetComponent<SpriteRenderer>().sprite = null;
		}
		else {
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
		}


	}

	/// <summary>
	/// Gets the tile at the unity-space coordinates
	/// </summary>
	/// <returns>The tile at world coordinate.</returns>
	/// <param name="coord">Unity World-Space coordinates.</param>
	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);
		
		return World.GetTileAt(x, y);
	}

	public void OnInstalledObjectCreated( InstalledObject obj ) {
		//Debug.Log("OnInstalledObjectCreated");
		// Create a visual GameObject linked to this data.

		// FIXME: Does not consider multi-tile objects nor rotated objects

		// This creates a new GameObject and adds it to our scene.
		GameObject obj_go = new GameObject();

		// Add our tile/GO pair to the dictionary.
		installedObjectGameObjectMap.Add( obj, obj_go );

		obj_go.name = obj.objectType + "_" + obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3( obj.tile.X, obj.tile.Y, 0);
		obj_go.transform.SetParent(this.transform, true);

		// FIXME: We assume that the object must be a wall, so use
		// the hardcoded reference to the wall sprite.
		obj_go.AddComponent<SpriteRenderer>().sprite = wallSprite;	// FIXME

		// Register our callback so that our GameObject gets updated whenever
		// the object's into changes.
		obj.RegisterOnChangedCallback( OnInstalledObjectChanged );

	}

	void OnInstalledObjectChanged( InstalledObject obj ) {
		Debug.LogError("OnInstalledObjectChanged -- NOT IMPLEMENTED");
	}

}
