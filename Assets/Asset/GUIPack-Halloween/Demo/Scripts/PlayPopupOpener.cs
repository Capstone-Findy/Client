// Copyright (C) 2017 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
	// Specialized version of the PopupOpener class that opens the PlayPopup popup
	// and sets an appropriate number of stars (that can be configured from within the
	// editor).
	public class PlayPopupOpener : PopupOpener
	{
	    public Canvas parentCanvas;
	    public int starsObtained = 0;

	    public override void OpenPopup()
	    {
	        var popup = Instantiate(popupPrefab) as GameObject;
	        popup.SetActive(true);
	        popup.transform.localScale = Vector3.zero;
	        popup.transform.SetParent(parentCanvas.transform, false);

	        var playPopup = popup.GetComponent<PlayPopup>();
	        playPopup.Open();
	        playPopup.SetAchievedStars(starsObtained);
	    }
	}
}