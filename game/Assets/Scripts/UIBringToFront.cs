using UnityEngine;

public class UIBringToFront : MonoBehaviour
{
	private void OnEnable()
	{
		transform.SetAsLastSibling();
	}
}
