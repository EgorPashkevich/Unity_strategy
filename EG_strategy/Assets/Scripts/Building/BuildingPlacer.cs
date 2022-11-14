using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour {

    public float CellSize = 1f;

    public Camera Camera;
    private Plane _plane;

    public Building CurrentBuilding;

    public Dictionary<Vector2Int, Building> BuildingDictionary = new Dictionary<Vector2Int, Building>();
    void Start() {
        _plane = new Plane(Vector3.up, Vector3.zero);
    }
    void Update() {
        CreateBuildingToTable();
    }

    public void CreateBuildingToTable() {
        if (CurrentBuilding == null) {
            return;
        }

        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 point = ray.GetPoint(distance) / CellSize;

        int x = Mathf.RoundToInt(point.x);
        int z = Mathf.RoundToInt(point.z);

        CurrentBuilding.transform.position = new Vector3(x, 0, z) * CellSize;

        if (CheckAllow(x, z, CurrentBuilding)) {
            CurrentBuilding.DisplayAcceptablePosition();
            if (Input.GetMouseButtonDown(0)) {

                InstallBuilding(x, z, CurrentBuilding);
                CurrentBuilding = null;
            }
        } else {
            CurrentBuilding.DisplayUnacceptablePosition();
        }
    }

    bool CheckAllow(int xPos, int zPos, Building building) {
        for (int x = 0; x < building.XSize; x++) {
            for (int z = 0; z < building.ZSize; z++) {
                Vector2Int coordinata = new Vector2Int(xPos + x, zPos + z);
                if (BuildingDictionary.ContainsKey(coordinata)) {
                    return false;
                }
            }
        }
        return true;
    }

    void InstallBuilding(int xPos, int zPos, Building building) {
        for (int x = 0; x < building.XSize; x++) {
            for (int z = 0; z < building.ZSize; z++) {
                Vector2Int coordinata = new Vector2Int(xPos + x, zPos + z);
                BuildingDictionary.Add(coordinata, CurrentBuilding);
            }
        }
    }

    public void CreateBuilding(GameObject buildingPrefab) {
        GameObject newBuilding = Instantiate(buildingPrefab);
        CurrentBuilding = newBuilding.GetComponent<Building>();
    }
}
