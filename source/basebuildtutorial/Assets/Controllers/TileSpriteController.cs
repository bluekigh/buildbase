﻿//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TileSpriteController : MonoBehaviour {

	// The only tile sprite we have right now, so this
	// it a pretty simple way to handle it.
	public Sprite floorSprite;	// FIXME!
	public Sprite emptySprite;	// FIXME!

	Dictionary<Tile, GameObject> tileGameObjectMap;

	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	void Start () {
		// Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		// Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				// Get the tile data
				Tile tile_data = world.GetTileAt(x, y);

				// This creates a new GameObject and adds it to our scene.
				GameObject tile_go = new GameObject();

				// Add our tile/GO pair to the dictionary.
				tileGameObjectMap.Add( tile_data, tile_go );

				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);

				// Add a Sprite Renderer
				// Add a default sprite for empty tiles.
				SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer>();
				sr.sprite = emptySprite;
				sr.sortingLayerName = "Tiles";

			}
		}

		// Register our callback so that our GameObject gets updated whenever
		// the tile's type changes.
		world.RegisterTileChanged( OnTileChanged );
	}

	// THIS IS AN EXAMPLE -- NOT CURRENTLY USED (and probably out of date)
	void DestroyAllTileGameObjects() {
		// This function might get called when we are changing floors/levels.
		// We need to destroy all visual **GameObjects** -- but not the actual tile data!

		while(tileGameObjectMap.Count > 0) {
			Tile tile_data = tileGameObjectMap.Keys.First();
			GameObject tile_go = tileGameObjectMap[tile_data];

			// Remove the pair from the map
			tileGameObjectMap.Remove(tile_data);

			// Unregister the callback!
			tile_data.UnregisterTileTypeChangedCallback( OnTileChanged );

			// Destroy the visual GameObject
			Destroy( tile_go );
		}

		// Presumably, after this function gets called, we'd be calling another
		// function to build all the GameObjects for the tiles on the new floor/level
	}

	// This function should be called automatically whenever a tile's data gets changed.
	void OnTileChanged( Tile tile_data ) {

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



}
