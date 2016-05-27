using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LayersController : MonoBehaviour {

	public GameObject layer_line_fab;
	public GameObject name_input;
	public GameObject name_input_field;

	private GameObject _content;
	private int _layerIndex = 0;
	private PolyWorldController _polyWorldController;

//	private List<GameObject> _layerList;
	private Dictionary<int, GameObject> _layerDict;
	private List<int> _selectedLayers;
	private CameraController _cameraController;	
	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("LayersContent");
		_layerDict = new Dictionary<int, GameObject> ();
		_selectedLayers = new List<int> ();
		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();
		name_input.SetActive (false);
		_cameraController = GameObject.Find ("CameraBase").GetComponent<CameraController> ();
		

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OnClickNewLayer()
	{
		Debug.Log ("new layer");

		// tell other to make a new layer, 
		// will 
		int layerId = _layerIndex++;

		var layer = Instantiate (layer_line_fab) as GameObject;
		layer.transform.SetParent (_content.transform);

		_layerDict.Add (layerId, layer);

		var layerLine = layer.GetComponent<LayerLineController> ();
		layerLine.SetLayerId(layerId);

		RefreshContentLayout ();

		_polyWorldController.NewPolyObject (layerId);
		
		OnSelectLayer (layerId);
	}

	void RefreshContentLayout()
	{
		int heightCount = 0;
		float lineHeight = 0;
		foreach (GameObject obj in _layerDict.Values) {
			var rect = obj.GetComponent<RectTransform> ();
			lineHeight = rect.sizeDelta.y * 1.2f;
			rect.localPosition = new Vector3 (0, -lineHeight * heightCount, 0);

			heightCount++;
		}

		var contentRect = _content.GetComponent<RectTransform> ();
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, _layerDict.Count * lineHeight);
	}

	public void OnClickDeleteLayer()
	{
		Debug.Log ("delete layer");

		if (_selectedLayers.Count == 0)
			return;

		// delete all selected
		foreach (int id in _selectedLayers) {
			var desObj = _layerDict[id];
			_layerDict.Remove(id);
			Destroy(desObj);

			_polyWorldController.DeletePolyObject(id);
		}
		_selectedLayers.Clear ();

		// reposition all
		RefreshContentLayout ();

		_polyWorldController.RefreshSelection ();
	}

	public void OnSelectLayer(int layerId)
	{
		if (Input.GetKey ("right shift") || Input.GetKey ("left shift")) {
			//multiple selection
		} else {
			// de-select old ones
			foreach (int id in _selectedLayers) {
				_layerDict [id].GetComponent<LayerLineController> ().SetSelection (false);
				_polyWorldController.SetObjectSelection(id, false);
			}
			_selectedLayers.Clear();
		}

		// select new
		if (!_selectedLayers.Contains (layerId)) {
			_selectedLayers.Add(layerId);
			_layerDict [layerId].GetComponent<LayerLineController> ().SetSelection (true);
			_polyWorldController.SetObjectSelection(layerId, true);
		}

		_polyWorldController.RefreshSelection ();
		_cameraController.OnSelectNewLayer ();

	}

	void RefreshLayerSelection()
	{
		foreach (var id in _layerDict.Keys) {
			_layerDict [id].GetComponent<LayerLineController> ().SetSelection (false);
			_polyWorldController.SetObjectSelection(id, false);
		}

		foreach (var id in _selectedLayers) {
			
			_layerDict [id].GetComponent<LayerLineController> ().SetSelection (true);
			_polyWorldController.SetObjectSelection(id, true);
		}

		_polyWorldController.RefreshSelection ();

	}

	private bool _flagAllShow = true;
	public void OnClickEyeAll()
	{
		Debug.Log ("eye all");

		_flagAllShow = !_flagAllShow;
		foreach (int id in _layerDict.Keys) {
//			_polyWorldController.SetObjectVisibility (id, _flagAllShow);
			_layerDict[id].GetComponent<LayerLineController>().OnToggleEye(_flagAllShow);
		}
	}

	public void SetLayerVisibility(int id, bool isShow)
	{
		_polyWorldController.SetObjectVisibility (id, isShow);
	}

	public void OnClickDuplicate()
	{
		Debug.Log ("duplicate");

		List<int> newSelected = new List<int> ();

		foreach (int id in _selectedLayers) {
			int newLayerId = _layerIndex++;

			
			var newLine = Instantiate (layer_line_fab) as GameObject;
			newLine.transform.SetParent (_content.transform);
			
			_layerDict.Add (newLayerId, newLine);
			
			var layerLineCon = newLine.GetComponent<LayerLineController> ();
			layerLineCon.SetLayerId(newLayerId);
			var oldLayerName = 	_layerDict[id].GetComponent<LayerLineController>().GetLayerName();

			layerLineCon.SetLayerName(oldLayerName);

			
//			_polyWorldController.NewPolyObject (newLayerId);

			_polyWorldController.DuplicateObject(id, newLayerId);

			newSelected.Add(newLayerId);
		}

		_selectedLayers = newSelected;

		RefreshContentLayout ();
		RefreshLayerSelection ();
	}

	private int _underEditingLayerId;

	public void BeginEditLayerName(int id)
	{
		_underEditingLayerId = id;
		// show input field
		name_input.SetActive (true);
		name_input_field.GetComponent<InputField> ().text = "";
	}

	public void EndEditLayerName()
	{
		string newName = name_input_field.GetComponent<InputField> ().text;
		if (newName.Length > 0 && _layerDict.ContainsKey(_underEditingLayerId)) {
			_layerDict[_underEditingLayerId].GetComponent<LayerLineController>().SetLayerName(newName);
		}

		name_input.SetActive (false);

	}
}
