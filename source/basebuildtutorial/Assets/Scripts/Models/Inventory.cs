//=======================================================================
// Copyright Martin "quill18" Glaude 2015-2016.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;

// Inventory are things that are lying on the floor/stockpile, like a bunch of metal bars
// or potentially a non-installed copy of furniture (e.g. a cabinet still in the box from Ikea)


public class Inventory {
	public string objectType = "Steel Plate";
	public int maxStackSize = 50;
	public int stackSize = 1;

	public Tile tile;
	public Character character;

	public Inventory() {
		
	}

	public Inventory(string objectType, int maxStackSize, int stackSize) {
		this.objectType   = objectType;
		this.maxStackSize = maxStackSize;
		this.stackSize    = stackSize;
	}

	protected Inventory(Inventory other) {
		objectType   = other.objectType;
		maxStackSize = other.maxStackSize;
		stackSize    = other.stackSize;
	}

	public virtual Inventory Clone() {
		return new Inventory(this);
	}

}
