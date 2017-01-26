using System.Xml;
using System.IO;
using UnityEngine;

public class XmlDatabaseManager : MonoBehaviour {

    public string filepath;
    private static XmlDatabaseManager instance;

    void Awake()
    {
        instance = this;
        filepath = Application.dataPath + @"/Logs/PositionDataBase.xml"; //Define the location of the xml database
    }

    public static XmlDatabaseManager Instance
    {
        get
        {
            return instance;
        }
    }

    void CreateDataBaseXmL() // Create a xml file to store data
    {
        if (!File.Exists(filepath))
        {
            XmlDocument document = new XmlDocument(); 

            XmlDeclaration xmlDecl = document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XmlComment disclaimer = document.CreateComment("Data for research purpose only. Positions are given following this schema: x_leftHand,y_leftHand,z_leftHand;x_rightHand,y_rightHand,z_rightHand");
            XmlNode rootNode = document.CreateElement("games");

            document.AppendChild(xmlDecl);
            document.AppendChild(disclaimer);
            document.AppendChild(rootNode);
            
            document.Save(filepath);
        }
    }

    void InitXmlGame(string level, string playerID)// Initialize the data fields of a game (see the dedicated wiki page for more informations on the document structure)
    {
        XmlDocument document = new XmlDocument();
        document.Load(filepath);
        XmlNode rootNode = document.SelectSingleNode("//games");

        XmlNode gameNode = document.CreateElement("game");
        XmlAttribute gameID = document.CreateAttribute("ID");
        int NewGameId = document.SelectNodes("//games/game").Count + 1;
        gameID.Value = NewGameId.ToString();
        gameNode.Attributes.Append(gameID);

        XmlNode playerNode = document.CreateElement("player");
        XmlAttribute playerIDAttribute = document.CreateAttribute("ID");
        playerIDAttribute.Value = playerID;
        playerNode.Attributes.Append(playerIDAttribute);

        XmlNode preferencesNode = document.CreateElement("preferences");

        XmlNode scoreNode = document.CreateElement("score");
        scoreNode.InnerText = "0";

        XmlNode levelNode = document.CreateElement("level");
        levelNode.InnerText = level;

        XmlNode positionNode = document.CreateElement("positions");

        XmlNode timeNode = document.CreateElement("time");
        XmlNode beginNode = document.CreateElement("begin");
        XmlNode endNode = document.CreateElement("end");
        beginNode.InnerText = System.DateTime.Today.ToLongDateString()+";"+System.DateTime.Now.ToLongTimeString();

        timeNode.AppendChild(beginNode);
        timeNode.AppendChild(endNode);


        playerNode.AppendChild(preferencesNode);
        gameNode.AppendChild(playerNode);
        gameNode.AppendChild(levelNode);
        gameNode.AppendChild(scoreNode);
        
        gameNode.AppendChild(timeNode);
        gameNode.AppendChild(positionNode);

        rootNode.AppendChild(gameNode);




     

        document.Save(filepath);

    } 

    string SaveTempPositions(string save, Vector3[] positions)// A function designed to manage temporary positions save easier
    {
        string newEntry = positions[0][0].ToString()+","+positions[0][1].ToString() + ","+positions[0][2].ToString() + ","+positions[1][0].ToString() + ","+positions[1][1].ToString() + ","+positions[1][2].ToString() +";";
        return (save + newEntry);
    } 

    void WriteTempPositions(string save)// Write the temporary positions save in the database
    {
        XmlDocument document = new XmlDocument();
        document.Load(filepath);
        XmlNode rootNode = document.SelectSingleNode("//games");
        XmlNode currentGame = rootNode.LastChild;
        XmlNode positions = currentGame.SelectSingleNode("positions");
        positions.InnerText = save;

        document.Save(filepath);
    } 

    void WriteData(string score)//Write other data in the database
    {
        XmlDocument document = new XmlDocument();
        document.Load(filepath);
        XmlNode rootNode = document.SelectSingleNode("//games");
        XmlNode currentGame = rootNode.LastChild;
        XmlNode scoreNode = currentGame.SelectSingleNode("score");
        XmlNode timeNode = currentGame.SelectSingleNode("time");
        XmlNode end = timeNode.SelectSingleNode("end");
        scoreNode.InnerText = score;
        end.InnerText = System.DateTime.Today.ToLongDateString() + ";" + System.DateTime.Now.ToLongTimeString();

        document.Save(filepath);
    } 
}
