using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMaker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMaker parent;

    public PathMaker(MapLocation location, float g, float h, float f, GameObject marker, PathMaker parent)
    {
        this.location = location;
        G = g;
        H = h;
        F = f;
        this.marker = marker;
        this.parent = parent;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMaker)obj).location);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class FindPathAStar : MonoBehaviour
{
    // maze
    public Maze maze;

    [Header("Nodes Material")]
    public Material closedNodeMaterial;
    public Material openNodeMaterial;

    // list of open and closed nodes
    List<PathMaker> closedNodes = new List<PathMaker>();
    List<PathMaker> openNodes = new List<PathMaker>();

    [Header("Nodes")]
    public GameObject startNodePrefab;
    public GameObject finishNodePrefab;
    public GameObject pathNodePrefab;

    PathMaker goalNode;
    PathMaker startNode;

    PathMaker lastPosition;
    bool done = false;

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject marker in markers)
        {
            marker.SetActive(!marker.activeSelf);
        }
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        // create a list of all space (not walls) location and added to the list
        List<MapLocation> locations = new List<MapLocation>();
        for (int i = 0; i < maze.depth; i++)
            for (int j = 0; j < maze.width; j++)
            {
                if (maze.map[j, i] != 1)
                    locations.Add(new MapLocation(j, i));
            }

        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x, 0.0f, locations[0].z) * maze.scale;
        startNode = new PathMaker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
            Instantiate(startNodePrefab, startLocation, Quaternion.identity), null);

        Vector3 endLocation = new Vector3(locations[1].x, 0.0f, locations[1].z) * maze.scale;
        startNode = new PathMaker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
            Instantiate(finishNodePrefab, endLocation, Quaternion.identity), null);

        openNodes.Clear();
        closedNodes.Clear();
        openNodes.Add(startNode);
        lastPosition = startNode;
    }

    void Search(PathMaker thisNode)
    {
        if (thisNode.Equals(goalNode)) { done = true; return; };    // the goal has been reached

        foreach (MapLocation direction in maze.directions)
        {
            MapLocation neighbour = thisNode.location + direction;
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (IsClosed(neighbour)) continue;

            float G = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(thisNode.location.ToVector(), goalNode.location.ToVector());
            float F = G + H;

            GameObject pathNode = Instantiate(pathNodePrefab, new Vector3(neighbour.x, 0.0f, neighbour.z) * maze.scale, Quaternion.identity);

        }
    }

    bool IsClosed(MapLocation marker)
    {
        foreach (var p in closedNodes)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            BeginSearch();
    }
}
