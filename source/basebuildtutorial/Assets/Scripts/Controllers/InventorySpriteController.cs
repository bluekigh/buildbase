using UnityEngine;
using System.Collections.Generic;

public class InventorySpriteController : MonoBehaviour {

	Dictionary<Inventory, GameObject> inventoryGameObjectMap;

	Dictionary<string, Sprite> inventorySprites;

	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	void Start () {
		LoadSprites();

		// Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
		inventoryGameObjectMap = new Dictionary<Inventory, GameObject>();

		// Register our callback so that our GameObject gets updated whenever
		// the tile's type changes.
		world.RegisterInventoryCreated(OnInventoryCreated);

		// Check for pre-existing inventory, which won't do the callback.
		foreach(string objectType in world.inventoryManager.inventories.Keys) {
			foreach(Inventory inv in world.inventoryManager.inventories[objectType]) {
				OnInventoryCreated(inv);
			}
		}


		//c.SetDestination( world.GetTileAt( world.Width/2 + 5, world.Height/2 ) );
	}

	void LoadSprites() {
		inventorySprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Inventory/");

		//Debug.Log("LOADED RESOURCE:");
		foreach(Sprite s in sprites) {
			//Debug.Log(s);
			inventorySprites[s.name] = s;
		}
	}

	public void OnInventoryCreated( Inventory inv ) {
		Debug.Log("OnInventoryCreated");
		// Create a visual GameObject linked to this data.

		// FIXME: Does not consider multi-tile objects nor rotated objects

		// This creates a new GameObject and adds it to our scene.
		GameObject inv_go = new GameObject();

		// Add our tile/GO pair to the dictionary.
		inventoryGameObjectMap.Add( inv, inv_go );

		inv_go.name = inv.objectType;
		inv_go.transform.position = new Vector3( inv.tile.X, inv.tile.Y, 0);
		inv_go.transform.SetParent(this.transform, true);

		SpriteRenderer sr = inv_go.AddComponent<SpriteRenderer>();
		sr.sprite = inventorySprites[ inv.objectType ];
		sr.sortingLayerName = "Inventory";

		// Register our callback so that our GameObject gets updated whenever
		// the object's into changes.
		// FIXME: Add on changed callbacks
		//inv.RegisterOnChangedCallback( OnCharacterChanged );

	}

	void OnInventoryChanged( Inventory inv ) {
		// FIXME:  Still needs to work!  And get called!

		//Debug.Log("OnFurnitureChanged");
		// Make sure the furniture's graphics are correct.

		if(inventoryGameObjectMap.ContainsKey(inv) == false) {
			Debug.LogError("OnCharacterChanged -- trying to change visuals for character not in our map.");
			return;
		}

		GameObject char_go = inventoryGameObjectMap[inv];
		//Debug.Log(furn_go);
		//Debug.Log(furn_go.GetComponent<SpriteRenderer>());

		//char_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

		char_go.transform.position = new Vector3( inv.tile.X, inv.tile.Y, 0);
	}


	
}
