//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	// A two-dimensional array to hold our tile data.
	Tile[,] tiles;
	List<Character> characters;

	// The pathfinding graph used to navigate our world map.
	public Path_TileGraph tileGraph;

	Dictionary<string, Furniture> furniturePrototypes;

	// The tile width of the world.
	public int Width { get; protected set; }

	// The tile height of the world
	public int Height { get; protected set; }

	Action<Furniture> cbFurnitureCreated;
	Action<Character> cbCharacterCreated;
	Action<Tile> cbTileChanged;

	// TODO: Most likely this will be replaced with a dedicated
	// class for managing job queues (plural!) that might also
	// be semi-static or self initializing or some damn thing.
	// For now, this is just a PUBLIC member of World
	public JobQueue jobQueue;

	/// <summary>
	/// Initializes a new instance of the <see cref="World"/> class.
	/// </summary>
	/// <param name="width">Width in tiles.</param>
	/// <param name="height">Height in tiles.</param>
	public World(int width = 100, int height = 100) {
		jobQueue = new JobQueue();

		Width = width;
		Height = height;

		tiles = new Tile[Width,Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tiles[x,y] = new Tile(this, x, y);
				tiles[x,y].RegisterTileTypeChangedCallback( OnTileChanged );
			}
		}

		Debug.Log ("World created with " + (Width*Height) + " tiles.");

		CreateFurniturePrototypes();

		characters = new List<Character>();
	}

	public void Update(float deltaTime) {
		foreach(Character c in characters) {
			c.Update(deltaTime);
		}

	}

	public Character CreateCharacter( Tile t ) {
		Character c = new Character( t ); 

		characters.Add(c);

		if(cbCharacterCreated != null)
			cbCharacterCreated(c);

		return c;
	}

	void CreateFurniturePrototypes() {
		furniturePrototypes = new Dictionary<string, Furniture>();

		furniturePrototypes.Add("Wall", 
			Furniture.CreatePrototype(
								"Wall",
								0,	// Impassable
								1,  // Width
								1,  // Height
								true // Links to neighbours and "sort of" becomes part of a large object
							)
		);
	}

	/// <summary>
	/// A function for testing out the system
	/// </summary>
	public void RandomizeTiles() {
		Debug.Log ("RandomizeTiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {

				if(UnityEngine.Random.Range(0, 2) == 0) {
					tiles[x,y].Type = TileType.Empty;
				}
				else {
					tiles[x,y].Type = TileType.Floor;
				}

			}
		}
	}

	public void SetupPathfindingExample() {
		Debug.Log ("SetupPathfindingExample");

		// Make a set of floors/walls to test pathfinding with.

		int l = Width / 2 - 5;
		int b = Height / 2 - 5;

		for (int x = l-5; x < l + 15; x++) {
			for (int y = b-5; y < b + 15; y++) {
				tiles[x,y].Type = TileType.Floor;


				if(x == l || x == (l + 9) || y == b || y == (b + 9)) {
					if(x != (l + 9) && y != (b + 4)) {
						PlaceFurniture("Wall", tiles[x,y]);
					}
				}



			}
		}

	}

	/// <summary>
	/// Gets the tile data at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y) {
		if( x >= Width || x < 0 || y >= Height || y < 0) {
			//Debug.LogError("Tile ("+x+","+y+") is out of range.");
			return null;
		}
		return tiles[x, y];
	}


	public void PlaceFurniture(string objectType, Tile t) {
		//Debug.Log("PlaceInstalledObject");
		// TODO: This function assumes 1x1 tiles -- change this later!

		if( furniturePrototypes.ContainsKey(objectType) == false ) {
			Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
			return;
		}

		Furniture obj = Furniture.PlaceInstance( furniturePrototypes[objectType], t);

		if(obj == null) {
			// Failed to place object -- most likely there was already something there.
			return;
		}

		if(cbFurnitureCreated != null) {
			cbFurnitureCreated(obj);
			InvalidateTileGraph();
		}
	}

	public void RegisterFurnitureCreated(Action<Furniture> callbackfunc) {
		cbFurnitureCreated += callbackfunc;
	}

	public void UnregisterFurnitureCreated(Action<Furniture> callbackfunc) {
		cbFurnitureCreated -= callbackfunc;
	}

	public void RegisterCharacterCreated(Action<Character> callbackfunc) {
		cbCharacterCreated += callbackfunc;
	}

	public void UnregisterCharacterCreated(Action<Character> callbackfunc) {
		cbCharacterCreated -= callbackfunc;
	}

	public void RegisterTileChanged(Action<Tile> callbackfunc) {
		cbTileChanged += callbackfunc;
	}

	public void UnregisterTileChanged(Action<Tile> callbackfunc) {
		cbTileChanged -= callbackfunc;
	}

	// Gets called whenever ANY tile changes
	void OnTileChanged(Tile t) {
		if(cbTileChanged == null)
			return;
		
		cbTileChanged(t);

		InvalidateTileGraph();
	}

	// This should be called whenever a change to the world
	// means that our old pathfinding info is invalid.
	public void InvalidateTileGraph() {
		tileGraph = null;
	}

	public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
		return furniturePrototypes[furnitureType].IsValidPosition(t);
	}

	public Furniture GetFurniturePrototype(string objectType) {
		if(furniturePrototypes.ContainsKey(objectType) == false) {
			Debug.LogError("No furniture with type: " + objectType);
			return null;
		}

		return furniturePrototypes[objectType];
	}
}
