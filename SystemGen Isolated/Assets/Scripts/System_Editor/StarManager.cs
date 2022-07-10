using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class StarManager : MonoBehaviour
{
    //public knowledge variables that will be accessed by UI
    public string StarSearchTerm;
    public float StarMass;
    public string ParentObject;
    public string StarName;
    public float RotationRate;
    public string SystemFileName;
    //system loaddata
    protected List<string> SystemDataArray;

    // Start is called before the first frame update
    void Start()
    {
        //name the root object after the stars unique idenfitier
        gameObject.name = StarSearchTerm;
        
        //recombine the system file directory address
        string SystemFileAddress = Application.streamingAssetsPath 
            + "/Star_Systems/" 
            + SystemFileName
            + ".system";

        //parse all lines of the system file to a list for reading
        SystemDataArray = File.ReadAllLines(SystemFileAddress).ToList();

        //search each line until unique identifier is found
        for (int i = 0; i < SystemDataArray.Count ; i++) 
        {
            if(SystemDataArray[i] == StarSearchTerm + " {"){

                //get saved body data from system file
                StarName = ReturnFileValue(i+1);
                Debug.Log(StarName);
                StarMass = float.Parse(ReturnFileValue(i+2));
                Debug.Log(StarMass);
                ParentObject = ReturnFileValue(i+3);
                Debug.Log(ParentObject);
                RotationRate = float.Parse(ReturnFileValue(i+4));
                Debug.Log(RotationRate);
            }
        }

        ApplyData();   
    }

    string ReturnFileValue(int index)
    {
        int FilePoint = SystemDataArray[index].IndexOf("=");
        return(SystemDataArray[index].Substring(FilePoint+2));
    }

    // Update is called once per frame
    void Update()
    {
        ApplyData();  
        RotateBody(); 
    }

    void RotateBody()
    {
        /*
        //rotation of body
        rotationrate
        float Rate = GameObject.Find("Barycenter").GetComponent<Timekeep>().GameSpeed;
        transform.Rotate(0, (Rate / 300) / (600 * Time.deltaTime), 0);
        */
    }


    // ApplyData is called by UI 
    public void ApplyData()
    {
         //Diameter at formation
        float BaseDiameter = Mathf.Pow(StarMass, 0.7f)*0.8f;
        //Temperature is consistent during the main sequence
        float Temperature = Mathf.Pow(StarMass, 0.5f) * 5780;

        //get general luminosity from size and mass based temperature
        float BaseLuminosity = Mathf.Pow(BaseDiameter, 2) * Mathf.Pow((Temperature/5780), 4);
        //estimate stellar lifespan from diameter divided by luminosity
        float Lifespan = (BaseDiameter / BaseLuminosity) * 9;
        //grab systemage from barycenter
        float SystemAge = GameObject.Find("Barycentre").GetComponent<SystemGen>().SystemAge;
        //Age Ajustment is % though lifespan
        float AgeAjustment = SystemAge / Lifespan;

        //Overwrite Luminosity and Diameter now  knowing the age
        float Diameter = BaseDiameter + ((BaseDiameter / 2) * AgeAjustment);
        float Luminosity = Mathf.Pow(Diameter, 2) * Mathf.Pow((Temperature/5780), 4);
        
        //set boundaries of various temperature zones
        float CenterLine = Mathf.Sqrt(Luminosity);
        float AridLine = CenterLine * 0.95f;
        float OuterHabitableLine = CenterLine * 1.35f;
        float FrostLine = CenterLine * 4.8f;

        //produce magnetic values

        float CancerLine = Mathf.Pow(Luminosity, 0.3f);
        //set radiation zone bounds
        transform.GetChild(1).GetChild(3).localScale = new Vector3(CancerLine * 2000, CancerLine * 2000, CancerLine * 2000);
        //set arid zone bounds
        transform.GetChild(1).GetChild(2).localScale = new Vector3(AridLine * 2000, AridLine * 2000, AridLine * 2000);
        //set habitable zone bounds
        transform.GetChild(1).GetChild(1).localScale = new Vector3(OuterHabitableLine * 2000, OuterHabitableLine * 2000, OuterHabitableLine * 2000);
        //set frost line bounds
        transform.GetChild(1).GetChild(0).localScale = new Vector3(FrostLine * 2000, FrostLine * 2000, FrostLine * 2000);
        //point the indicators towards the camera
        transform.GetChild(1).transform.LookAt(GameObject.Find("MainCam").transform.position);
        

        //set size of the star itself relative to earth=1
        transform.GetChild(0).localScale = new Vector3(Diameter * 109, Diameter * 109, Diameter * 109);
        //set size of double click collider
        transform.GetComponent<SphereCollider>().radius = Diameter*1090;
        //All bodies are weighed where 1 = Earth
        float MassInEarth = StarMass * 333030;
        //get rigidbody and apply the mass
        transform.GetComponent<Rigidbody>().mass = MassInEarth;
    } 
}
