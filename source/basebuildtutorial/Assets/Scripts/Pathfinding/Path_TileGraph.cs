//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections.Generic;


public class Path_TileGraph {

	// This class constructs a simple path-finding compatible graph
	// of our world.  Each tile is a node. Each WALKABLE neighbour
	// from a tile is linked via an edge connection.

	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world) {

		Debug.Log("Path_TileGraph");

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

		Debug.Log("Path_TileGraph: Created "+nodes.Count+" nodes.");


		// Now loop through all nodes again
		// Create edges for neighbours

		int edgeCount = 0;

		foreach(Tile t in nodes.Keys) {
			Path_Node<Tile> n = nodes[t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

			// Get a list of neighbours for the tile
			Tile[] neighbours = t.GetNeighbours(true);	// NOTE: Some of the array spots could be null.

			// If neighbour is walkable, create an edge to the relevant node.
			for (int i = 0; i < neighbours.Length; i++) {
				if(neighbours[i] != null && neighbours[i].movementCost > 0) {
					// This neighbour exists and is walkable, so create an edge.
					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbours[i].movementCost;
					e.node = nodes[ neighbours[i] ];

					// Add the edge to our temporary (and growable!) list
					edges.Add(e);

					edgeCount++;
				}
			}

			n.edges = edges.ToArray();
		}

		Debug.Log("Path_TileGraph: Created "+edgeCount+" edges.");

	}

}
