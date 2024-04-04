//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

public class Tile {

	// TileType is the base type of the tile. In some tile-based games, that might be
	// the terrain type. For us, we only need to differentiate between empty space
	// and floor (a.k.a. the station structure/scaffold). Walls/Doors/etc... will be
	// InstalledObjects sitting on top of the floor.
	public enum TileType { Empty, Floor };

	private TileType _type = TileType.Empty;
	public TileType Type {
		get { return _type; }
		set {
			TileType oldType = _type;
			_type = value;
			// Call the callback and let things know we've changed.

			if(cbTileTypeChanged != null && oldType != _type)
				cbTileTypeChanged(this);
		}
	}

	// LooseObject is something like a drill or a stack of metal sitting on the floor
	LooseObject looseObject;

	// InstalledObject is something like a wall, door, or sofa.
	InstalledObject installedObject;

	// We need to know the context in which we exist. Probably. Maybe.
	World world;
	public int X { get; protected set; }
	public int Y { get; protected set; }

	// The function we callback any time our type changes
	Action<Tile> cbTileTypeChanged;

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
		cbTileTypeChanged += callback;
	}
	
	/// <summary>
	/// Unregister a callback.
	/// </summary>
	public void UnegisterTileTypeChangedCallback(Action<Tile> callback) {
		cbTileTypeChanged -= callback;
	}
	
}
