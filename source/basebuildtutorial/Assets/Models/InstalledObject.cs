//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

// InstalledObjects are things like walls, doors, and furniture (e.g. a sofa)

public class InstalledObject {

	// This represents the BASE tile of the object -- but in practice, large objects may actually occupy
	// multile tiles.
	public Tile tile {
		get; protected set;
	}

	// This "objectType" will be queried by the visual system to know what sprite to render for this object
	public string objectType {
		get; protected set;
	}

	// This is a multipler. So a value of "2" here, means you move twice as slowly (i.e. at half speed)
	// Tile types and other environmental effects may be combined.
	// For example, a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
	// would have a total movement cost of (2+3+3 = 8), so you'd move through this tile at 1/8th normal speed.
	// SPECIAL: If movementCost = 0, then this tile is impassible. (e.g. a wall).
	float movementCost; 

	// For example, a sofa might be 3x2 (actual graphics only appear to cover the 3x1 area, but the extra row is for leg room.)
	int width;
	int height;

	Action<InstalledObject> cbOnChanged;

	// TODO: Implement larger objects
	// TODO: Implement object rotation

	protected InstalledObject() {
		
	}

	static public InstalledObject CreatePrototype( string objectType, float movementCost = 1f, int width=1, int height=1 ) {
		InstalledObject obj = new InstalledObject();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	static public InstalledObject PlaceInstance( InstalledObject proto, Tile tile ) {
		InstalledObject obj = new InstalledObject();

		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;

		obj.tile = tile;

		// FIXME: This assumes we are 1x1!
		if( tile.PlaceObject(obj) == false ) {
			// For some reason, we weren't able to place our object in this tile.
			// (Probably it was already occupied.)

			// Do NOT return our newly instantiated object.
			// (It will be garbage collected.)
			return null;
		}

		return obj;
	}

	public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunc) {
		cbOnChanged += callbackFunc;
	}

	public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunc) {
		cbOnChanged -= callbackFunc;
	}


}
