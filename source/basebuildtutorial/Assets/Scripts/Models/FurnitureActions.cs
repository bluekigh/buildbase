using UnityEngine;
using System.Collections;

public static class FurnitureActions {

	public static void Door_UpdateAction(Furniture furn, float deltaTime) {
		//Debug.Log("Door_UpdateAction: " + furn.furnParameters["openess"]);

		if(furn.furnParameters["is_opening"] >= 1) {
			furn.furnParameters["openess"] += deltaTime;
			if (furn.furnParameters["openess"] >= 1) {
				furn.furnParameters["is_opening"] = 0;
			}
		}
		else {
			furn.furnParameters["openess"] -= deltaTime;
		}

		furn.furnParameters["openess"] = Mathf.Clamp01(furn.furnParameters["openess"]);
	}

	public static ENTERABILITY Door_IsEnterable(Furniture furn) {
		Debug.Log("Door_IsEnterable");
		furn.furnParameters["is_opening"] = 1;

		if(furn.furnParameters["openess"] >= 1) {
			return ENTERABILITY.Yes;
		}

		return ENTERABILITY.Soon;
	}
}
