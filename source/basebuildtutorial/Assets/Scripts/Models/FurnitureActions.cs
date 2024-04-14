using UnityEngine;
using System.Collections;

public static class FurnitureActions {
	// This file contains code which will likely be completely moved to
	// some LUA files later on and will be parsed at run-time.

	public static void Door_UpdateAction(Furniture furn, float deltaTime) {
		//Debug.Log("Door_UpdateAction: " + furn.furnParameters["openness"]);

		if(furn.GetParameter("is_opening") >= 1) {
			furn.ChangeParameter("openness", deltaTime * 4);	// FIXME: Maybe a door open speed parameter?
			if (furn.GetParameter("openness") >= 1) {
				furn.SetParameter("is_opening", 0);
			}
		}
		else {
			furn.ChangeParameter("openness", deltaTime * -4);
		}

		furn.SetParameter("openness", Mathf.Clamp01(furn.GetParameter("openness")) );

		if(furn.cbOnChanged != null) {
			furn.cbOnChanged(furn);
		}
	}

	public static ENTERABILITY Door_IsEnterable(Furniture furn) {
		//Debug.Log("Door_IsEnterable");
		furn.SetParameter("is_opening", 1);

		if(furn.GetParameter("openness") >= 1) {
			return ENTERABILITY.Yes;
		}

		return ENTERABILITY.Soon;
	}

	public static void JobComplete_FurnitureBuilding(Job theJob) {
		WorldController.Instance.world.PlaceFurniture( theJob.jobObjectType, theJob.tile );

		// FIXME: I don't like having to manually and explicitly set
		// flags that preven conflicts. It's too easy to forget to set/clear them!
		theJob.tile.pendingFurnitureJob = null;
	}

	public static void Stockpile_UpdateAction(Furniture furn, float deltaTime) {
		// We need to ensure that we have a job on the queue
		// asking for either:
		//  (if we are empty): That ANY loose inventory be brought to us.
		//  (if we have something): Then IF we are still below the max stack size,
		//						    that more of the same should be brought to us.



		if( furn.tile.inventory == null ) {
			Debug.Log("furn.tile.inventory == null");
			// We are empty -- just ask for ANYTHING to be brought here.
		
			// Do we already have a job?
			if(furn.JobCount() > 0) {
				return;
			}

			Job j = new Job (
				furn.tile,
				null, // ""
				null,
				0,
				new Inventory[1] { new Inventory("Steel Plate", 50, 0) }	// FIXME: Need to be able to indicate all/any type is okay
			);
			j.RegisterJobWorkedCallback(Stockpile_JobWorked);

			furn.AddJob( j );

		}
		else if ( furn.tile.inventory.stackSize < furn.tile.inventory.maxStackSize ) {
			// We have a stack of something started, but we're not full yet.
			// Do we already have a job?
			if(furn.JobCount() > 0) {
				return;
			}

			Inventory desInv = furn.tile.inventory.Clone();
			desInv.maxStackSize -= desInv.stackSize;
			desInv.stackSize = 0;

			Job j = new Job (
				furn.tile,
				null, // ""
				null,
				0,
				new Inventory[1] { desInv  }
			);
			j.RegisterJobWorkedCallback(Stockpile_JobWorked);

			furn.AddJob( j );
		}

	}

	static void Stockpile_JobWorked(Job j) {
		j.tile.furniture.RemoveJob(j);

		// TODO: Change this when we figure out what we're doing for the all/any pickup job.
		foreach(Inventory inv in j.inventoryRequirements.Values) {
			if(inv.stackSize > 0) {
				j.tile.world.inventoryManager.PlaceInventory(j.tile, inv);

				return;  // There should be no way that we ever end up with more than on inventory requirement with stackSize > 0
			}
		}
	}

}
