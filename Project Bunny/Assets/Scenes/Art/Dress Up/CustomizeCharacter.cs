using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeCharacter : MonoBehaviour
{
    public InputField myName; //stores the player's name
    public GameObject playerName; //refers to the text mesh display above player

    public Color[] teamColor;

    public Material teamMat;
    public GameObject teamShirt;

    public GameObject[] model;
    public int currentIndex;
    public Material mat;
    public Color[] color;
    public int currentColorIndex;
    public Slider colorSlider;

    public void ChangeModel()
    {
        if (currentIndex >= model.Length - 1)
        {
            currentIndex = 0;
        }
        else
        {
            currentIndex++;
        }

        for (int i = 0; i < model.Length; i++)
        {
            model[i].SetActive(false);
        }
        model[currentIndex].SetActive(true);
    }

    //changes the model based on last selected, 1 set of palettes for all
    public void ChangeColor()
    {
        if(colorSlider)
            colorSlider.value = 0;

        if (currentColorIndex >= color.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }

        for (int i = 0; i < color.Length; i++)
        {
            mat.color = color[currentColorIndex];
        }
    }


    public void TuneColor()
    {
        float H, S, V;
        Color.RGBToHSV(color[currentColorIndex], out H, out S, out V);
        mat.color = Color.HSVToRGB(H + 0.2f * colorSlider.value, S + 0.02f * colorSlider.value, V + 0.05f * colorSlider.value);
    }

    public void ChangeName(string name)
    {
        name = myName.text;
        playerName.transform.GetChild(0).GetComponent<TextMesh>().text = name;
    }

    public void ChangeTeamColor(int index) //0:blue;1:red
    {
        teamMat.color = teamColor[index];
        //playerName.transform.GetChild(0).GetComponent<TextMesh>().color = teamColor[index];
        teamShirt.SetActive(true);

    }


    // Start is called before the first frame update
    void Start()
    {
        //ChangeHair();
        //ChangeHat();
        //ChangePants();
        
        if(teamShirt)
            teamShirt.SetActive(false);//starts with no team assigned
    }

    private void Update()
    {
        if(playerName)
            playerName.transform.LookAt(Camera.main.transform); //move this to player movement later
    }
}
