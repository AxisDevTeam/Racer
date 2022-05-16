using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public string verticalAxisName = "Vertical";        //The name of the thruster axis
	public string horizontalAxisName = "Horizontal";    //The name of the rudder axis
	public string brakingKey = "Brake";                 //The name of the brake button

	//We hide these in the inspector because we want 
	//them public but we don't want people trying to change them
	[HideInInspector] public float thruster = 0;            //The current thruster value
	[HideInInspector] public float rudder = 0;              //The current rudder value
	[HideInInspector] public bool isBraking = false;            //The current brake value

	void Update()
	{
		//If the player presses the Escape key and this is a build (not the editor), exit the game
		if (Input.GetButtonDown("Cancel") && !Application.isEditor)
			Application.Quit();

		/*
		//If a GameManager exists and the game is not active...
		if (GameManager.instance != null && !GameManager.instance.IsActiveGame())
		{
			//...set all inputs to neutral values and exit this method
			thruster = rudder = 0f;
			isBraking = false;
			return;
		}
		*/

		//Get the values of the thruster, rudder, and brake from the input class
		thruster = Input.GetAxis(verticalAxisName);
		rudder = Input.GetAxis(horizontalAxisName);
		isBraking = Input.GetButton(brakingKey);
	}
}