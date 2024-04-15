using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FurnitureBuildMenu : MonoBehaviour {

	public GameObject buildFurnitureButtonPrefab;

	// Use this for initialization
	void Start () {

		BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();
	
		// For each furniture prototype in our world, create one instance
		// of the button to be clicked!


		foreach( string s in World.current.furniturePrototypes.Keys ) {
			GameObject go = (GameObject)Instantiate(buildFurnitureButtonPrefab);
			go.transform.SetParent(this.transform);

			go.name = "Button - Build " + s;

			go.transform.GetComponentInChildren<Text>().text = "Build " + s;

			Button b = go.GetComponent<Button>();

			string objectId = s;
			b.onClick.AddListener( delegate { bmc.SetMode_BuildFurniture(objectId); } );

		}

	}
	
}
