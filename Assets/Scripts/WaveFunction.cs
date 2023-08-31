#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    [SerializeField] private int dimensions;
    [SerializeField] private Tile[] tileObjects;
    [SerializeField] private SpriteRenderer prefab;

    private List<Cell> gridComponents;
    private List<Cell> tempCells;
    private List<Sprite> validOptions;
    private int iterations = 0;
    private System.Random rand;

    private void Awake()
    {
        rand = new System.Random(System.DateTime.Now.Millisecond);
        gridComponents = new List<Cell>();
        tempCells = new List<Cell>();
        validOptions = new List<Sprite>();
    }

    private void Start()
    {
        InitializeGrid();
        StartEntropy();
#if !TEST
        gridComponents = tempCells = validOptions = null;
#endif
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var newCell = new Cell(new Vector3(x, y), false, tileObjects);
                gridComponents.Add(newCell);
            }
        }
    }

    //get all cells that have lowest tileOptions
    //sort to get the lowest, and remove those with more tileOptions
    private
#if TEST
        IEnumerator
#else
        void
#endif
        CheckEntropy()
    {
        tempCells.Clear();
        for (var i = 0; i < gridComponents.Count; i++) if (!gridComponents[i].collapsed) tempCells.Add(gridComponents[i]);
        tempCells.Sort((a, b) => a.tileOptions.Count.CompareTo(b.tileOptions.Count));

        int arrLength = tempCells[0].tileOptions.Count;
        for (var i = tempCells.Count - 1; i >= 0; i--)
        {
            if (tempCells[i].tileOptions.Count <= arrLength) break;
            tempCells.RemoveAt(i);
        }

#if TEST
        yield return new WaitForSeconds(0.01f);
#endif

        CollapseCell(tempCells);
    }

    private void StartEntropy()
    {
#if TEST
        StartCoroutine(CheckEntropy());
#else
        CheckEntropy();
#endif
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        var cellToCollapse = tempGrid[rand.Next(tempGrid.Count)];

        cellToCollapse.collapsed = true;
        var options = cellToCollapse.tileOptions;
        var selectedTile = options[rand.Next(options.Count)];
        options.Clear();
        options.Add(selectedTile);

        Instantiate(selectedTile, cellToCollapse.pos, Quaternion.identity);

        UpdateGeneration();
    }

    //update posibility of all tiles base on its neighbours
    private void UpdateGeneration()
    {
        tempCells.Clear();
        tempCells.AddRange(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    //Debug.Log("called");
                    tempCells[index] = gridComponents[index];
                }
                else
                {
                    var options = tempCells[index].tileOptions;
                    options.Clear();
                    options.AddRange(tileObjects);

                    //check down
                    if (y > 0)
                    {
                        Cell down = gridComponents[x + (y - 1) * dimensions];
                        validOptions.Clear();

                        foreach (var possibleOptions in down.tileOptions)
                        {
                            validOptions.AddRange(possibleOptions.upNeighbors);
                        }

                        CheckValidity(options, validOptions);
                    }

                    //check right
                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        validOptions.Clear();

                        foreach (var possibleOptions in right.tileOptions)
                        {
                            validOptions.AddRange(possibleOptions.leftNeighbors);
                        }

                        CheckValidity(options, validOptions);
                    }

                    //check up
                    if (y < dimensions - 1)
                    {
                        Cell up = gridComponents[x + (y + 1) * dimensions];
                        validOptions.Clear();

                        foreach (var possibleOptions in up.tileOptions)
                        {
                            validOptions.AddRange(possibleOptions.downNeighbors);
                        }

                        CheckValidity(options, validOptions);
                    }

                    //check left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        validOptions.Clear();

                        foreach (var possibleOptions in left.tileOptions)
                        {
                            validOptions.AddRange(possibleOptions.rightNeighbors);
                        }

                        CheckValidity(options, validOptions);
                    }
                }
            }
        }

        gridComponents.Clear();
        gridComponents.AddRange(tempCells);
        iterations++;

        if(iterations < dimensions * dimensions)
        {
            StartEntropy();
        }

    }

    private void CheckValidity(List<Tile> optionList, List<Sprite> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element.sprite))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
