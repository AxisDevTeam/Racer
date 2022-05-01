using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipItem : MonoBehaviour
{
    public ItemHandler ih;
    public Item item;
    ShipController sc;
    public Image display;
    // Start is called before the first frame update
    void Awake()
    {
        sc = GetComponent<ShipController>();
        ih = GameObject.Find("EventSystem []").GetComponent<ItemHandler>();
        display = GameObject.Find("Item Image").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (item != null)
            {
                if (item.itemName.Equals("Boost"))
                {
                    StartCoroutine(sc.Boost(item.length));
                }
                item = null;
            }
            
        }

        if (item != null)
        {
            display.sprite = item.image;
        }
        else
        {
            display.sprite = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("item"))
        {
            item = ih.randomItem();
        }
    }

    
}
