﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Serialization;
using System.IO;


public class FileSaveController : DialogBoxController {

	public GameObject fileListItemPrefab;
	public Transform fileList;

	// Use this for initialization
	void Start () {
	
	}

	public override void ShowDialog()
	{
		base.ShowDialog();

		// Get list of files in save location

		string filePath = WorldController.Instance.FileSaveBasePath();

		// existingSaves will contain the FULL path to the save file
		string[] existingSaves = Directory.GetFiles(filePath, "*.sav");

		// TODO: Make sure the saves are sorted by date/time, with the newest
		//  saves being at the top.

		// Our save dialog has an input field, which the fileListItems fill out for
		// us when we click on them
		InputField inputField = gameObject.GetComponentInChildren<InputField>();

		// Build file list by instantiating fileListItemPrefab

		foreach(string file in existingSaves) {
			GameObject go = (GameObject)GameObject.Instantiate(fileListItemPrefab);

			// Make sure this gameobject is a child of our list box
			go.transform.SetParent( fileList );

			// file contains something like "C:\Users\UserName\......\Project Porcupine\Saves\SomeFileName.sav"
			// Path.GetFileName(file) returns "SomeFileName.sav"
			// Path.GetFileNameWithoutExtension(file) returns "SomeFileName"

			go.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(file);

			go.GetComponent<DialogListItem>().inputField = inputField;
		}

	}

	public override void CloseDialog() {
		// Clear out all the children of our file list

		while(fileList.childCount > 0) {
			Transform c = fileList.GetChild(0);
			c.SetParent(null);	// Become Batman
			Destroy(c.gameObject);
		}

		// We COULD clear out the inputField field here, but I think
		// it makes sense to leave the old filename in there to make
		// overwriting easier?
		// Alternatively, we could either:
		//   a) Clear out the text box
		//	 b) Append an incremental number to it so that it automatically does
		//		something like "SomeFileName 13"

		base.CloseDialog();
	}

	public void OkayWasClicked() {
		// TODO:
		// check to see if the file already exists
		// if so, ask for overwrite confirmation.

		string fileName = gameObject.GetComponentInChildren<InputField>().text;

		// TODO: Is the filename valid?  I.E. we may want to ban path-delimiters (/ \ or :) and 
		// maybe periods?      ../../some_important_file

		// Right now fileName is just what was in the dialog box.  We need to pad this out to the full
		// path, plus an extension!
		// In the end, we're looking for something that's going to be similar to this (depending on OS)
		//    C:\Users\Quill18\ApplicationData\MyCompanyName\MyGameName\Saves\SaveGameName123.sav

		// Application.persistentDataPath == C:\Users\<username>\ApplicationData\MyCompanyName\MyGameName\

		string filePath = System.IO.Path.Combine( WorldController.Instance.FileSaveBasePath(), fileName + ".sav" );

		// At this point, filePath should look very much like
		//     C:\Users\Quill18\ApplicationData\MyCompanyName\MyGameName\Saves\SaveGameName123.sav

		if(File.Exists(filePath) == true) {
			// TODO: Do file overwrite dialog box.

			Debug.LogError("File already exists -- overwriting the file for now.");

		}

		CloseDialog();

		SaveWorld(filePath);
	}

	public void SaveWorld(string filePath) {
		// This function gets called when the user confirms a filename
		// from the save dialog box.

		// Get the file name from the save file dialog box

		Debug.Log("SaveWorld button was clicked.");

		XmlSerializer serializer = new XmlSerializer( typeof(World) );
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, WorldController.Instance.world);
		writer.Close();

		Debug.Log( writer.ToString() );

		//PlayerPrefs.SetString("SaveGame00", writer.ToString());

		// Create/overwrite the save file with the xml text.

		// Make sure the save folder exists.
		if( Directory.Exists( WorldController.Instance.FileSaveBasePath() ) == false ) {
			// NOTE: This can throw an exception if we can't create the folder,
			// but why would this ever happen? We should, by definition, have the ability
			// to write to our persistent data folder unless something is REALLY broken
			// with the computer/device we're running on.
			Directory.CreateDirectory( WorldController.Instance.FileSaveBasePath() );
		}

		File.WriteAllText( filePath, writer.ToString() );

	}}
