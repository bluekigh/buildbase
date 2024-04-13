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

}
