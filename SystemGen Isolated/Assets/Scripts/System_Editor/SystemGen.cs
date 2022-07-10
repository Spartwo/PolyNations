using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class SystemGen : MonoBehaviour
{
    //declare prefab bodies to instantiate
    public GameObject AsteroidPrefab;
    public GameObject CometPrefab;
    public GameObject RingPrefab;
    public GameObject BodyPrefab;
    public GameObject StarPrefab;
    //declare the animation curve for star mass generation
    public AnimationCurve StellarMassProbabilityCurve;
    //declare seed input from UI and
    protected string SeedInput;
    protected string SystemFileName;

    //declare system age as 0 and work from thhre
    public float SystemAge = 0;
    
    //system loaddata
    protected List<string> SystemDataArray;

    void Start()
    {
        //create folder on initialisation incase of total deletion
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Star_Systems/");

        SetSystemName();
    }

    public void SaveSystemFile()
    {   
        //function to save system file post-generation
        //create the system file in the extant directory from Start()
        SystemFileName = Application.streamingAssetsPath 
            + "/Star_Systems/" 
            + (GameObject.Find("Seed_Input").GetComponent<TMP_InputField>().text) 
            + ".system";

        //overwrite values
        // File.WriteAllText(SystemFileName, "UsableSeed = " + UsableSeed);
        // File.AppendAllText(SystemFileName, InitGenConfig);
    }

    public void LoadSystemFile()
    {   
        string SystemName = GameObject.Find("Seed_Input").GetComponent<TMP_InputField>().text;
        //create the system file in the extant directory from Start()
        SystemFileName = Application.streamingAssetsPath 
            + "/Star_Systems/" 
            + (GameObject.Find("Seed_Input").GetComponent<TMP_InputField>().text) 
            + ".system";
        
        //read all save data to a list
        SystemDataArray = File.ReadAllLines(SystemFileName).ToList();

        //for each Star
        for (int i = 0; i < int.Parse(ReturnFileValue(1)) ; i++) // or count if it is a list
        {
            //Instantiate Stars and pass their unique ID and the system file to their script
            GameObject obj = Instantiate(StarPrefab);
            obj.transform.SetParent(this.gameObject.transform, false);
            //set the starmanager unique identifier
            obj.GetComponent<StarManager>().StarSearchTerm = "STAR_" + (i+1);
            //set the starmanager input filename
            obj.GetComponent<StarManager>().SystemFileName = SystemName;
        }

        //for each planet
        for (int i = 0; i < int.Parse(ReturnFileValue(2)) ; i++) // or count if it is a list
        {
         /*   //Instantiate Stars and pass their unique ID and the system file to their script
            GameObject obj = Instantiate(BodyPrefab);
            obj.transform.SetParent(this.gameObject.transform, false);
            //set the starmanager unique identifier
            obj.GetComponent<BodyManager>().BodySearchTerm = "BODY_" + (i+1);
            //set the starmanager input filename
            obj.GetComponent<BodyManager>().SystemFileName = SystemName;*/
        }

       // GameObject.Find("Camera_Focus").GetComponent<CameraMovement>().UpdateBodyList();
        
    }

    //return value from a certain line of systemdata
    string ReturnFileValue(int index)
    {
        int FilePoint = SystemDataArray[index].IndexOf("=");
        return(SystemDataArray[index].Substring(FilePoint+2));
    }
    public void SetSystemName()
    {
        //generate if no custom provided
        //import list from text file
        string NameGenerationFilePath = Application.streamingAssetsPath + "/Localisation/System_Names.txt";
        List<string> StarNameArray = File.ReadAllLines(NameGenerationFilePath).ToList();

        //choose position in new array
        int RandNamePosition = Random.Range(1,(StarNameArray.Count)); 
        int RandNumber = Random.Range(1,999);
        //retrieve name from position
        string RandName = StarNameArray[RandNamePosition];
        //apply to name
        string NameInput = RandName + "-" + RandNumber;
        GameObject.Find("Seed_Input").GetComponent<TMP_InputField>().text = NameInput;
    }
    public void GenerateSystemFile()
    {
        string SeedInput;
        SeedInput = GameObject.Find("Seed_Input").GetComponent<TMP_InputField>().text;
        //generate system seed
        if (SeedInput == "") 
        {
            SetSystemName();
        }

        //code for converting provided characters into a usable seed
        int UsableSeed = SeedInput.GetHashCode();
        //set appropriate file name to the name value
        SystemFileName = Application.streamingAssetsPath 
            + "/Star_Systems/" 
            + SeedInput 
            + ".system";
        //overwrite values as new seed
        File.WriteAllText(SystemFileName, "System Name = " + SeedInput + "\n");
        
        //set system seed to the converted value
        Random.InitState(UsableSeed); 

        int StarDictator = Random.Range(1, 1000);
        int StarCount;
        //assign number of stars
        if (StarDictator > 50) {
            StarCount = 1;
            //set single star distance at centre
            float ABDistance = 0;
            if (StarDictator > 180) {
                StarCount = 2;
                //generate binary seperation
                ABDistance = Random.Range(1F, 1000F)/10;
                //Eccentricity capacity increases with distance
                float ABEccentricity = Random.Range(0f, (ABDistance*0.005f));
                if (StarDictator > 960) {
                    StarCount = 3;
                    //reduce binary seperation
                    ABDistance = Random.Range(1F, 1000F)/10;
                    //generate p-type seperation of body 3
                    float CDistance = Random.Range(ABDistance*30,3000F)/10;
                    float CEccentricity = Random.Range(0f, 0.5f);
                    float CInclination = Random.Range(0f, 359f);
                }
            }
        } else {
            StarCount = 0;
            //rogue planet with no parent star
            //PlanetGen(0,10,0);
        }

        
        
        File.AppendAllText(SystemFileName, "Stars = " + StarCount + "\n");
        //temp for debugging
        int BodyCount = 1;
        File.AppendAllText(SystemFileName, "Planets = " + BodyCount + "\n");

        //reset star ages to current time maximum
        float[] StarLifespans = {13.5F, 13.5F, 13.5F, 13.5F};
        //call the generation method for each star
        for (int i = 0; i < StarCount; i++)
        {
            //adds to array for comparison and calls generation method
           StarLifespans[i] = StarGen(i+1);
        }
        //Gets a reasonable current age between universe age and too young to support planets for all stars
        SystemAge = Random.Range(50, Mathf.Min(10000, StarLifespans.Min() * 900)) / 1000;
        Debug.Log("System Age" + SystemAge);

        

    
    }

    float StarGen(int StarNumber) 
    {
        //Generate Stellar mass using an animation curve
        float MassDictator = Random.Range(0.5f, 5f);
        float Mass = StellarMassProbabilityCurve.Evaluate(MassDictator);
        //Diameter at formation
        float Diameter = Mathf.Pow(Mass, 0.7f);
        //get general luminosity from size and mass based temperature
        float Luminosity = Mathf.Pow(Diameter, 2) * Mathf.Pow((Mathf.Pow(Mass, 0.5f)), 4);
        //estimate stellar lifespan from diameter divided by luminosity
        float Lifespan = (Diameter / Luminosity) * 10;


        //generate number of planets
        int PlanetCount = (int)(Mathf.Max((Mathf.Pow(Mass,0.3f)*Random.Range(1,10)),1));
        //estimated distance of the heliopause
        float SOIEdge = Mathf.Sqrt(Luminosity)*150;
        
        return Lifespan;
    }

    void Star1Gen()
    {

    } 
    void Star2Gen()
    {

    } 
    void Star3Gen()
    {

    } 
    void StarGen2() {
        
        
        //Calculate Hill Sphere in AU
        //float BHillSphere = ABDistance*(1-ABEccentricity)*(BMass/3*AMass)^0.4


        //revise stellar properties with respect to age
        //Age Ajustment is % though lifespan
       // float AgeAjustment = SystemAge / Lifespan;
       // Diameter = Diameter + ((Diameter / 2) * AgeAjustment);
        //Temperature in Kelvin Luminosity in Stellar Measures
      //  Temperature = mathf.Pow(Diameter*1.25,0.54) * 5780;
      //  Luminosity = mathf.Pow(AgeAdjDiameter, 2) * mathf.Pow((Temperature / 5780), 4);

        //calculate mean eccentricity
       // float MeanEccentricity = mathf.Max((PlanetCount^(-0.15)-0.65), 0.02);
        //for each planet calculate the available mass
      //  for (int x = 0; x < PlanetCount; x++) 
      //  {
            //Pass parent body and orbital limitations
           // PlanetGen(i+1, HillRadius, SOIEdge);

      //  }
       // InitGenConfig = InitGenConfig + "STAR_" + StarNumber + "\n{\n\tObject = " + StarNumber + "\n\tStellar Mass = " + Mass + "\n\tChild Bodies = " + PlanetCount + "\n\tHelioPause Distance (AU) = " + SOIEdge + "\n\tRoche Limit (AU) = " + RocheLimit + "\n}\n";  
    }

    //float PlanetGen(int ParentBody, float PlanetSMA, int BodyNumber) {
        //Generate Eccentricity

        //Generate 
        
    //}
    void SetOrbit(
        float SMAInput,
        float MeanEccentricity,
        float MaxInclination,
		out float SMA, 
		out float Eccentricity,
		out float LongitudeOfAscending,
		out float Inclination,
		out float PeriArgument 
        )
    {
        //Jiggle the orbit a little within a permissable range
        SMAInput += Random.Range(-0.03f, 0.03f)*SMAInput;
        SMA = SMAInput; 
        //Generate Eccentricity with a basis in the mean solar eccentricity
        //Mean is generated by planetcount and isn't a tally of the actual mean
        Eccentricity = Random.Range(0.001f, MeanEccentricity*2);
        //Generate the angle where the orbit goes from below the equator to above it
        LongitudeOfAscending = Random.Range(0f, 359f);
        //Generate the inclination of the body in a given range
        Inclination = Random.Range(-MaxInclination, MaxInclination);
        //Generate how far in the inclined disk the body will set it's periapse
        PeriArgument = Random.Range(0f, 359f);
    }

}