using System.Configuration;


/// <summary>
/// Summary description for CSXMLConfigManager
/// </summary>
/*
 * Return some application params from  web.config (section "appSettings")
 */

public class CSXMLConfigManager
{
    public static string GetDVUser()
    {
        return ConfigurationManager.AppSettings["CS_DV_User"];
    }


    public static string GetDVPassword()
    {
        return ConfigurationManager.AppSettings["CS_DV_Password"];
    }


    public static string GetDVServerURL()
    {
        return ConfigurationManager.AppSettings["CS_DV_URL"];
    }


    public static string GetDVDatabase()
    {
        return ConfigurationManager.AppSettings["CS_DV_DB"];
    }


    public static string GetDogDocumentFolderID()
    {
        return ConfigurationManager.AppSettings["CS_DOG_FOLDER"];
    }
	
	public static string GetTempMigration()
    {
        return ConfigurationManager.AppSettings["CS_TempMigration"];
    }
}