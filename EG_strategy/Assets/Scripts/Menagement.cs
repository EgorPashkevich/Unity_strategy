using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusSelection {
    UnitSelected,
    Frame,
    Other
}
public class Menagement : MonoBehaviour {
    public Camera Camera;
    public SelectableOjects Howered;
    public List<SelectableOjects> ListOfSelected = new List<SelectableOjects>();

    public Image FrameImage;
    private Vector2 _frameStart;
    private Vector2 _frameEnd;

    public StatusSelection CurrentStatusSelection;
    void Update() {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.GetComponent<SelectableCollider>()) {

                SelectableOjects hitSelectable = hit.collider.GetComponent<SelectableCollider>().SelectableOjects;

                if (Howered) {
                    if (Howered != hitSelectable) {
                        Howered.UnOnHover();
                        Howered = hitSelectable;
                        Howered.OnHover();
                    }
                } else {
                    Howered = hitSelectable;
                    Howered.OnHover();
                }
            } else {
                UnhoverCurrent();
            }
        } else {
            UnhoverCurrent();
        }

        if (Input.GetMouseButtonUp(0)) {
            if (Howered) {
                if (Input.GetKey(KeyCode.LeftControl) == false) {
                    UnselectAll();
                }
                CurrentStatusSelection = StatusSelection.UnitSelected;
                Select(Howered);
            }
        }

        if (CurrentStatusSelection == StatusSelection.UnitSelected) {
            if (Input.GetMouseButtonUp(0)) {
                if (hit.collider.tag == "Ground") {

                    int rowNumber = Mathf.CeilToInt(Mathf.Sqrt(ListOfSelected.Count));
                    for (int i = 0; i < ListOfSelected.Count; i++) {

                        int row = i / rowNumber;
                        int column = i % rowNumber;

                        Vector3 point = hit.point + new Vector3(row, 0, column);
                        ListOfSelected[i].WhenClickOnGround(point);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            UnselectAll();
        }

        // Выделение рамекой
        if (Input.GetMouseButtonDown(0)) {
            _frameStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) {

            _frameEnd = Input.mousePosition;
            Vector2 min = Vector2.Min(_frameStart, _frameEnd);
            Vector2 max = Vector2.Max(_frameStart, _frameEnd);
            Vector2 size = max - min;

            if (size.magnitude > 10) {

                FrameImage.enabled = true;
                FrameImage.rectTransform.anchoredPosition = min;
                FrameImage.rectTransform.sizeDelta = size;
                Rect rect = new Rect(min, size);

                UnselectAll();

                Unit[] allUnits = FindObjectsOfType<Unit>();
                for (int i = 0; i < allUnits.Length; i++) {
                    Vector2 screenPosition = Camera.WorldToScreenPoint(allUnits[i].transform.position);
                    if (rect.Contains(screenPosition)) {
                        Select(allUnits[i]);
                    }
                }

                CurrentStatusSelection = StatusSelection.Frame;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            FrameImage.enabled = false;
            if (ListOfSelected.Count > 0) {
                CurrentStatusSelection = StatusSelection.UnitSelected;
            } else {
                CurrentStatusSelection = StatusSelection.Other;
            }
        }

    }

    void Select(SelectableOjects selectableOjects) {
        if (ListOfSelected.Contains(selectableOjects) == false) {
            ListOfSelected.Add(selectableOjects);
            selectableOjects.Select();
        }
    }

    public void Unselect(SelectableOjects selectableOjects) {
        if (ListOfSelected.Contains(selectableOjects)) {
            ListOfSelected.Remove(selectableOjects);
        }
    }

    void UnselectAll() {
        for (int i = 0; i < ListOfSelected.Count; i++) {
            ListOfSelected[i].UnSelect();
        }
        ListOfSelected.Clear();
        CurrentStatusSelection = StatusSelection.Other;
    }

    void UnhoverCurrent() {
        if (Howered) {
            Howered.UnOnHover();
            Howered = null;
        }
    }
}
