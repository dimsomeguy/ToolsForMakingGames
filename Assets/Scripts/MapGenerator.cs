using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, Mesh};
	public DrawMode drawMode;

	const int mapChunkSize = 241;
	[Range(0,6)]
	public int levelOfDetail;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public bool autoUpdate;

	public TerrainType[] regions;

    public GameObject[] FloraArray;
    public GameObject[] HighFlora;

    public GameObject[] BuildingObjArray;

    public bool BuildingGen = false;



    public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }

                    if (noiseMap[x, y] > 0.35 && noiseMap[x, y] < 0.7)
                    {
                        if (Random.Range(0, 200) == 0)
                        {
                            Vector3 Placeposition = new Vector3(x - 120, meshHeightCurve.Evaluate(noiseMap[x, y]) * 40.0f, y * -1 + 120);
                            GameObject P = FloraArray[Random.Range(0, FloraArray.Length - 1)];
                            Instantiate(P, Placeposition, P.transform.rotation);
                        }
                    }
                    if (noiseMap[x, y] > 0.6 && noiseMap[x, y] < 0.9)
                    {
                        if (Random.Range(0,100) == 0)
                        {
                            Vector3 Placeposition = new Vector3(x - 120, meshHeightCurve.Evaluate(noiseMap[x, y]) * 40.0f, y * -1 + 120);
                            GameObject P = HighFlora[Random.Range(0, HighFlora.Length - 1)];
                            Instantiate(P, Placeposition, P.transform.rotation);
                        }
                    }
                    if (noiseMap[x, y] > 0.1 && noiseMap[x, y] < 0.35 && BuildingGen == true)
                    {
                        if (Random.Range(0, 1000) == 0)
                        {
                            Vector3 Placeposition = new Vector3(x - 120, meshHeightCurve.Evaluate(noiseMap[x, y]) * 40.0f, y * -1 + 120);
                            GameObject P = BuildingObjArray[Random.Range(0, BuildingObjArray.Length - 1)];
                            Instantiate(P, Placeposition, P.transform.rotation);
                        }
                    }
                }
            }
        }    

        MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		}
	}

	void OnValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}

    void isCollide()
    {

    }
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}