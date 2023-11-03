using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public TileState state;

    [HideInInspector]
    public TileCell cell;

    //[HideInInspector]
    public bool locked;

    public Image imageBackground;
    public Text txtNumber;


    public void SetState(TileState state)
    {
        this.state = state;

        imageBackground.color = state.backgroundColor;
        txtNumber.color = state.txtColor;
        txtNumber.text = state.number.ToString();
    }

    public void LinkCell(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(MoveAnimate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(MoveAnimate(cell.transform.position, true));
    }

    private IEnumerator MoveAnimate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(a: from, b: to, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = to;

        if (merging)
        {
            Destroy(gameObject);
        }
    }
}
