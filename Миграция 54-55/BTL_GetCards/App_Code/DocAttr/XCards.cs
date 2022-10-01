using DocsVision.BackOffice.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
/// <summary>
/// Summary description for XCards
/// </summary>
[Serializable]
public class XCards
{
    public XCards()
    {
        Sections = new List<XSection>();
    }
    public List<XSection> Sections;


    internal XSection getSection(string sectionAlias)
    {
        var res = this.Sections.FirstOrDefault(s => s.Section.ToLower() == sectionAlias.ToLower());        
        return res;

    }

    public List<XFile> XFiles;
    public List<XStory> XStoryes;
}

 public class XFileVersion
    {
        public string FileName;
        public byte[] Data;
        public bool isMain;
        public string AuthorData;
        public string ChangeDate;
        public string CreateDate;
        public List<XFileComments> xFileVersionComments = new List<XFileComments>();
        public List<XFileVersion> SubVersion = new List<XFileVersion>();

        public string CurrentVersionID;
    }

    public class XFileComments
    {
        public string AuthorData;
        public string Comment;
        public string CreationDate;
    }

    public class XFile
    {
        public List<XFileVersion> XFileVersions = new List<XFileVersion>();
        public string CheckinDate;

        public string CurrentVersionID;
    }
	
	

public class XStory
{
    public string Description;
}

public class XSection
{
	
	
    public XSection()
    {
        Rows = new List<XRow>();
    }
    public string Section { get; set; }
    public List<XRow> Rows;

    public XRow FirstRow
    {
        get
        {
            if (this.Rows.Count > 0) { return this.Rows.First(); }
            return new XRow();
        }
        set { }
    }

    public XRow findRowByAlias(string alias)
    {
        foreach (var row in this.Rows) // Для свойств типа Properties
        {
            var cellValue = row.Cells.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.ShortValue+"") && c.ShortValue+"".ToLower() == alias.ToLower());
            if (cellValue != null)
            {
                return row;
            }
        }
        return null;
    }

    internal string getStringByFinderRow(string aliasToFinder, string aliasToShow)
    {
        XRow propertiesRow = this.findRowByAlias(aliasToFinder);
        if (propertiesRow != null)
        {
            var result = propertiesRow.getCell(aliasToShow);
            if (result != null)
            {
                return result.ShortValue+"";
            }
        }
        return null;
    }

    internal XCell getTypeByFinderRow(string aliasToFinder, string aliasToShow)
    {
        XRow propertiesRow = this.findRowByAlias(aliasToFinder);
        if (propertiesRow != null)
        {
            return propertiesRow.getCell(aliasToShow);
        }
        return null;
    }

    internal string getStringByFinderRow(string aliasToFinder)
    {
        return getStringByFinderRow(aliasToFinder, "DisplayValue");
    }
}


public class XCell
    {
        public XCell(string Alias, string ShortValue)
        {
            this.Alias = Alias;
            this.ShortValue = ShortValue;
        }

        public XCell()
        {
        }

        public string Alias { get; set; }
        public string DataType { get; set; }

        public object ShortValue { get; set; }
        public string CardTypeAlias { get; set; }

        public object AdditionalValue;
    }

public class refStaff
{
    public string Accaunt { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }

}


public class XRow
{
    public XRow()
    {
        Cells = new List<XCell>();
    }
    public List<XCell> Cells;

    public XCell getCell(string alias)
    {
        foreach (var cell in this.Cells)
        {
            if (alias.ToLower() == cell.Alias.ToLower())
                return cell;
        }
        return null;
    }

    public string getString(string alias)
    {
        var cell = this.Cells.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Alias) && c.Alias.ToLower() == alias.ToLower());
        if (cell != null)
        {
            return cell.ShortValue+"";
        }

        return null;
    }
}