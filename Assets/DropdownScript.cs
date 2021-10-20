using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownScript : MonoBehaviour
{
    public string text;
    // Start is called before the first frame update
    void Start()
    {
        var dropdown = transform.GetComponent<Dropdown>();

        dropdown.options.Clear();

        dropdown.options.Add(new Dropdown.OptionData() { text = "BFS" });
        dropdown.options.Add(new Dropdown.OptionData() { text = "DFS" });
        dropdown.options.Add(new Dropdown.OptionData() { text = "A*" });
        dropdown.options.Add(new Dropdown.OptionData() { text = "Q Learning" });

        DropdownItemSelected(dropdown);

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        text = dropdown.options[index].text;
    }
}
