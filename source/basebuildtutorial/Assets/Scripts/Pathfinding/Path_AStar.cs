﻿using UnityEngine;
using System.Collections.Generic;

public class Path_AStar {

	Queue<Tile> path;

	public Path_AStar(World world, Tile start, Tile end) {



	}

	public Tile GetNextTile() {
		return path.Dequeue();
	}

}
