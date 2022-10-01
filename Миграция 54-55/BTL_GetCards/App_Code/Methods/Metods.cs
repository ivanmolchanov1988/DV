using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectManager.Metadata;
using DocsVision.Platform.ObjectManager.SearchModel;
using DocsVision.Platform.ObjectManager.SystemCards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DocsVision.BackOffice.Localization;
using System.Reflection;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using DocsVision.BackOffice.ObjectModel;

/// <summary>
/// Summary description for WordUtils
/// </summary>
public class Methods
{
    public Context context = null;
    private CardData _cardDataRefPartners = null;
    private SectionData _secEmployees = null;
    private readonly string[] aliasesEmployeesLog = new string[] { "FROMEMPLOYEE", "TOEMPLOYEE" };
    private readonly string[] aliasesTypesLog = new string[] { "ISCOPY", "ISRECEIVED" };

    public Methods(HttpApplicationState application)
    {
        context = new Context(application);

        _cardDataRefPartners = context.userSession.CardManager.GetCardData(new Guid("{6710B92A-E148-4363-8A6F-1AA0EB18936C}"));
        _secEmployees = _cardDataRefPartners.Sections[new Guid("DBC8AE9D-C1D2-4D5E-978B-339D22B32482")];
    }


    private readonly string[] excludingSection = new string[]
    {
           "Files"
    };

 	internal string GetSectionByCardId(string IDs, string sectionAlias)
    {
        List<XCards> listXCards = new List<XCards>();

        foreach (string id in IDs.Split(','))
        {
            CardData card;
            try
            {
                card = context.userSession.CardManager.GetCardData(new Guid(id));
                XCards xCard = new XCards();

                var p = getSectionData(card, sectionAlias);
                if (p != null){
			 p.Section = sectionAlias;
                    xCard.Sections.Add(p);
		}
                listXCards.Add(xCard);
            }
            catch
            {

            }

        }

        return SerializeHelper.getXmlForObject<List<XCards>>(listXCards);
    }

    internal string GetEmployee(string ID)
    {
        CardData card = context.userSession.CardManager.GetCardData(new Guid("6710B92A-E148-4363-8A6F-1AA0EB18936C"));

        var sectionFields = card.Type.AllSections.FirstOrDefault(s => s.Alias.ToLower() == "Employees".ToLower());

        XCards xCard = new XCards();
        XSection xSection = new XSection();
        xCard.Sections.Add(xSection);
        xSection.Section = "Employees";

        SectionData employees = card.Sections[card.Type.AllSections["Employees"].Id];

        if (employees.RowExists(new Guid(ID)))
        {
            RowData row = employees.GetRow(new Guid(ID));

            XRow xRow = new XRow();

            foreach (Field field in sectionFields.Fields)
            {
                if (String.IsNullOrWhiteSpace(row[field.Alias] + "")) continue;
				if (!(new string[] {"AccountName", "LastName", "FirstName", "MiddleName"}).Contains(field.Alias)) continue;


                XCell cell = new XCell();

                cell.Alias = field.Alias;
                cell.DataType = field.Type + "";
                cell.ShortValue = row[cell.Alias] + "";
                xRow.Cells.Add(cell);

                xSection.Rows.Add(xRow);
            }
        }
        return SerializeHelper.getXmlForObject<XCards>(xCard);
    }


	public string GetCreateDate(string ID){		
		Document document;
		try            
		{
			document = context.objectContext.GetObject<Document>(new Guid(ID));
			return document.CreateDate+"";
		} catch 
		{
			return "";
		}
	}
	
	public string GetCardStoryes(string ID){
		try            
		{
			CardData card = context.userSession.CardManager.GetCardData(new Guid(ID));			
			List<XStory> xStopy = getStoryes(card);
			return SerializeHelper.getXmlForObject<List<XStory>>(xStopy);
		} catch 
		{
			return "";
		}
	}


    internal string GetListCards(string searchXML)
    {
        List<string> cardsResult = new List<string>();


		List<CardData> documentsID = new List<CardData>();
        SearchQuery searchQuery = context.userSession.CreateSearchQuery();
        searchQuery.ParseXml(searchXML);
        string query = searchQuery.GetXml();
        // Выполнение запроса
        CardDataCollection coll = context.userSession.CardManager.FindCards(query);
        if (coll != null && coll.Count > 0)
        {
            foreach (var c in coll)
            {
				cardsResult.Add(c.Id + "");
            }
			return String.Join(",", cardsResult.ToArray());
        }

        return "";
    }


