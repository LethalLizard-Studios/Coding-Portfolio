using UnityEngine;

[RequireComponent(typeof(InventoryController))]
public class InventoryView : MonoBehaviour
{
    public Vector2Int gridDimensions = new Vector2Int(2, 2);

    public ScriptableInventoryStyle style;

    public Transform footer;

    public bool hasInitialized { private set; get; } = false;

    private Grid[,] _gridArray;

    private Transform _backgroundFaded;
    private Transform _background;

    private InventoryController _inventoryController;

    private const int GRID_SPACING = 40;
    private const int FOOTER_SPACING = 5;

    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();

        Initialize();
    }

    private void Initialize()
    {
        _backgroundFaded = Instantiate(style.fadedBacking, transform.GetChild(0)).transform;
        _background = Instantiate(style.gridBacking, transform.GetChild(0)).transform;

        //Make sure they are below the headers and additional UI.
        _backgroundFaded.SetAsFirstSibling();
        _background.SetAsFirstSibling();

        Resize(gridDimensions.x, gridDimensions.y);

        _gridArray = new Grid[gridDimensions.y, gridDimensions.x];

        for (int i = 0; i < gridDimensions.y; i++)
        {
            for (int j = 0; j < gridDimensions.x; j++)
            {
                GameObject g = Instantiate(style.grid, transform.GetChild(0));
                g.GetComponent<Grid>().x = j;
                g.GetComponent<Grid>().y = i;
                g.GetComponent<Grid>().inventoryController = _inventoryController;
                g.name = "Grid (" + j + "-" + i + ")";
                _gridArray[i, j] = g.GetComponent<Grid>();

                g.transform.localPosition = new Vector3(-j * GRID_SPACING, -i * GRID_SPACING);
            }
        }

        hasInitialized = true;

        _inventoryController.Initialize(_gridArray);
    }

    public void Resize(int width, int height)
    {
        if (_background == null || _backgroundFaded == null)
            return;

        //Get offset depending on if even or odd for centering purposes.
        int offsetLength;
        int offsetHeight;

        if (width % 2 == 0)
            offsetLength = GRID_SPACING;
        else
            offsetLength = 0;

        if (height % 2 == 0)
            offsetHeight = GRID_SPACING;
        else
            offsetHeight = 0;

        //Diagnol Background
        _backgroundFaded.localPosition = new Vector2((width / 2) * -GRID_SPACING + (offsetLength / 2),
            (height / 2) * -GRID_SPACING + (offsetHeight / 2));

        _backgroundFaded.GetComponent<RectTransform>().sizeDelta = new Vector2((width * GRID_SPACING) + 75, (height * GRID_SPACING) + 75);

        //Faded Transparent Background
        _background.localPosition = new Vector2((width / 2) * -GRID_SPACING + (offsetLength / 2),
            (height / 2) * -GRID_SPACING + (offsetHeight / 2));

        _background.GetComponent<RectTransform>().sizeDelta = new Vector2(width * GRID_SPACING, height * GRID_SPACING);

        if (footer != null)
            footer.localPosition = new Vector2(footer.localPosition.x, (height * -GRID_SPACING) + FOOTER_SPACING);
    }
}
