using UnityEngine;
using System.Collections.Generic;

public class Room {

	public float atmosO2  = 0;
	public float atmosN   = 0;
	public float atmosCO2 = 0;

	List<Tile> tiles;

	public Room() {
		tiles = new List<Tile>();
	}

	public void AssignTile( Tile t ) {
		if(tiles.Contains(t)) {
			return;
		}
			
		t.room = this;
		tiles.Add(t);
	}

	public void UnAssignAllTiles() {
		for (int i = 0; i < tiles.Count; i++) {
			tiles[i].room = tiles[i].world.GetOutsideRoom();	// Assign to outside
		}
		tiles = new List<Tile>();
	}

	public static void DoRoomFloodFill(Furniture sourceFurniture) {
		// sourceFurniture is the piece of furniture that may be
		// splitting two existing rooms, or may be the final 
		// enclosing piece to form a new room.
		// Check the NESW neighbours of the furniture's tile
		// and do flood fill from them

		World world = sourceFurniture.tile.world;

		// If this piece of furniture was added to an existing room
		// (which should always be true assuming with consider "outside" to be a big room)
		// delete that room and assign all tiles within to be "outside" for now
		if(sourceFurniture.tile.room != world.GetOutsideRoom()) {
			world.DeleteRoom(sourceFurniture.tile.room);	// This re-assigns tiles to the outside room.
		}

	}

}