    internal string GetCards(string IDs)
    {
        List<XCards> listXCards = new List<XCards>();

        foreach (string id in IDs.Split(',')) {
            CardData card;
            try            
            {
                card = context.userSession.CardManager.GetCardData(new Guid(id));
		XCards xCard = new XCards();


            	foreach (CardSection section in card.Type.Sections)
            	{
                	if (excludingSection.Contains(section.Alias)) continue;
                	var p = getSectionData(card, section.Alias);
                	if (p != null)
                	    xCard.Sections.Add(p);

            	}

           	 xCard.XStoryes = getStoryes(card);
            	
            	listXCards.Add(xCard);
            } catch 
            {
                
            }

            
        }

        return SerializeHelper.getXmlForObject<List<XCards>>(listXCards);
    }
	
	internal string testGetTomNameInCards(string @searchXML)
    {
         List<string> cardsResult = new List<string>();


		List<CardData> documentsID = new List<CardData>();
        SearchQuery searchQuery = context.userSession.CreateSearchQuery();
        searchQuery.ParseXml(searchXML);
        string query = searchQuery.GetXml();
        // Выполнение запроса
        CardDataCollection coll = context.userSession.CardManager.FindCards(query);
        if (coll != null && coll.Count > 0)
        {			
            foreach (var c in coll)
            {
				string tomaName = GetTomNameInArhivsCases(c.Id + "");
				if (tomaName==null)
				{
					tomaName = "Error";
				}
				
				
				if (!cardsResult.Contains(tomaName + ""))
                {
					cardsResult.Add(tomaName + "");
				}
            }
			return String.Join(",", cardsResult.ToArray());
        }

        return "";
    }
	
	internal string GetTomNameInArhivsCases(string ID)
    {
        CardData card = context.userSession.CardManager.GetCardData(new Guid(ID));
        RowData cs = card.Sections[new Guid("{30EB9B87-822B-4753-9A50-A1825DCA1B74}")].FirstRow;

        Guid? caseId = cs.GetGuid("CaseId");
        if (caseId.HasValue && caseId.Value != Guid.Empty)
        {
            CardData cardCase;
            try
            {
                cardCase = context.userSession.CardManager.GetCardData(caseId.Value);
            } catch
            {
                return "";
            }
            
            var cds = cardCase.Sections[new Guid("{3486E1F4-AACC-4C68-9093-FEA1E14A6549}")];
            if (cds.Rows.Count() > 0)
            {
                /*string res = cds.FirstRow["Article"]+"";
				if (String.IsNullOrWhiteSpace(res)){
					res = cds.FirstRow["Title"]+"";
				}*/
                //return res;
                return cds.FirstRow["Title"] + "";
            }
        }
        return "";
    }
	
	internal string GetDateTimeFieldFromCard(string idX, string aliasSection, string indexRow, string aliasField)
    {
		int indexRow32 = Int32.Parse(indexRow);	
		CardData card = context.userSession.CardManager.GetCardData(new Guid(idX));	
		var cd = card.Sections[card.Type.Sections[aliasSection].Id];
		
		bool isFieldExist = false;
        foreach (Field field in cd.Type.Fields)
        {
            if (field.Alias.ToLower() == aliasField.ToLower())
            {
                isFieldExist = true;
                break;
            }
        }

        if (!isFieldExist)
            return "";
		
		return cd.Rows[indexRow32][aliasField]+"";
    }
	
