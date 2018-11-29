using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour {

	public void OpenTab (string URL) {
        Application.OpenURL(URL);
	}

}
