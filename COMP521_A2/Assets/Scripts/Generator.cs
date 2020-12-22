using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tingyu Shen 260798146
// This class is for terrain generation includes ground, mountain and water
public class Generator : MonoBehaviour
{
	// I make variables public to let other classes to obtain
	public GameObject line;                             // In scene line. 
	public GameObject point;                            // In scene point.
	public List<Vector3> TerrainPoints;
	public List<Vector3> WaterPoints;
	private Color m_point_color;                        // Color

	// gorund start point and end point in x axis
	public int minX = -36;
	public int maxX = 37;
	// ground level position and water level position in y axis
	public int groundlevel = -8;
	public int waterlevel = -9;

	// this is for wind altitude
	public float mountainTop = 0;
	// my point index
	public int index = 0;

	private Color waterColor;

	// My perlin function
	PerlinNoise perlinNoise1;

	void Start()
	{
		TerrainPoints = new List<Vector3>();
		WaterPoints = new List<Vector3>();
		perlinNoise1 = new PerlinNoise();
		m_point_color = new Color(0, 0, 0);
		waterColor = new Color(0,0,1);
		GenerateTerrain();
	}

	void Update()
	{

	}

	// My generate terrain function
	private void GenerateTerrain()
	{
		GameObject previousPoint = null;
		Vector3 previousPoint_position = new Vector3();
		Vector3 waterPoint_position = new Vector3();

		// the variant in y axis 
		float addHeight = 0;

		// distance between 2 points is 0.25f
		for (float i = minX; i < maxX; i += 0.25f)
		{
			// using x position to generate overall shape
			// seuqnce is ground -> mountain upward -> mountain downward
			// -> water floor -> moutain upward -> moutain downward -> ground
			if (i < -24 || i > 24)
			{
				addHeight = 0;
			}
			else if (-24 < i && i <= -18)
			{
				addHeight += 0.5f;
			}
			else if (-18 < i && i <= -8)
			{
				addHeight -= 0.5f;
			}
			else if (8 < i && i <= 18)
			{
				addHeight += 0.5f;
			}
			else if (18 < i && i <= 24)
			{
				addHeight -= 0.5f;
			}

			// each position create a point 
			GameObject block = point;

			// add a random offset to make random perlin noise
			System.Random random = new System.Random();
			float offset = random.Next(1000);
			float c = perlinNoise1.PerlinNoise_1D(i+offset);

			// I add noise in y axis to create slightly different shape everytime
			Vector3 block_position = new Vector3(i, groundlevel + addHeight + c, 0);

			if(block_position.y > mountainTop)
            {
				mountainTop = block_position.y;
            }
			// add point to list for collision detection
			TerrainPoints.Insert(index, block_position);
			index++;
			Instantiate(block, block_position, Quaternion.identity);

			// check the water level point
			if(groundlevel + addHeight == waterlevel)
            {
				// left water level point
				if(i < 0)
                {
					waterPoint_position = block_position;
				}
				// right water level point
                else
                {
					// from here create water points with perlin noise
					Vector3 previousWater = waterPoint_position;
					for (int j = 1;  j < 45; j++)
                    {
						WaterPoints.Add(previousWater);
						GameObject waterPoint = point;
						float offsetW = random.Next(100);
						float water1 = perlinNoise1.PerlinNoise_1D(i + offsetW);
						Instantiate(waterPoint, waterPoint_position + new Vector3(0.5f * j, water1, 0), Quaternion.identity);
						
						GameObject waterLine = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
						LineRenderer lineRendererW = waterLine.GetComponent<LineRenderer>();
						lineRendererW.SetPosition(0, previousWater);                              // Setting the line start position. 
						lineRendererW.SetPosition(1, waterPoint_position + new Vector3(0.5f * j, water1, 0));
						lineRendererW.startColor = waterColor;                                    // Setting the line start color. 
						lineRendererW.endColor = waterColor;

						previousWater = waterPoint_position + new Vector3(0.5f * j, water1, 0);
					}

					// link last two points
					WaterPoints.Add(block_position);
					GameObject waterLine1 = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
					LineRenderer lineRenderer1 = waterLine1.GetComponent<LineRenderer>();
					lineRenderer1.SetPosition(0, previousWater);                              // Setting the line start position. 
					lineRenderer1.SetPosition(1, block_position);
					lineRenderer1.startColor = waterColor;                                    // Setting the line start color. 
					lineRenderer1.endColor = waterColor;

				}
				
            }

			// draw line between two points
			if (previousPoint == null)
			{
				previousPoint = block;
				previousPoint_position = block_position;
			}
			else
			{
				GameObject line1 = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
				LineRenderer lineRenderer = line1.GetComponent<LineRenderer>();
				lineRenderer.SetPosition(0, previousPoint_position);                              // Setting the line start position. 
				lineRenderer.SetPosition(1, block_position);
				lineRenderer.startColor = m_point_color;                                    // Setting the line start color. 
				lineRenderer.endColor = m_point_color;
				previousPoint = block;
				previousPoint_position = block_position;
			}
		}
	}

}

