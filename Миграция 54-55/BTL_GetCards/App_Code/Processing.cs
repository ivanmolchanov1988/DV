using System.Web.Services;


/// <summary>
/// Summary description for TemplateFiles
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Processing : System.Web.Services.WebService
{

    public Processing()
    {

    }

    [WebMethod]
    public string GetListCards(string searchXML)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetListCards(searchXML);
    }

    [WebMethod]
    public string GetReferenceList(string ID)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetReferenceList(ID);
    }

    [WebMethod]
    public string GetNumericPart(string NumId)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetNumericPart(NumId);
    }

    [WebMethod]
    public string GetCreateDate(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetCreateDate(IDs);
    }
	
	[WebMethod]
    public string GetCardStoryes(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetCardStoryes(IDs);
    }


    [WebMethod]
    public string GetCards(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetCards(IDs);
    }
	
	 [WebMethod]
    public string GetRegNumbers(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetRegNumbers(IDs);
    }
	
	[WebMethod]
    public string GetCard(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetCards(IDs);
    }
	
	[WebMethod]
    public string GetFilesByCard(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetFilesByCard(IDs);
    }
	
	[WebMethod]
    public string GetDigest(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetDigest(IDs);
    }
	
	[WebMethod]
    public string GetDictionaryAndItemFromCard(string idX, string aliasSection, string indexRow, string aliasField)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetDictionaryAndItemFromCard(idX, aliasSection, indexRow, aliasField);
    }
	
	[WebMethod]
    public string GetDateTimeFieldFromCard(string idX, string aliasSection, string indexRow, string aliasField)
    {		
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetDateTimeFieldFromCard(idX, aliasSection, indexRow, aliasField);
    }
	
	[WebMethod]
    public string GetTomNameInArhivsCases(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetTomNameInArhivsCases(IDs);
    }
	
	[WebMethod]
    public string GetRegistrator(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetRegistrator(IDs);
    }
	
	[WebMethod]
    public string GetCardsIdByRefferenceId(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetReferenceListByID(IDs);
    }
	
	[WebMethod]
    public string GetRefferenceByCardsId(string parentCardID, string refCardID)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetRefferenceByCardsId(parentCardID, refCardID);
    }
	
	[WebMethod]
    public string GetCardKindNameByID(string IDs)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetCardKindNameByID(IDs);
    }
	
	[WebMethod]
    public string testGetTomNameInCards(string searchXML)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.testGetTomNameInCards(searchXML);
    }
	
	

    [WebMethod]
    public string GetEmployee(string ID)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetEmployee(ID);
    }

 [WebMethod]
    public string GetSectionByCardId(string IDs, string sectionAlias)
    {
        Methods methodUtils = new Methods(Application);

        return methodUtils.GetSectionByCardId(IDs, sectionAlias);
    }


}
