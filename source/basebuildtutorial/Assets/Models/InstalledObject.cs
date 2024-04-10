//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;

// InstalledObjects are things like walls, doors, and furniture (e.g. a sofa)

public class InstalledObject {

	// This represents the BASE tile of the object -- but in practice, large objects may actually occupy
	// multile tiles.
	Tile tile;	

	// This "objectType" will be queried by the visual system to know what sprite to render for this object
	string objectType;

	// This is a multipler. So a value of "2" here, means you move twice as slowly (i.e. at half speed)
	// Tile types and other environmental effects may be combined.
	// For example, a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
	// would have a total movement cost of (2+3+3 = 8), so you'd move through this tile at 1/8th normal speed.
	// SPECIAL: If movementCost = 0, then this tile is impassible. (e.g. a wall).
	float movementCost; 

	// For example, a sofa might be 3x2 (actual graphics only appear to cover the 3x1 area, but the extra row is for leg room.)
	int width;
	int height;

	// This is basically used by our object factory to create the prototypical object
	// Note that it DOESN'T ask for a tile.
	public InstalledObject( string objectType, float movementCost = 1f, int width=1, int height=1 ) {
		this.objectType = objectType;
		this.movementCost = movementCost;
		this.width = width;
		this.height = height;
	}

	protected InstalledObject( InstalledObject proto, Tile tile ) {
		this.objectType = proto.objectType;
		this.movementCost = proto.movementCost;
		this.width = proto.width;
		this.height = proto.height;

		this.tile = tile;

		//tile.installedObject = this;
	}



}
