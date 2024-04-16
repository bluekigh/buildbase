using UnityEngine;
using System.Collections.Generic;

public class SpriteManager : MonoBehaviour {

	// Sprite Manager isn't responsible for actually creating GameObjects.
	// That is going to be the job of the individual ________SpriteController scripts.
	// Our job is simply to load all sprites from disk and keep the organized.

	static public SpriteManager current;

	Dictionary<string, Sprite> sprites;

	// Use this for initialization
	void OnEnable () {
		current = this;

		LoadSprites();
	}

	void LoadSprites() {
		sprites = new Dictionary<string, Sprite>();

		string filePath = System.IO.Path.Combine( Application.streamingAssetsPath, "Images" );
		filePath = System.IO.Path.Combine( Application.streamingAssetsPath, "CursorCircle.png" );

		LoadSprite("CursorCircle", filePath);



	}

	void LoadSprite(string spriteName, string filePath) {
		byte[] imageBytes = System.IO.File.ReadAllBytes( filePath );

		Texture2D imageTexture = new Texture2D(2, 2);	// Create some kind of dummy instance of Texture2D
		imageTexture.LoadImage(imageBytes);	// This will correctly resize the texture based on the image file

		Rect spriteCoordinates = new Rect(0, 0, imageTexture.width, imageTexture.height);	// In pixels!
		Vector2 pivotPoint = new Vector2(0.5f, 0.5f);	// Ranges from 0..1 -- so 0.5f == center
		int pixelsPerUnit = 32;

		Sprite s = Sprite.Create(imageTexture, spriteCoordinates, pivotPoint, pixelsPerUnit);

		sprites[spriteName] = s;
	}
	
	public Sprite GetSprite(string spriteName) {
		if(sprites.ContainsKey(spriteName) == false) {
			Debug.LogError("No sprite with name: " + spriteName);
			return null;	// TODO: What if we return a "dummy" sprite, like a purple square?
		}

		return sprites[spriteName];
	}
}
