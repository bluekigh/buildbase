using UnityEngine;
using System.Collections.Generic;


public class Path_TileGraph {

	// This class constructs a simple path-finding compatible graph
	// of our world.  Each tile is a node. Each WALKABLE neighbour
	// from a tile is linked via an edge connection.

	Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world) {

		// Loop through all tiles of the world
		// For each tile, create a node
		//  Do we create nodes for non-floor tiles?  NO!
		//  Do we create nodes for tiles that are completely unwalkable (i.e. walls)?  NO!

		nodes = new Dictionary<Tile, Path_Node<Tile>>();

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {

				Tile t = world.GetTileAt(x,y);

				if(t.movementCost > 0) {	// Tiles with a move cost of 0 are unwalkable
					Path_Node<Tile> n = new Path_Node<Tile>();
					n.data = t;
					nodes.Add(t, n);
				}

			}
		}


		// Now loop through all nodes again
		// Create edges for neighbours

		foreach(Tile t in nodes.Keys) {

			// Get a list of neighbours for the tile
			// If neighbour is walkable, create an edge to the relevant node.

		}

	}

}
