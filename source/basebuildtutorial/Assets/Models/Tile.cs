//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

// TileType is the base type of the tile. In some tile-based games, that might be
// the terrain type. For us, we only need to differentiate between empty space
// and floor (a.k.a. the station structure/scaffold). Walls/Doors/etc... will be
// InstalledObjects sitting on top of the floor.
public enum TileType { Empty, Floor };

public class Tile {
	private TileType _type = TileType.Empty;
	public TileType Type {
		get { return _type; }
		set {
			TileType oldType = _type;
			_type = value;
			// Call the callback and let things know we've changed.

			if(cbTileChanged != null && oldType != _type) {
				cbTileChanged(this);
			}
		}
	}

	// LooseObject is something like a drill or a stack of metal sitting on the floor
	Inventory inventory;

	// Furniture is something like a wall, door, or sofa.
	public Furniture furniture {
		get; protected set;
	}

	// FIXME: This seems like a terrible way to flag if a job is pending
	// on a tile.  This is going to be prone to errors in set/clear.
	public Job pendingFurnitureJob;

	// We need to know the context in which we exist. Probably. Maybe.
	public World world { get; protected set; }

	public int X { get; protected set; }
	public int Y { get; protected set; }

	// The function we callback any time our tile's data changes
	Action<Tile> cbTileChanged;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">A World instance.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile( World world, int x, int y ) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	/// <summary>
	/// Register a function to be called back when our tile type changes.
	/// </summary>
	public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
		cbTileChanged += callback;
	}
	
	/// <summary>
	/// Unregister a callback.
	/// </summary>
	public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
		cbTileChanged -= callback;
	}

	public bool PlaceFurniture(Furniture objInstance) {
		if(objInstance == null) {
			// We are uninstalling whatever was here before.
			furniture = null;
			return true;
		}

		// objInstance isn't null

		if(furniture != null) {
			Debug.LogError("Trying to assign a furniture to a tile that already has one!");
			return false;
		}

		// At this point, everything's fine!

		furniture = objInstance;
		return true;
	}

	// Tells us if two tiles are adjacent.
	public bool IsNeighbour(Tile tile, bool diagOkay = false) {


		// Check to see if we have a difference of exactly ONE between the two
		// tile coordinates.  Is so, then we are vertical or horizontal neighbours.
		return 
			Mathf.Abs( this.X - tile.X ) + Mathf.Abs( this.Y - tile.Y ) == 1 ||  // Check hori/vert adjacency
			( diagOkay && ( Mathf.Abs( this.X - tile.X ) == 1 && Mathf.Abs( this.Y - tile.Y ) == 1 ) ) // Check diag adjacency
			;







/*		// First, are we on the same X column?  If so, see if we differ
		// in our Y by exactly one.
		if(this.X == tile.X && ( Mathf.Abs( this.Y - tile.Y ) == 1 ) )
			return true;

		// Now check on the same Y row...
		if(this.Y == tile.Y && ( Mathf.Abs( this.X - tile.X ) == 1 ) )
			return true;

		if(diagOkay) {
			if(this.X == tile.X+1 && (this.Y == tile.Y+1 || this.Y == tile.Y-1 ))
				return true;
			if(this.X == tile.X-1 && (this.Y == tile.Y+1 || this.Y == tile.Y-1 ))
				return true;
		}

		return false;
*/	}

}
