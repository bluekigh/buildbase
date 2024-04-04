using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	public GameObject circleCursor;

	// The world-position of the mouse last frame.
	Vector3 lastFramePosition;
	Vector3 currFramePosition;

	// The world-position start of our left-mouse drag operation
	Vector3 dragStartPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currFramePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		currFramePosition.z = 0;

		UpdateCursor();
		UpdateDragging();
		UpdateCameraMovement();

		// Save the mouse position from this frame
		// We don't use currFramePosition because we may have moved the camera.
		lastFramePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		lastFramePosition.z = 0;
	}

	void UpdateCursor() {
		// Update the circle cursor position
		Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
		if(tileUnderMouse != null) {
			circleCursor.SetActive(true);
			Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
		}
		else {
			// Mouse is outside of the valid tile space, so hide the cursor.
			circleCursor.SetActive(false);
		}
	}

	void UpdateDragging() {
		// Start Drag
		if( Input.GetMouseButtonDown(0) ) {
			dragStartPosition = currFramePosition;
		}
		
		// End Drag
		if( Input.GetMouseButtonUp(0) ) {
			int start_x = Mathf.FloorToInt( dragStartPosition.x );
			int end_x =   Mathf.FloorToInt( currFramePosition.x );
			int start_y = Mathf.FloorToInt( dragStartPosition.y );
			int end_y =   Mathf.FloorToInt( currFramePosition.y );
			
			// We may be dragging in the "wrong" direction, so flip things if needed.
			if(end_x < start_x) {
				int tmp = end_x;
				end_x = start_x;
				start_x = tmp;
			}
			if(end_y < start_y) {
				int tmp = end_y;
				end_y = start_y;
				start_y = tmp;
			}
			
			// Loop through all the tiles
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);
					if(t != null) {
						t.Type = Tile.TileType.Floor;
					}
				}
			}
		}
	}

	void UpdateCameraMovement() {
		// Handle screen panning
		if( Input.GetMouseButton(1) || Input.GetMouseButton(2) ) {	// Right or Middle Mouse Button
			
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate( diff );
			
		}
	}

}
