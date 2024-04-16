//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	// The world and tile data
	public World world { get; protected set; }

	static bool loadWorld = false;

	public GameObject saveFileDialogBox;

	// Use this for initialization
	void OnEnable () {
		if(Instance != null) {
			Debug.LogError("There should never be two world controllers.");
		}
		Instance = this;

		if(loadWorld) {
			loadWorld = false;
			CreateWorldFromSaveFile();
		}
		else {
			CreateEmptyWorld();
		}
	}

	void Update() {
		// TODO: Add pause/unpause, speed controls, etc...
		world.Update(Time.deltaTime);

	}
		
	/// <summary>
	/// Gets the tile at the unity-space coordinates
	/// </summary>
	/// <returns>The tile at world coordinate.</returns>
	/// <param name="coord">Unity World-Space coordinates.</param>
	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x + 0.5f);
		int y = Mathf.FloorToInt(coord.y + 0.5f);
		
		return world.GetTileAt(x, y);
	}

	public void NewWorld() {
		Debug.Log("NewWorld button was clicked.");

		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}

	public void ShowSaveDialog() {
		// When the "Save" button gets clicked, we should
		// show the user a file dialog box asking for a filename
		// to save the game to.  The user can click on an existing
		// file to overwrite it.

		// When the save dialog box is closed with the Save / OK button,
		// do the actual save.  The user can also close / cancel the
		// dialog box in which case we do nothing.

		saveFileDialogBox.SetActive(true);
	}

	string FileSaveBasePath() {
		return 	System.IO.Path.Combine( Application.persistentDataPath, "Saves" );

	}

	public void SaveDialogOkayWasClicked() {
		// TODO:
		// check to see if the file already exists
		// if so, ask for overwrite confirmation.

		string fileName = saveFileDialogBox.GetComponentInChildren<InputField>().text;

		// TODO: Is the filename valid?  I.E. we may want to ban path-delimiters (/ \ or :) and 
		// maybe periods?      ../../some_important_file

		// Right now fileName is just what was in the dialog box.  We need to pad this out to the full
		// path, plus an extension!
		// In the end, we're looking for something that's going to be similar to this (depending on OS)
		//    C:\Users\Quill18\ApplicationData\MyCompanyName\MyGameName\Saves\SaveGameName123.sav

		// Application.persistentDataPath == C:\Users\<username>\ApplicationData\MyCompanyName\MyGameName\

		string filePath = System.IO.Path.Combine( FileSaveBasePath(), fileName + ".sav" );

		// At this point, filePath should look very much like
		//     C:\Users\Quill18\ApplicationData\MyCompanyName\MyGameName\Saves\SaveGameName123.sav

		if(File.Exists(filePath) == true) {
			// TODO: Do file overwrite dialog box.
			return;
		}

		saveFileDialogBox.SetActive(false);

		SaveWorld(filePath);
	}

	public void SaveWorld(string filePath) {
		// This function gets called when the user confirms a filename
		// from the save dialog box.

		// Get the file name from the save file dialog box

		Debug.Log("SaveWorld button was clicked.");

		XmlSerializer serializer = new XmlSerializer( typeof(World) );
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, world);
		writer.Close();

		Debug.Log( writer.ToString() );

		//PlayerPrefs.SetString("SaveGame00", writer.ToString());

		// Create/overwrite the save file with the xml text.

		// Make sure the save folder exists.
		if( Directory.Exists( FileSaveBasePath() ) == false ) {
			// NOTE: This can throw an exception if we can't create the folder,
			// but why would this ever happen? We should, by definition, have the ability
			// to write to our persistent data folder unless something is REALLY broken
			// with the computer/device we're running on.
			Directory.CreateDirectory( FileSaveBasePath() );
		}

		File.WriteAllText( filePath, writer.ToString() );

	}

	public void LoadWorld() {
		Debug.Log("LoadWorld button was clicked.");

		// Reload the scene to reset all data (and purge old references)
		loadWorld = true;
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );

	}

	void CreateEmptyWorld() {
		// Create a world with Empty tiles
		world = new World(100, 100);

		// Center the Camera
		Camera.main.transform.position = new Vector3( world.Width/2, world.Height/2, Camera.main.transform.position.z );

	}

	void CreateWorldFromSaveFile() {
		Debug.Log("CreateWorldFromSaveFile");
		// Create a world from our save file data.

		XmlSerializer serializer = new XmlSerializer( typeof(World) );
		TextReader reader = new StringReader( PlayerPrefs.GetString("SaveGame00") );
		Debug.Log( reader.ToString() );
		world = (World)serializer.Deserialize(reader);
		reader.Close();



		// Center the Camera
		Camera.main.transform.position = new Vector3( world.Width/2, world.Height/2, Camera.main.transform.position.z );

	}

}
