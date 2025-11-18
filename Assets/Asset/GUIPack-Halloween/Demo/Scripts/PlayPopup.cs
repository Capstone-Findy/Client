// Copyright (C) 2017 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
	// Specialized behavior for the popup that opens before selecting a level to play in
	// the demo. It showcases how to create a specialized popup with custom behavior: in this
	// case, one to three stars can be displayed depending on the player score on that particular
	// level.
	public class PlayPopup : Popup
	{
	    public Color enabledColor;
	    public Color disabledColor;

	    public Image leftStarImage;
	    public Image middleStarImage;
	    public Image rightStarImage;

	    public void SetAchievedStars(int starsObtained)
	    {
	        if (starsObtained == 0)
	        {
	            leftStarImage.color = disabledColor;
	            middleStarImage.color = disabledColor;
	            rightStarImage.color = disabledColor;
	        }
	        else if (starsObtained == 1)
	        {
	            leftStarImage.color = enabledColor;
	            middleStarImage.color = disabledColor;
	            rightStarImage.color = disabledColor;
	        }
	        else if (starsObtained == 2)
	        {
	            leftStarImage.color = enabledColor;
	            middleStarImage.color = enabledColor;
	            rightStarImage.color = disabledColor;
	        }
	        else if (starsObtained == 3)
	        {
	            leftStarImage.color = enabledColor;
	            middleStarImage.color = enabledColor;
	            rightStarImage.color = enabledColor;
	        }
	    }
	}
}