	public string GetDictionaryAndItemFromCard(string idX, string aliasSection, string indexRow, string aliasField)
    {
		int indexRow32 = Int32.Parse(indexRow);	
		CardData card = context.userSession.CardManager.GetCardData(new Guid(idX));	
		var cd = card.Sections[card.Type.Sections[aliasSection].Id];
		
		RowData row = cd.Rows[indexRow32];
        bool isFieldExist = false;
        foreach (Field field in cd.Type.Fields)
        {
            if (field.Alias.ToLower()== aliasField.ToLower())
            {
                isFieldExist = true;
                break;
            }
        }

        if (!isFieldExist)
            return null;
		
		string valueId = cd.Rows[indexRow32][aliasField]+"";
		if (!String.IsNullOrWhiteSpace(valueId))
		{
			Guid Id = new Guid(valueId);
			if (Id != null && Id != Guid.Empty)
			{
				CardData universalItem = context.userSession.CardManager.GetDictionaryData(new Guid("4538149D-1FC7-4D41-A104-890342C6B4F8"));
				SectionData unit = universalItem.Sections[new Guid("A1DCE6C1-DB96-4666-B418-5A075CDB02C9")];
				foreach (RowData rowItemType in unit.GetAllRows())
				{
					SubSectionData items = rowItemType.ChildSections[new Guid("1B1A44FB-1FB1-4876-83AA-95AD38907E24")];
					foreach (RowData rowItems in items.GetAllRows())
					{
						if (rowItems.Id == Id)
						{
							return rowItemType["Name"]+","+rowItems["Name"];
						}
					}

				}
			}
		}
		
		return null;
    }
	

    internal string GetNumericPart(string NumericPartID)
    {
        XCards xCard = new XCards();

        XSection p = new XSection();
        p.Section = "Numeric";
        xCard.Sections.Add(p);
        XRow xRow = new XRow();

        // карточка нумератора
        CardData numeratorCardData = context.userSession.CardManager.GetCardData(new Guid(NumericPartID), new Guid("D47F2C38-6553-4864-BAFF-0BC4D3A85290"));
        // строка номера
        RowData numberRow = numeratorCardData.Sections[new Guid("D47F2C38-6553-4864-BAFF-0BC4D3A85290")].GetRow(new Guid(NumericPartID));
        // числовое значение номера
        int? numberValue = numberRow.GetInt32("Number");


        // Зона
        Guid? zoneRowId = numberRow.GetGuid("ParentRowID");  // id зоны, в котором строка номера
        RowData zoneRow = numeratorCardData.Sections[new Guid("916CDAB9-1FDA-4D0A-935F-6492C75477A8")].GetRow(zoneRowId.Value);
        String zoneName = zoneRow.GetString("ZoneName");

        // атрибуты нумератора
        RowData numeratorAtrRow = numeratorCardData.Sections[new Guid("7A357C7B-7C36-48C8-8008-294B00F48AB2")].FirstRow;
        String numeratorName = numeratorAtrRow.GetString("Name");

        XCell cell = new XCell();
        cell.Alias = "NumeratorName";
        cell.ShortValue = numeratorName;
        xRow.Cells.Add(cell);

        XCell cell2 = new XCell();
        cell2.Alias = "ZoneName";
        cell2.ShortValue = zoneName;
        xRow.Cells.Add(cell2);

        XCell cell3 = new XCell();
        cell3.Alias = "NumberValue";
        cell3.ShortValue = numberValue;
        xRow.Cells.Add(cell3);

        p.Rows.Add(xRow);

        return SerializeHelper.getXmlForObject<XCards>(xCard);
    }
	
	public string GetCardKindNameByID(string ID)
    {
		Document docKind;
		try            
		{
			docKind = context.objectContext.GetObject<Document>(new Guid(ID));
			return docKind.Description;
		} catch 
		{
			return "";
		}
    }
	
	public string GetReferenceListByID(string ID)
    {
		XCards xCard = new XCards();

        XSection p = new XSection();
        p.Section = "References";
        xCard.Sections.Add(p);
		
		
		CardData card = context.userSession.CardManager.GetCardData(new Guid(ID));
		var References = card.Sections[card.Type.Sections["References"].Id];
        if (References.Rows.Count>0){
			foreach(var r in References.Rows){				
				
				ReferenceListReference rlr = context.objectContext.GetObject<ReferenceListReference>(r.Id);
				
				XRow xRow = new XRow();

                XCell cell = new XCell();
                cell.Alias = "CreationDate";
                cell.ShortValue = rlr.CreationDate;
                xRow.Cells.Add(cell);

                XCell cell2 = new XCell();
                cell2.Alias = "Author";
                cell2.ShortValue = rlr.Author != null ? rlr.Author.GetObjectId() + "" : null;
                xRow.Cells.Add(cell2);


                XCell cell3 = new XCell();
                cell3.Alias = "LinkName";
                cell3.ShortValue = rlr.Type.LinkName;
                xRow.Cells.Add(cell3);

                XCell cell4 = new XCell();
                cell4.Alias = "OppositeLinkName";
                cell4.ShortValue = rlr.Type.OppositeLinkName;
                xRow.Cells.Add(cell4);

                XCell cell5 = new XCell();
                cell5.Alias = "Card";
                cell5.ShortValue = rlr.Card;
                xRow.Cells.Add(cell5);

                if (rlr.Card != null)
                {
                    Document docKind = context.objectContext.GetObject<Document>(rlr.Card);
		    if (docKind!=null){

                    		XCell cell6 = new XCell();
                    		cell6.Alias = "CardKind";
                    		cell6.ShortValue = docKind.SystemInfo.CardKind.Name;
                    		xRow.Cells.Add(cell6);
			}
                }

                p.Rows.Add(xRow);
			}			
		}
        return SerializeHelper.getXmlForObject<XCards>(xCard);
    }
	
	public string GetRefferenceByCardsId(string parentCardID, string refCardID)
    {		
		XCards xCard = new XCards();

        XSection p = new XSection();
        p.Section = "References";
        xCard.Sections.Add(p);
		
		Document editingDoc = context.objectContext.GetObject<Document>(new Guid(parentCardID));
			if (editingDoc.MainInfo.ReferenceList != null && editingDoc.MainInfo.ReferenceList.References.Count > 0)
			{
				foreach (ReferenceListReference rlr in editingDoc.MainInfo.ReferenceList.References)
				{
					if (rlr.Card == new Guid(refCardID))
					{
						XRow xRow = new XRow();
						XCell cell = new XCell();
						cell.Alias = "CreationDate";
						cell.ShortValue = rlr.CreationDate;
						xRow.Cells.Add(cell);

						XCell cell2 = new XCell();
						cell2.Alias = "Author";
						cell2.ShortValue = rlr.Author != null ? rlr.Author.GetObjectId() + "" : null;
						xRow.Cells.Add(cell2);


						XCell cell3 = new XCell();
						cell3.Alias = "LinkName";
						cell3.ShortValue = rlr.Type.LinkName;
						xRow.Cells.Add(cell3);

						XCell cell4 = new XCell();
						cell4.Alias = "OppositeLinkName";
						cell4.ShortValue = rlr.Type.OppositeLinkName;
						xRow.Cells.Add(cell4);

						XCell cell5 = new XCell();
						cell5.Alias = "Card";
						cell5.ShortValue = rlr.Card;
						xRow.Cells.Add(cell5);

						if (rlr.Card != null)
						{
							Document docKind = context.objectContext.GetObject<Document>(rlr.Card);

							XCell cell6 = new XCell();
							cell6.Alias = "CardKind";
							cell6.ShortValue = docKind.SystemInfo.CardKind.Name;
							xRow.Cells.Add(cell6);
						}

						p.Rows.Add(xRow);

					}
				}
			}
		return SerializeHelper.getXmlForObject<XCards>(xCard);
    }

    internal string GetReferenceList(string ID)
    {
        Document doc = context.objectContext.GetObject<Document>(new Guid(ID));
		if (doc.MainInfo.ReferenceList == null)
		{
			return "";
		}
		return GetReferenceListByID(doc.MainInfo.ReferenceList.GetObjectId()+"");
    }

    private List<XStory> getStoryes(CardData card)
    {
        List<XStory> result = new List<XStory>();

        //Получение документа
        Document document = context.objectContext.GetObject<Document>(card.Id);

        //Получение сервиса журналирования
        ILogService logService = context.objectContext.GetService<ILogService>();
        logService.AddMessageResources(LocalizationManager.GetLibraryId(),
            DocsVision.BackOffice.Localization.Resources.ResourceManager);


        foreach (LogMessage logMessage in logService.GetLogMessages(null, null, context.objectContext.GetObject<BaseCard>(card.Id)).Reverse())
        {
            XStory XStory = new XStory();
            XStory.Description = String.Format("{0}. {1}. {2}",
                logMessage.Date, logMessage.EmployeeName, logService.GetLogMessageDescription(logMessage));

            result.Add(XStory);
        }
        return result;
    }
	
	public string GetFilesByCard(string ID){		
		CardData card;		

		try            
		{
			card = context.userSession.CardManager.GetCardData(new Guid(ID));
		
			List<XFile> xCard = getFiles(card);		
			return SerializeHelper.getXmlForObject<List<XFile>>(xCard);
		} catch 
			{
				return "";
			}
	}
	
	public string GetDigest(string ID){		
		Document document = context.objectContext.GetObject<Document>(new Guid(ID));		
		return document.Description;
	}

    private List<XFile> getFiles(CardData card)
	{
		List<XFile> result = new List<XFile>();

		RowDataCollection files = card.Sections[card.Type.Sections["Files"].Id].Rows;

		foreach (RowData file in files)
		{
			Guid fileId = file.GetGuid("FileId") ?? Guid.Empty;
			if (fileId == Guid.Empty)
				continue;

			VersionedFileCard vFileCard = (VersionedFileCard)card.Session.CardManager.GetCard(fileId);		   
		   
			string tmpFolder = CSXMLConfigManager.GetTempMigration();
            string path = Path.Combine(tmpFolder, Guid.NewGuid() + "");

			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
			dir.Create();

			XFile xfile = new XFile();
			xfile.CurrentVersionID = vFileCard.CurrentVersion.Id + "";
			xfile.CheckinDate = vFileCard.CheckinDate + "";
			try
			{
			   
				foreach (var v1 in vFileCard.Versions)
				{
					foreach (var v2 in v1.Versions)
					{
						if (v2.Versions.Count > 0)
						{
							throw new Exception("В карточке: "+ card.Id +" файл содержит версии вложения более 1. Web клиент не поддерживает");
						}
					}
				}  

				if (vFileCard.CurrentVersion.Size>20971520 && vFileCard.Versions.Count>2){

					FileVersion currentVersion = vFileCard.CurrentVersion;
					XFileVersion xVersion = createVersion(file, dir, xfile, currentVersion);
					foreach (FileVersion subVersion in currentVersion.Versions)
					{
						xVersion.SubVersion.Add(createVersion(file, dir, xfile, subVersion));
					}
					xfile.XFileVersions.Add(xVersion);
					
				} else 
				{				
					foreach (FileVersion currentVersion in vFileCard.Versions)
					{
						XFileVersion xVersion = createVersion(file, dir, xfile, currentVersion);
						foreach (FileVersion subVersion in currentVersion.Versions)
						{
							xVersion.SubVersion.Add(createVersion(file, dir, xfile, subVersion));
						}
						xfile.XFileVersions.Add(xVersion);
					}
				}	

				
				
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally {
				Directory.Delete(path, true);

                if (Directory.Exists(path))
                {
                    throw new Exception("Папка не была удалена: "+path);
                }
			}
			result.Add(xfile);
		}
		return result;
	}

	private XFileVersion createVersion(RowData file, DirectoryInfo dir, XFile xfile, FileVersion currentVersion)
	{
		string filePath = dir + "\\" + currentVersion.Name;

		currentVersion.Download(filePath);
		var s64 = File.ReadAllBytes(filePath);
		XFileVersion xVersion = new XFileVersion();
		if (s64.Length > 0)
		{
			xVersion.CurrentVersionID = currentVersion.Id + "";
			xVersion.FileName = currentVersion.Name;
			xVersion.isMain = file["FileType"] + "" == "0" ? true : false;
			xVersion.Data = s64;

			if (currentVersion.AuthorId != null)
			{
				xVersion.AuthorData = GetEmployee(currentVersion.AuthorId + "");
			}

			xVersion.CreateDate = currentVersion.CreateDate + "";
			xVersion.ChangeDate = currentVersion.ChangeDate + "";

			foreach (var com in currentVersion.Comments)
			{
				XFileComments xFileComments = new XFileComments();
				xFileComments.AuthorData = GetEmployee(com.AuthorId + "");
				xFileComments.Comment = com.Comment;
				xFileComments.CreationDate = com.CreationDate + "";
				xVersion.xFileVersionComments.Add(xFileComments);
			}                
		}
		return xVersion;
	}

	
    private XSection getSectionData(CardData card, string sectionAlias)
    {
        XSection xSection = new XSection();

        var sectionFields = card.Type.Sections.FirstOrDefault(s => s.Alias.ToLower() == sectionAlias.ToLower());
        if (sectionFields == null)
        {
            return xSection;
        }

        SectionData section = card.Sections[card.Type.Sections[sectionAlias].Id];
        if (section.Rows.Count <= 0)
            return null;

        xSection.Section = sectionAlias;

        foreach (RowData row in section.Rows)
        {
            // string s2 = "";
            XRow xRow = new XRow();

            foreach (Field field in sectionFields.Fields)
            {
                if (String.IsNullOrWhiteSpace(row[field.Alias] + "")) continue;

                XCell cell = new XCell();

                cell.Alias = field.Alias;
                cell.DataType = field.Type + "";
                cell.ShortValue = row[cell.Alias] + "";

                // Если это ссылочный тип           
                if (field.Type == FieldType.RefId)
                {
                    CardType linkedCardType = field.LinkedCardType;

                    if (linkedCardType != null && field.LinkedSection != null)
                    {
                        if (linkedCardType.Alias == "RefBaseUniversal") // Если это справочник костуркторов
                        {

                            Guid Id = new Guid(cell.ShortValue + "");
                            if (Id != null && Id != Guid.Empty)
                            {
                                CardData universalItem = context.userSession.CardManager.GetDictionaryData(new Guid("4538149D-1FC7-4D41-A104-890342C6B4F8"));
                                SectionData unit = universalItem.Sections[new Guid("A1DCE6C1-DB96-4666-B418-5A075CDB02C9")];
                                foreach (RowData rowItemType in unit.GetAllRows())
                                {
                                    SubSectionData items = rowItemType.ChildSections[new Guid("1B1A44FB-1FB1-4876-83AA-95AD38907E24")];

                                    foreach (RowData rowItems in items.GetAllRows())
                                    {
                                        if (rowItems.Id == Id)
                                        {
                                            XRow xRowRefBaseUniversal = new XRow();
                                            xRowRefBaseUniversal.Cells.Add(new XCell() { ShortValue = rowItems["Name"], Alias = "ItemName" });
                                            xRowRefBaseUniversal.Cells.Add(new XCell() { ShortValue = rowItemType["Name"], Alias = "ItemTypeName" });
                                            cell.AdditionalValue = xRowRefBaseUniversal;
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            cell.AdditionalValue = getRefSectionDictionary(field, cell, linkedCardType);
                        }

                    }
                }

                xRow.Cells.Add(cell);
            }

            xSection.Rows.Add(xRow);
        }

        return xSection;
    }

    private object getRefSectionDictionary(Field field, XCell cell, CardType linkedCardType)
    {

        object resultData = null;

        /* Guid g;
                 if (!Guid.TryParse(this.tbId.Text, out g))
                 {
                     MessageBox.Show("Неправильный формат идентификатора");
                     return;
                 }*/


        CardData cardDataDic = context.userSession.CardManager.GetDictionaryData(linkedCardType.Id); // Справочник сотрудников или контрагентов и т.д.                       

        SectionData sectionFounded = null;
        foreach (CardSection sectionMD in cardDataDic.Type.AllSections)
        {
            SectionData sectionDataTmp = cardDataDic.Sections[sectionMD.Id];
            if (sectionDataTmp.RowExists(new Guid(cell.ShortValue + "")))
            {
                sectionFounded = sectionDataTmp;
                break;
            }
        }


        if (sectionFounded != null)
        {
            var sectionFoundedFields = cardDataDic.Type.AllSections.FirstOrDefault(s => s.Alias.ToLower() == field.LinkedSection.Alias.ToLower());
            if (sectionFounded.RowExists(new Guid(cell.ShortValue + "")))
            {
                try
                {
                    RowData rowAny = sectionFounded.GetRow(new Guid(cell.ShortValue + ""));
                    if (rowAny != null)
                    {
                        XRow xRowAny = new XRow();
                        foreach (Field fieldUnit in sectionFoundedFields.Fields)
                        {
                            xRowAny.Cells.Add(new XCell() { ShortValue = rowAny.GetString(fieldUnit.Alias), Alias = fieldUnit.Alias });
                        }
                        resultData = xRowAny;
                    }
                }
                catch { }
            }
        }

        return resultData;
    }
	
	public string GetRegistrator(string ID)
	{
		XCards xCard = new XCards();
		XSection p = new XSection();
		p.Section = "Numeric";
		xCard.Sections.Add(p);
		XRow xRow = new XRow();
		//Получение сервиса журналирования
		ILogService logService = context.objectContext.GetService<ILogService>();
		logService.AddMessageResources(LocalizationManager.GetLibraryId(),
			DocsVision.BackOffice.Localization.Resources.ResourceManager);
		foreach (LogMessage logMessage in logService.GetLogMessages(null, null, context.objectContext.GetObject<BaseCard>(new Guid(ID))).Reverse())
		{
			string messageDescription = logService.GetLogMessageDescription(logMessage);
			Regex regex = new Regex("Выдан номер");
            // Step 2: call Match on Regex instance.
            Match match = regex.Match(messageDescription);
            // Step 3: test for Success.
            if (match.Success)
            {
                Guid? empl = findEmployee(logMessage.EmployeeName);
                if (empl.HasValue)
                {
                    xRow.Cells.Add(new XCell("RegistratorJsonData", GetEmployee(empl.Value + "")));
                    p.Rows.Add(xRow);
                    return SerializeHelper.getXmlForObject<XCards>(xCard);
                }
            }  
		}
		return "";
	}
	
	public string GetRegNumbers(string ID)
        {
            XCards xCard = new XCards();

            XSection p = new XSection();
            p.Section = "Numeric";
            xCard.Sections.Add(p);
            
            //Получение сервиса журналирования
            ILogService logService = context.objectContext.GetService<ILogService>();
            logService.AddMessageResources(LocalizationManager.GetLibraryId(),
                DocsVision.BackOffice.Localization.Resources.ResourceManager);

            string registratorJsonData = null;
            foreach (LogMessage logMessage in logService.GetLogMessages(null, null, context.objectContext.GetObject<BaseCard>(new Guid(ID))).Reverse())
            {
				string messageDescription = logService.GetLogMessageDescription(logMessage);
				Regex regex = new Regex("Выдан номер");
				// Step 2: call Match on Regex instance.
				Match match = regex.Match(messageDescription);
				// Step 3: test for Success.
				if (match.Success)
				{
					Guid? empl = findEmployee(logMessage.EmployeeName);
					if (empl.HasValue)
					{
						registratorJsonData = GetEmployee(empl.Value + "");  
					}
					break;
				}  
            }

            Document document = context.objectContext.GetObject<Document>(new Guid(ID));
            foreach (var num in document.Numbers)
            {
                XRow xRow = new XRow();
                xRow.Cells.Add(new XCell("NumberText", num.Number));
                xRow.Cells.Add(new XCell("RegistratorJsonData", registratorJsonData));

				if (num.NumericPart!=null && num.NumericPart!=Guid.Empty)
				{
					// карточка нумератора
					CardData numeratorCardData = context.userSession.CardManager.GetCardData(num.NumericPart, new Guid("D47F2C38-6553-4864-BAFF-0BC4D3A85290"));
					// строка номера
					RowData numberRow = numeratorCardData.Sections[
						new Guid("D47F2C38-6553-4864-BAFF-0BC4D3A85290")].GetRow(num.NumericPart);
					// числовое значение номера
					xRow.Cells.Add(new XCell("NumberValue", numberRow["Number"]+""));
					// Зона
					Guid? zoneRowId = numberRow.GetGuid("ParentRowID");  // id зоны, в котором строка номера
					RowData zoneRow = numeratorCardData.Sections[new Guid("916CDAB9-1FDA-4D0A-935F-6492C75477A8")].GetRow(zoneRowId.Value);
					xRow.Cells.Add(new XCell("ZoneName", zoneRow["ZoneName"] + ""));
					// атрибуты нумератора
					RowData numeratorAtrRow = numeratorCardData.Sections[new Guid("7A357C7B-7C36-48C8-8008-294B00F48AB2")].FirstRow;
					xRow.Cells.Add(new XCell("NumeratorName", numeratorAtrRow["Name"] + ""));
				}
				
                p.Rows.Add(xRow);
            }

            return SerializeHelper.getXmlForObject<XCards>(xCard);
        }
	
	private Guid? findEmployee(String empDisplayString)
	{
		CardData employeesDict = context.userSession.CardManager.GetDictionaryData(
			new Guid("{6710B92A-E148-4363-8A6F-1AA0EB18936C}")
			);
		SectionData allEmps = employeesDict.Sections[new Guid("{DBC8AE9D-C1D2-4D5E-978B-339D22B32482}")];
		RowData employee = allEmps.FindRow("@DisplayString  = '" + empDisplayString + "'");

		if (employee != null)
		{
			return employee.Id;
		}
		else
		{
			return null;
		}
	}

   
}

