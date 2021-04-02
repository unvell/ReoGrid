/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using unvell.ReoGrid.Graphics;

#pragma warning disable 1591

namespace unvell.ReoGrid.IO.OpenXML.Schema
{
	#region Namespaces Definitions
	internal class OpenXMLNamespaces
	{
		internal const string NET_XML______ = "http://www.w3.org/XML/1998/namespace";
		internal const string NET_XSI______ = "http://www.w3.org/2001/XMLSchema-instance";
		internal const string NET_XSD______ = "http://www.w3.org/2001/XMLSchema";

		internal const string DC___________ = "http://purl.org/dc/elements/1.1/";
		internal const string DCTerms______ = "http://purl.org/dc/terms/";
		internal const string DCMIType_____ = "http://purl.org/dc/dcmitype/";
		internal const string MC___________ = "http://schemas.openxmlformats.org/markup-compatibility/2006";

		internal const string X15__________ = "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main";
		internal const string X14AC________ = "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac";

		internal const string ContentType__ = "http://schemas.openxmlformats.org/package/2006/content-types";
		internal const string Relationships = "http://schemas.openxmlformats.org/package/2006/relationships";
		internal const string CP___________ = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";

		internal const string VTypes_______ = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";
		internal const string App__________ = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
		internal const string R____________ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

		internal const string Main_________ = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

		internal const string XDR__________ = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
		internal const string Drawing______ = "http://schemas.openxmlformats.org/drawingml/2006/main";
		internal const string Chart________ = "http://schemas.openxmlformats.org/drawingml/2006/chart";

		internal const string Drawing14____ = "http://schemas.microsoft.com/office/drawing/2010/main";
	}
	#endregion // Namespaces Definitions

	#region ContentTypes Definitions
	internal class OpenXMLContentTypes
	{
		internal const string XML___________ = "application/xml";

		internal const string Relationships_ = "application/vnd.openxmlformats-package.relationships+xml";
		internal const string CoreProperties = "application/vnd.openxmlformats-package.core-properties+xml";

		internal const string Theme_________ = "application/vnd.openxmlformats-officedocument.theme+xml";
		internal const string Workbook______ = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
		internal const string Worksheet_____ = "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";
		internal const string Styles________ = "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";
		internal const string SharedStrings_ = "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";
		internal const string App___________ = "application/vnd.openxmlformats-officedocument.extended-properties+xml";
		internal const string Drawing_______ = "application/vnd.openxmlformats-officedocument.drawing+xml";
	}
	#endregion // ContentTypes Definitions

	#region RelationTypes Definitions
	internal class OpenXMLRelationTypes
	{
		internal const string docProps_core____ = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";

		internal const string xl_workbook______ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
		internal const string docProps_app_____ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";
		internal const string docProps_custom__ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";
		internal const string worksheets_sheet_ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
		internal const string shared_strings___ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";
		internal const string theme____________ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";
		internal const string styles___________ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
		internal const string drawing__________ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";
		internal const string image____________ = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
	}
	#endregion // RelationTypes Definitions

	#region docProps/core.xml
	[XmlRoot("coreProperties", Namespace = OpenXMLNamespaces.CP___________)]
	public class CoreProperties : OpenXMLFile
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]{
				new System.Xml.XmlQualifiedName("cp", OpenXMLNamespaces.CP___________),
				new System.Xml.XmlQualifiedName("dc", OpenXMLNamespaces.DC___________),
				new System.Xml.XmlQualifiedName("dcterms", OpenXMLNamespaces.DCTerms______),
				new System.Xml.XmlQualifiedName("dcmitype", OpenXMLNamespaces.DCMIType_____),
				new System.Xml.XmlQualifiedName("xsi", OpenXMLNamespaces.NET_XSI______),
			});

		[XmlElement(Namespace = OpenXMLNamespaces.DC___________)]
		public InnerTextElement creator;

		[XmlElement(Namespace = OpenXMLNamespaces.CP___________)]
		public InnerTextElement lastModifiedBy;

		[XmlElement(Namespace = OpenXMLNamespaces.DCTerms______)]
		public OpenXMLDateTime created;

		[XmlElement(Namespace = OpenXMLNamespaces.DCTerms______)]
		public OpenXMLDateTime modified;
	}

	[XmlRoot("Types", Namespace = OpenXMLNamespaces.ContentType__)]
	public class ContentType
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]{
				new System.Xml.XmlQualifiedName(string.Empty, OpenXMLNamespaces.ContentType__),
			});

		[XmlElement("Default")]
		public List<ContentTypeDefaultItem> Defaults;

		[XmlElement("Override")]
		public List<ContentTypeOverrideItem> Overrides;

		[XmlIgnore]
		internal static readonly string _xmlTarget = "[Content_Types].xml";
	}

	public class ContentTypeDefaultItem
	{
		[XmlAttribute("Extension")]
		public string Extension;

		[XmlAttribute("ContentType")]
		public string ContentType;
	}

	public class ContentTypeOverrideItem
	{
		[XmlAttribute("PartName")]
		public string PartName;

		[XmlAttribute("ContentType")]
		public string ContentType;
	}
	#endregion // docProps/core.xml

	#region docProps/app.xml
	[XmlRoot("Properties", Namespace = OpenXMLNamespaces.App__________)]
	public class AppProperties : OpenXMLFile
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]{
				new System.Xml.XmlQualifiedName("vt", OpenXMLNamespaces.VTypes_______),
			});

		[XmlElement("Application")]
		public string Application = "ReoGrid - .NET Spreadsheet Component";

		[XmlElement("DocSecurity")]
		public string DocSecurity = "0";

		[XmlElement("ScaleCrop")]
		public string ScaleCrop = "false";

		[XmlElement("HeadingPairs")]
		public HeadingPairs headingPairs;

		[XmlElement("TitlesOfParts")]
		public TitlesOfParts titlesOfParts;

		[XmlElement("Company")]
		public string Company = null;

		[XmlElement("LinksUpToDate")]
		public string LinksUpToDate = "false";

		[XmlElement("SharedDoc")]
		public string SharedDoc = "false";

		[XmlElement("HyperlinksChanged")]
		public string HyperlinksChanged = "false";

		[XmlElement("AppVersion")]
		public string AppVersion = "0.9300"; // todo: get from assembly version

		[XmlElement("AppHomepage")]
		public string AppHomepage = null;// = "https://reogrid.net";
	}

	public class HeadingPairs
	{
		[XmlElement("vector", Namespace = OpenXMLNamespaces.VTypes_______)]
		public Vector vector;
	}

	public class TitlesOfParts
	{
		[XmlElement("vector", Namespace = OpenXMLNamespaces.VTypes_______)]
		public Vector vector;
	}

	public class Vector : Variant
	{
		[XmlAttribute("size")]
		public string size;

		[XmlAttribute("baseType")]
		public string baseType;

		[XmlElement("variant")]
		public List<Variant> variants;
	}

	public class Variant
	{
		[XmlElement("lpstr")]
		public InnerTextElement lpstr;

		[XmlElement("i4")]
		public InnerTextElement i4;
	}
	#endregion // docProps/app.xml

	#region xl/workbook.xml
	[XmlRoot("workbook", Namespace = OpenXMLNamespaces.Main_________)]
	public partial class Workbook : OpenXMLFile
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName("r", OpenXMLNamespaces.R____________),
				//new System.Xml.XmlQualifiedName("mc", OpenXMLNamespaces.MC),
				//new System.Xml.XmlQualifiedName("x15", OpenXMLNamespaces.X15),
			});

		//[XmlAttribute("Ignorable", Namespace=OpenXMLNamespaces.MC)]
		//public string Ignorable = "x15";

		[XmlElement("fileVersion")]
		public FileVersion fileVersion;

		[XmlElement("workbookPr")]
		public WorkbookProperties workbookPr;

		[XmlElement("bookViews")]
		public BookViews bookViews;

		[XmlElement("calcPr")]
		public CalcProperties calcPr;

		[XmlArray("sheets"), XmlArrayItem("sheet")]
		public List<WorkbookSheet> sheets;

		[XmlArray("definedNames"), XmlArrayItem("definedName")]
		public List<DefinedName> definedNames;

	}

	// <workbookPr backupFile="false" showObjects="all" date1904="false"/>
	public class WorkbookProperties
	{
		[XmlAttribute("filterPrivacy")]
		public string filterPrivacy;

		[XmlAttribute("defaultThemeVersion")]
		public string defaultThemeVersion;

		[XmlAttribute("backupFile")]
		public string backupFile;

		[XmlAttribute("showObjects")]
		public string showObjects;

		[XmlAttribute("date1904")]
		public string date1904;
	}

	// <fileVersion appName="xl" lastEdited="6" lowestEdited="6" rupBuild="14420"/>
	public class FileVersion
	{
		[XmlAttribute("appName")]
		public string appName = "ReoGrid";

		[XmlAttribute("lastEdited")]
		public string lastEdited;

		[XmlAttribute("lowestEdited")]
		public string lowestEdited;

		[XmlAttribute("rupBuild")]
		public string rupBuild;
	}

	public class BookViews
	{
		[XmlElement("workbookView")]
		public WorkbookView workbookView;
	}

	// <workbookView showHorizontalScroll="true" showVerticalScroll="true" showSheetTabs="true" xWindow="0" yWindow="0" windowWidth="16384" windowHeight="8192" tabRatio="983" firstSheet="0" activeTab="0"/>
	public class WorkbookView
	{
		[XmlAttribute("showHorizontalScroll")]
		public string showHorizontalScroll;

		[XmlAttribute("showVerticalScroll")]
		public string showVerticalScroll;

		[XmlAttribute("showSheetTabs")]
		public string showSheetTabs;

		[XmlAttribute("xWindow")]
		public string xWindow;

		[XmlAttribute("yWindow")]
		public string yWindow;

		[XmlAttribute("windowWidth")]
		public string windowWidth;

		[XmlAttribute("windowHeight")]
		public string windowHeight;

		[XmlAttribute("tabRatio")]
		public string tabRatio;

		[XmlAttribute("firstSheet")]
		public string firstSheet;

		[XmlAttribute("activeTab")]
		public string activeTab;
	}

	// <calcPr iterateCount="100" refMode="A1" iterate="false" iterateDelta="0.001"/>
	public class CalcProperties
	{
		[XmlAttribute("calcId")]
		public string calcId;

		[XmlAttribute("iterateCount")]
		public string iterateCount;

		[XmlAttribute("refMode")]
		public string refMode;

		[XmlAttribute("iterate")]
		public string iterate;

		[XmlAttribute("iterateDelta")]
		public string iterateDelta;
	}

	// <sheet name="Sheet1" sheetId="1" r:id="rId1"/>
	public class WorkbookSheet
	{
		[XmlAttribute("name")]
		public string name;
		[XmlAttribute("sheetId")]
		public string sheetId;
		[XmlAttribute("id", Namespace = OpenXMLNamespaces.R____________)]
		public string resId;

		[XmlIgnore]
		internal Worksheet _instance;
	}

	public class DefinedName
	{
		[XmlAttribute]
		public string name;
		[XmlText]
		public string address;
		[XmlAttribute]
		public string localSheetId;
	}
	#endregion // xl/workbook.xml

	#region xl/worksheets/sheet.xml
	[XmlRoot("worksheet", Namespace = OpenXMLNamespaces.Main_________)]
	public class Worksheet : OpenXMLFile
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName("r", OpenXMLNamespaces.R____________),
				//new System.Xml.XmlQualifiedName("mc", OpenXMLNamespaces.MC___________),
				//new System.Xml.XmlQualifiedName("x14ac", OpenXMLNamespaces.X14AC________),
			});

		//[XmlAttribute("Ignorable", Namespace=OpenXMLNamespaces.MC___________)]
		//public string Ignorable = "x14ac";

		[XmlElement("dimension")]
		public Dimension dimension;

		[XmlArray("sheetViews"), XmlArrayItem("sheetView")]
		public List<SheetView> sheetViews;

		[XmlElement("sheetFormatPr")]
		public SheetFormatProperty sheetFormatProperty;

		[XmlArray("cols"), XmlArrayItem("col")]
		public List<Column> cols;

		[XmlArray("sheetData"), XmlArrayItem("row")]
		public List<Row> rows;

		[XmlArray("mergeCells"), XmlArrayItem("mergeCell")]
		public List<MergeCell> mergeCells;

		[XmlElement("drawing")]
		public SheetDrawing drawing;

		[XmlElement("pageMargins")]
		public PageMargins pageMargins;

		[XmlElement("pageSetup")]
		public PageSetup pageSetup;

		[XmlElement("rowBreaks")]
		public RowBreaks rowBreaks;

		[XmlElement("colBreaks")]
		public ColumnBreaks colBreaks;
	}

	public class SheetProperty
	{
		[XmlElement("pageSetUpPr")]
		public PageSetUpProperty pageSetUpPr;
	}

	// <pageSetUpPr autoPageBreaks="0" fitToPage="1"/>
	public class PageSetUpProperty
	{
		public string autoPageBreaks;
		public string fitToPage;
	}

	//<rowBreaks count="2" manualBreakCount="2">
	//	<brk id="4" max="16383" man="1"/>
	//	<brk id="11" max="16383" man="1"/>
	//</rowBreaks>
	public class RowBreaks
	{
		[XmlElement("brk")]
		public List<PageBreak> breaks;
	}

	//<colBreaks count="1" manualBreakCount="1">
	//</colBreaks>
	public class ColumnBreaks
	{
		[XmlElement("brk")]
		public List<PageBreak> breaks;
	}

	//<brk id = "4" max="1048575" man="1"/>
	public class PageBreak
	{
		[XmlAttribute("id")]
		public int id;
		[XmlAttribute("max")]
		public int max;
		[XmlAttribute("man")]
		public int man;
	}

	// <dimension ref="B3:J43"/>
	public class Dimension
	{
		[XmlAttribute("ref")]
		public string address;
	}

	// <sheetView showGridLines="0" showRowColHeaders="0" tabSelected="1" zoomScaleNormal="100" workbookViewId="0">
	public class SheetView
	{
		[XmlAttribute("showGridLines")]
		public string showGridLines;
		[XmlAttribute("showRowColHeaders")]
		public string showRowColHeaders;
		[XmlAttribute("tabSelected")]
		public string tabSelected;

		[XmlAttribute("zoomScale")]
		public string zoomScale;
		[XmlAttribute("zoomScaleNormal")]
		public string zoomScaleNormal;

		[XmlAttribute("workbookViewId")]
		public string workbookViewId;

		[XmlAttribute("view")]
		public string view;

		[XmlElement("pane")]
		public Pane pane;

		[XmlElement("selection")]
		public Selection selection;
	}

	// <selection activeCell="C11" sqref="C11"/>
	public class Selection
	{
		[XmlAttribute("activeCell")]
		public string activeCell;
		[XmlAttribute("sqref")]
		public string sqref;
	}

	// <pane xSplit="1" ySplit="3" topLeftCell="B4" activePane="bottomRight" state="frozen"/>
	public class Pane
	{
		[XmlAttribute("xSplit")]
		public string xSplit;
		[XmlAttribute("ySplit")]
		public string ySplit;
		[XmlAttribute("topLeftCell")]
		public string topLeftCell;
		[XmlAttribute("activePane")]
		public string activePane;
		[XmlAttribute("state")]
		public string state;
	}

	// <sheetFormatPr defaultRowHeight="12.75"/>
	public class SheetFormatProperty
	{
		[XmlAttribute("baseColWidth")]
		public string baseColWidth;
		[XmlAttribute("defaultRowHeight")]
		public string defaultRowHeight;
		[XmlAttribute("defaultColWidth")]
		public string defaultColumnWidth;

		[XmlAttribute("customHeight")]
		public string customHeight;

		//[XmlAttribute("dyDescent", Namespace = OpenXMLNamespaces.X14AC________)]
		//public string dyDescent;
	}

	// <col min="1" max="1" width="1.7109375" style="3" customWidth="1"/>
	public class Column
	{
		[XmlAttribute("min")]
		public int min;
		[XmlAttribute("max")]
		public int max;
		[XmlAttribute("width")]
		public double width;
		[XmlAttribute("style")]
		public string style;
		[XmlAttribute("customWidth")]
		public string customWidth;
		[XmlAttribute("customFormat")]
		public string customFormat;
	}

	// <row r="3" spans="2:10" ht="33.75">
	public class Row
	{
		[XmlAttribute("r")]
		public int index;
		[XmlAttribute("spans")]
		public string spans;
		[XmlAttribute("hidden")]
		public string hidden;
		[XmlAttribute("s")]
		public string styleIndex;
		[XmlAttribute("ht")]
		public string height;
		[XmlAttribute("customHeight")]
		public string customHeight;
		//[XmlAttribute("dyDescent", Namespace = OpenXMLNamespaces.X14AC________)]
		//public string dyDescent;
		[XmlAttribute("customFormat")]
		public string customFormat;

		[XmlElement("c")]
		public List<Cell> cells;
	}

	// <c r="B3" s="1" t="s">
	public class Cell
	{
		[XmlAttribute("r")]
		public string address;
		[XmlAttribute("s")]
		public string styleIndex;
		[XmlAttribute("t")]
		public string dataType;
		[XmlElement("f")]
		public Formula formula;
		[XmlElement("v")]
		public ElementText value;
		//[XmlAttribute("spans")]
		//public string spans;
	}

	public class Formula
	{
		[XmlAttribute("t")]
		public string type;

		[XmlAttribute("ref")]
		public string @ref;

		[XmlAttribute("si")]
		public string sharedIndex;

		[XmlText]
		public string val;
	}

	// <mergeCell ref="B43:J43"/>
	public class MergeCell
	{
		[XmlAttribute("ref")]
		public string address;
	}

	// <pageMargins left="0.23622047244094491" right="0.23622047244094491" top="0.74803149606299213"
	// bottom="0.74803149606299213" header="0.23622047244094491" footer="0.51181102362204722"/>
	public class PageMargins
	{
		[XmlAttribute("left")]
		public float left;
		[XmlAttribute("right")]
		public float right;
		[XmlAttribute("top")]
		public float top;
		[XmlAttribute("bottom")]
		public float bottom;
		[XmlAttribute("header")]
		public float header;
		[XmlAttribute("footer")]
		public float footer;
	}

	// <pageSetup orientation="portrait" horizontalDpi="4294967294" verticalDpi="300" r:id="rId1"/>
	public class PageSetup
	{
		[XmlAttribute("orientation")]
		public string orientation;

		public string horizontalDpi;
		public string verticalDpi;

		[XmlAttribute("id", Namespace = OpenXMLNamespaces.R____________,
			Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string id;
	}

	// <drawing r:id="rId1"/>
	public class SheetDrawing
	{
		[XmlAttribute("id", Namespace = OpenXMLNamespaces.R____________,
			Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string id;

#if DRAWING
		[XmlIgnore]
		internal Drawing _instance;
#endif // DRAWING

	}
	#endregion // xl/worksheets/sheet.xml

	#region xl/styles.xml
	[XmlRoot("styleSheet", Namespace = OpenXMLNamespaces.Main_________)]
	public class Stylesheet
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName(string.Empty, OpenXMLNamespaces.Main_________),
			});

		[XmlElement("numFmts")]
		public NumberFormatCollection numberFormats;

		[XmlElement("fonts")]
		public FontCollection fonts;

		[XmlElement("fills")]
		public FillCollection fills;

		[XmlElement("borders")]
		public BorderCollection borders;

		[XmlElement("cellStyleXfs")]
		public CellFormatCollection cellStyleFormats;

		[XmlElement("cellXfs")]
		public CellFormatCollection cellFormats;

		[XmlElement("cellStyles")]
		public CellStyleCollection cellStyles;

		[XmlElement("colors")]
		public IndexedColors colors;

		[XmlIgnore]
		internal string _xmlTarget;
	}

	public class NumberFormatCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("numFmt")]
		public List<NumberFormat> list = new List<NumberFormat>();

		public void Add(NumberFormat item) { this.list.Add(item); }
		public int FindIndex(Predicate<NumberFormat> match) { return this.list.FindIndex(match); }
	}

	public class FontCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("font")]
		public List<Font> list = new List<Font>();

		public void Add(Font item) { this.list.Add(item); }
		public int FindIndex(Predicate<Font> match) { return this.list.FindIndex(match); }
	}

	public class FillCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("fill")]
		public List<Fill> list = new List<Fill>();

		public void Add(Fill item) { this.list.Add(item); }
		public int FindIndex(Predicate<Fill> match) { return this.list.FindIndex(match); }
	}

	public class BorderCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("border")]
		public List<Border> list = new List<Border>();

		public void Add(Border item) { this.list.Add(item); }
		public Border this[int index] { get { return this.list[index]; } set { this.list[index] = value; } }
		public int FindIndex(Predicate<Border> match) { return this.list.FindIndex(match); }
	}

	public class CellFormatCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("xf")]
		public List<CellFormat> list = new List<CellFormat>();

		public void Add(CellFormat item) { this.list.Add(item); }
		public int FindIndex(Predicate<CellFormat> match) { return this.list.FindIndex(match); }
	}

	public class CellStyleCollection
	{
		[XmlAttribute("count")]
		public int Count { get { return this.list.Count; } set { } }
		[XmlElement("cellStyle")]
		public List<CellStyle> list = new List<CellStyle>();

		public void Add(CellStyle item) { this.list.Add(item); }
		public int FindIndex(Predicate<CellStyle> match) { return this.list.FindIndex(match); }
	}

	// <numFmt numFmtId="44" formatCode="_(&quot;$&quot;* #,##0.00_);_(&quot;$&quot;* \(#,##0.00\);_(&quot;$&quot;* &quot;-&quot;??_);_(@_)"/>
	public class NumberFormat
	{
		[XmlAttribute("numFmtId")]
		public int formatId;
		[XmlAttribute("formatCode")]
		public string formatCode;

		[XmlIgnore]
		internal object _iarg;
	}

	public class Font
	{
		[XmlElement("sz")]
		public ElementValue<string> size;
		[XmlElement("b")]
		public Bold bold;
		[XmlElement("i")]
		public Italic italic;
		[XmlElement("strike")]
		public Strikethrough strikethrough;
		[XmlElement("u")]
		public Underline underline;
		[XmlElement("color")]
		public ColorValue color;
		[XmlElement("name")]
		public ElementValue<string> name;
		[XmlElement("family")]
		public ElementValue<string> family;

		public Font() { }

		internal Font(WorksheetRangeStyle rgStyle)
		{
			this._size = rgStyle.FontSize;

			this.name = new ElementValue<string>(rgStyle.FontName);
			this.size = new ElementValue<string>(rgStyle.FontSize.ToString(ExcelWriter.EnglishCulture));

			this.bold = ((rgStyle.Flag & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold && rgStyle.Bold)
				? new Bold() : null;

			this.italic = ((rgStyle.Flag & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic && rgStyle.Italic)
				? new Italic() : null;

			this.strikethrough = ((rgStyle.Flag & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough && rgStyle.Strikethrough)
				? new Strikethrough() : null;

			if ((rgStyle.Flag & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor && rgStyle.TextColor.A > 0)
			{
				this.color = new ColorValue(rgStyle.TextColor) { _rgColor = rgStyle.TextColor };
			}
		}

		[XmlIgnore]
		internal float _size;
	}

	public class Bold { }
	public class Italic { }
	public class Strikethrough { }
	public class Underline { }

	// <fill>
	//   <patternFill patternType="solid">
	//   <fgColor indexed="44"/>
	//   </patternFill>
	// </fill>
	public class Fill
	{
		[XmlElement("patternFill")]
		public PatternFill patternFill;
	}

	public class PatternFill
	{
		[XmlAttribute("patternType")]
		public string patternType;
		[XmlElement("fgColor")]
		public ColorValue foregroundColor;
		[XmlElement("bgColor")]
		public ColorValue backgroundColor;
	}

	public class Border
	{
		[XmlElement("left")]
		public SideBorder left;
		[XmlElement("right")]
		public SideBorder right;
		[XmlElement("top")]
		public SideBorder top;
		[XmlElement("bottom")]
		public SideBorder bottom;
		[XmlElement("diagonal")]
		public SideBorder diagonal;

		[XmlIgnore]
		internal RangeBorderStyle _left;
		[XmlIgnore]
		internal RangeBorderStyle _right;
		[XmlIgnore]
		internal RangeBorderStyle _top;
		[XmlIgnore]
		internal RangeBorderStyle _bottom;

		[XmlIgnore]
		internal bool _preprocessed = false;
		[XmlIgnore]
		internal bool _hasLeft;
		[XmlIgnore]
		internal bool _hasRight;
		[XmlIgnore]
		internal bool _hasTop;
		[XmlIgnore]
		internal bool _hasBottom;
	}

	public class SideBorder
	{
		[XmlAttribute("style")]
		public string style;

		[XmlElement("color")]
		public ColorValue color;
	}

	// <xf numFmtId="0" fontId="25" fillId="2" borderId="0" applyNumberFormat="0" applyBorder="0" applyAlignment="0" applyProtection="0"/>
	public class CellFormat
	{
		[XmlAttribute("numFmtId")]
		public string numberFormatId;
		[XmlAttribute("fontId")]
		public string fontId;
		[XmlAttribute("fillId")]
		public string fillId;
		[XmlAttribute("borderId")]
		public string borderId;
		[XmlAttribute("xfId")]
		public string xfId;

		[XmlAttribute("applyNumberFormat")]
		public string applyNumberFormat;
		[XmlAttribute("applyFill")]
		public string applyFill;
		[XmlAttribute("applyFont")]
		public string applyFont;
		[XmlAttribute("applyBorder")]
		public string applyBorder;
		[XmlAttribute("applyAlignment")]
		public string applyAlignment;
		[XmlAttribute("applyProtection")]
		public string applyProtection;

		[XmlElement("alignment")]
		public Alignment alignment;
		[XmlElement("protection")]
		public Protection protection;

		[XmlIgnore]
		internal bool _preprocessed = false;
		[XmlIgnore]
		internal WorksheetRangeStyle _cachedStyleSet;
		//[XmlIgnore]
		//internal Font _cachedFont;
	}

	// <cellStyle name="20% - Accent1" xfId="1" builtinId="30" customBuiltin="1"/>
	public class CellStyle
	{
		[XmlAttribute("name")]
		public string name;
		[XmlAttribute("xfId")]
		public string xfId;
		[XmlAttribute("builtinId")]
		public string builtinId;
		[XmlAttribute("customBuiltin")]
		public string customBuiltin;
	}

	public class IndexedColors
	{
		[XmlArray("indexedColors"), XmlArrayItem("rgbColor")]
		public List<ColorValue> indexedColors;
	}

	public class ColorValue
	{
		[XmlAttribute("indexed")]
		public string indexed;
		[XmlAttribute("rgb")]
		public string rgb;
		[XmlAttribute("auto")]
		public string auto;
		[XmlAttribute("theme")]
		public string theme;
		[XmlAttribute("tint")]
		public string tint;

		public ColorValue() { }

		[XmlIgnore]
		internal SolidColor? _rgColor;

		internal ColorValue(SolidColor c)
		{
			int index = c.A == 255 ? Array.IndexOf(IndexedColorTable.colors, (c.R << 16) | (c.G << 8) | (c.B)) : -1;

			if (index >= 0)
			{
				this.indexed = index.ToString();
			}
			else
			{
				this.rgb = Convert.ToString(c.ToArgb(), 16);
			}
		}
	}

	public class Alignment
	{
		[XmlAttribute("horizontal")]
		public string horizontal;
		[XmlAttribute("vertical")]
		public string vertical;
		[XmlAttribute("indent")]
		public string indent;
		[XmlAttribute("textRotation")]
		public string textRotation;
		[XmlAttribute("wrapText")]
		public string wrapText;

		[XmlIgnore]
		internal ReoGridHorAlign _horAlign;
		[XmlIgnore]
		internal ReoGridVerAlign _verAlign;
		[XmlIgnore]
		internal int _rotateAngle;
	}

	public class Protection
	{
		[XmlAttribute("locked")]
		public string locked;
	}

	public class ElementValue<T>
	{
		[XmlAttribute("val")]
		public T value;

		public ElementValue() { }

		internal ElementValue(T val)
		{
			this.value = val;
		}

		public static implicit operator T(ElementValue<T> v)
		{
			return v == null ? default(T) : v.value;
		}

		public static implicit operator ElementValue<T>(T v)
		{
			return new ElementValue<T>(v);
		}
	}

	public class ElementText
	{
		[XmlText]
		public string val;

		[XmlAttribute("space", Namespace = OpenXMLNamespaces.NET_XML______)]
		public string space;

		public ElementText() { }

		internal ElementText(string val)
		{
			this.val = val;
			if (val.Length > 0 && (Char.IsWhiteSpace(val[0]) || Char.IsWhiteSpace(val[val.Length - 1])))
			{
				this.space = "preserve";
			}
		}

		public static implicit operator string(ElementText t)
		{
			return t == null ? null : t.val;
		}

		public static implicit operator ElementText(string t)
		{
			return new ElementText(t);
		}
	}
	#endregion // xl/styles.xml

	#region SharedStringTable
	[XmlRoot("sst", Namespace = OpenXMLNamespaces.Main_________)]
	public class SharedStrings
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName(string.Empty, OpenXMLNamespaces.Main_________),
			});

		[XmlAttribute("count")]
		public string count;

		[XmlAttribute("uniqueCount")]
		public string uniqueCount;

		[XmlElement("si")]
		public List<SharedStringItem> items;

		[XmlIgnore]
		internal string _xmlTarget;
	}

	public class SharedStringItem
	{
		/// <summary>
		/// tag: t
		/// </summary>
		[XmlElement("t")]
		public ElementText text;

		/// <summary>
		/// tag: r
		/// </summary>
		[XmlElement("r")]
		public List<Run> runs;

#if DRAWING
		[XmlIgnore]
		internal ReoGrid.Drawing.RichText _rt;
#endif // DRAWING
	}

#endregion // SharedStringTable

#region Drawing
#if DRAWING

	[XmlRoot("wsDr", Namespace = OpenXMLNamespaces.XDR__________)]
	public class Drawing : OpenXMLFile
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName("xdr", OpenXMLNamespaces.XDR__________),
				new System.Xml.XmlQualifiedName("a", OpenXMLNamespaces.Drawing______),
			});

		[XmlElement("twoCellAnchor")]
		public List<TwoCellAnchor> twoCellAnchors;

		[XmlIgnore]
		internal int _drawingObjectCount = 2;

		[XmlIgnore]
		internal Dictionary<string, int> _typeObjectCount;

		[XmlIgnore]
		internal List<Blip> _images;
	}

	public class TwoCellAnchor
	{
		[XmlElement("from")]
		public CellAnchor from;

		[XmlElement("to")]
		public CellAnchor to;

		[XmlElement("pic")]
		public Pic pic;

		[XmlElement("sp")]
		public Shape shape;

		[XmlElement("cxnSp")]
		public Shape cxnShape;

		[XmlElement("graphicFrame")]
		public GraphicFrame graphcFrame;

		[XmlElement("clientData")]
		public ClientData clientData;

		[XmlAttribute("editAs")]
		public string editAs;
	}

	public class CellAnchor
	{
		[XmlElement("col")]
		public int col;
		[XmlElement("colOff")]
		public int colOff;
		[XmlElement("row")]
		public int row;
		[XmlElement("rowOff")]
		public int rowOff;
	}

	public class Pic
	{
		[XmlElement("nvPicPr")]
		public NvPicProperty nvPicPr;

		[XmlElement("blipFill")]
		public BlipFill blipFill;

		[XmlElement("spPr")]
		public ShapeProperty prop;
	}

	public class NvPicProperty
	{
		[XmlElement("cNvPr")]
		public NonVisualProp cNvPr;

		[XmlElement("cNvPicPr")]
		public CNvPicProperty cNvPicPr;
	}

	public class CNvPicProperty
	{
		[XmlElement("picLocks", Namespace = OpenXMLNamespaces.Drawing______)]
		public PicLocks picLocks;
	}

	public class PicLocks
	{
		[XmlAttribute("noChangeAspect")]
		public string noChangeAspect = "1";
	}

	public class BlipFill
	{
		[XmlElement("blip", Namespace = OpenXMLNamespaces.Drawing______)]
		public Blip blip;

		[XmlElement("stretch", Namespace = OpenXMLNamespaces.Drawing______)]
		public Stretch stretch;
	}

	public class Blip
	{
		[XmlAttribute("embed", Namespace = OpenXMLNamespaces.R____________)]
		public string embedId;

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[] {
				new System.Xml.XmlQualifiedName("r", OpenXMLNamespaces.R____________),
			});

		[XmlIgnore]
		internal ReoGrid.Drawing.ImageObject _imageObject;

		[XmlIgnore]
		internal string _imageFileName;
	}

	public class Stretch
	{
		[XmlElement("fillRect")]
		public FillRect fillRect;
	}

	public class FillRect
	{

	}

	// <xdr:sp macro="" textlink="">
	public class Shape
	{
		[XmlAttribute("macro")]
		public string macro;
		[XmlAttribute("textlink")]
		public string textlink;

		[XmlElement("nvSpPr")]
		public NonVisualShapeProp nvSpPr;

		[XmlElement("spPr")]
		public ShapeProperty prop;

		[XmlElement("style")]
		public ShapeStyle style;

		[XmlElement("txBody")]
		public TextBody textBody;
	}

	public class NonVisualProp
	{
		[XmlAttribute("id")]
		public int id;
		[XmlAttribute("name")]
		public string name;
	}

	// <xdr:nvSpPr>
	public class NonVisualShapeProp
	{
		[XmlElement("cNvPr")]
		public NonVisualProp nvPr;
	}

	public class ShapeProperty
	{
		[XmlElement("xfrm", Namespace = OpenXMLNamespaces.Drawing______)]
		public Transform transform;

		[XmlElement("prstGeom", Namespace = OpenXMLNamespaces.Drawing______)]
		public PresetGeometry prstGeom;

		[XmlElement("solidFill", Namespace = OpenXMLNamespaces.Drawing______)]
		public CompColor solidFill;

		[XmlElement("noFill", Namespace = OpenXMLNamespaces.Drawing______)]
		public NoFill noFill;

		[XmlElement("ln", Namespace = OpenXMLNamespaces.Drawing______)]
		public Outline line;
	}

	public class Transform
	{
		[XmlAttribute("flipH")]
		public string flipH;
		[XmlAttribute("flipV")]
		public string flipV;
		[XmlAttribute("rot")]
		public string rotation;

		[XmlElement("off")]
		public Offset offset;

		[XmlElement("ext")]
		public Extents extents;
	}

	public class Offset
	{
		[XmlAttribute("x")]
		public int x;

		[XmlAttribute("y")]
		public int y;
	}

	public class Extents
	{
		[XmlAttribute("cx")]
		public int cx;

		[XmlAttribute("cy")]
		public int cy;
	}

	public class NoFill
	{
	}

	// <a:prstGeom prst="rect">
	public class PresetGeometry
	{
		[XmlAttribute("prst")]
		public string presetType;

		[XmlArray("avLst"), XmlArrayItem("gd")]
		public List<ShapeGuide> avList;
	}

	public class ShapeGuide
	{
		[XmlAttribute("name")]
		public string name;

		[XmlAttribute("fmla")]
		public string formula;
	}

	// <a:ln w="28575">
	public class Outline
	{
		[XmlAttribute("w")]
		public string weight;

		[XmlElement("headEnd")]
		public HeadEnd headEnd;

		[XmlElement("tailEnd")]
		public HeadEnd tailEnd;

		[XmlElement("solidFill")]
		public CompColor solidFill;

		[XmlElement("prstDash")]
		public ElementValue<string> prstDash;

		[XmlElement("noFill")]
		public NoFill noFill;
	}

	public class HeadEnd
	{
		[XmlAttribute("type")]
		public string type;
	}

	public class ShapeStyle
	{
		[XmlElement("lnRef", Namespace = OpenXMLNamespaces.Drawing______)]
		public LineReference lnRef;

		[XmlElement("fillRef", Namespace = OpenXMLNamespaces.Drawing______)]
		public FillReference fillRef;

		[XmlElement("effectRef", Namespace = OpenXMLNamespaces.Drawing______)]
		public EffectReference effectRef;

		[XmlElement("fontRef", Namespace = OpenXMLNamespaces.Drawing______)]
		public FontReference fontRef;
	}

	// <a:lnRef idx="2">
	public class LineReference : CompColor
	{
		[XmlAttribute("idx")]
		public string idx;
	}

	public class FillReference : CompColor
	{
		[XmlAttribute("idx")]
		public string idx;
	}

	public class EffectReference : CompColor
	{
		[XmlAttribute("idx")]
		public string idx;
	}
#endif // DRAWING

	public class FontReference : CompColor
	{
		[XmlAttribute("idx")]
		public string idx;

		//[XmlElement("schemeClr")]
		//public CompColorVar schemeClr;
	}

	public class TextBody
	{
		[XmlElement("bodyPr")]
		public BodyProperty bodyProperty;

		[XmlElement("p", Namespace = OpenXMLNamespaces.Drawing______)]
		public List<Paragraph> paragraphs;
	}

	// <a:bodyPr vertOverflow="clip" horzOverflow="clip" rtlCol="0" anchor="t"/>
	public class BodyProperty
	{
		[XmlAttribute("vertOverflow")]
		public string vertOverflow;

		[XmlAttribute("horzOverflow")]
		public string horzOverflow;

		[XmlAttribute("rtlCol")]
		public int rtlCol;

		[XmlAttribute("anchor")]
		public string anchor;
	}

	public class Paragraph
	{
		[XmlElement("r")]
		public List<Run> runs;

		[XmlElement("pPr")]
		public ParagraphProp property;
	}

	public class ParagraphProp
	{
		[XmlAttribute("algn")]
		public string align;
	}

	public class Run
	{
		[XmlElement("rPr")]
		public RunProperty property;

		[XmlElement("t")]
		public Text text;
	}

	public class Text
	{
		[XmlAttribute("space", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string space;

		[XmlText]
		public string innerText;
	}

	public class RunProperty
	{
		//<vertAlign val="superscript"/>
		//<sz val="11"/>
		//<color rgb = "FFFF0000" />
		//< rFont val="Calibri"/>
		//<family val = "2" />
		//< scheme val="minor"/>
		[XmlElement("vertAlign")]
		public ElementValue<string> vertAlign;

		[XmlElement("sz")]
		public ElementValue<string> size;

		[XmlElement("strike")]
		public ElementValue<string> strike;

		[XmlElement("b")]
		public ElementValue<string> b;

		[XmlElement("i")]
		public ElementValue<string> i;

		[XmlElement("u")]
		public ElementValue<string> u;

		[XmlElement("color")]
		public ColorValue color;

		[XmlElement("rFont")]
		public ElementValue<string> font;

		[XmlElement("family")]
		public ElementValue<string> family;

		[XmlElement("schema")]
		public ElementValue<string> schema;

		//////////////////////////////////////////

		[XmlAttribute("sz")]
		public string sizeAttr;

		[XmlElement("solidFill")]
		public CompColor solidFill;

	}

	public class GraphicFrame
	{
		[XmlElement("graphic", Namespace = OpenXMLNamespaces.Drawing______)]
		public Graphic graphic;
	}

	public class Graphic
	{
		[XmlElement("graphicData")]
		public GraphicData data;
	}

	public class GraphicData
	{
		[XmlAttribute("uri")]
		public string uri;

		[XmlElement("chart", Namespace = OpenXMLNamespaces.Chart________)]
		public GraphicChartRelation chart;
	}

	public class GraphicChartRelation
	{
		[XmlAttribute("id", Namespace = OpenXMLNamespaces.R____________)]
		public string id;
	}

	public class ClientData
	{
	}

#endregion // Drawing

#region Chart
	[XmlRoot("chartSpace", Namespace = OpenXMLNamespaces.Chart________)]
	public class ChartSpace
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[]
			{
				new System.Xml.XmlQualifiedName("c", OpenXMLNamespaces.Chart________),
				new System.Xml.XmlQualifiedName("a", OpenXMLNamespaces.Drawing______),
				new System.Xml.XmlQualifiedName("r", OpenXMLNamespaces.R____________),
			});

		[XmlElement("chart")]
		public Chart chart;
	}

	public class Chart
	{
		[XmlElement("plotArea")]
		public PlotArea plotArea;

		[XmlElement("legend")]
		public Legend legend;
	}

	public class PlotArea
	{
		[XmlElement("lineChart")]
		public LineChart lineChart;

		[XmlElement("barChart")]
		public BarChart barChart;

		[XmlElement("pieChart")]
		public PieChart pieChart;

		[XmlElement("doughnutChart")]
		public PieChart doughnutChart;

		[XmlElement("areaChart")]
		public AreaChart areaChart;

		[XmlElement("valAx")]
		public ValueAxis valueAxis;
	}

#region PlotArea
	public class LineChart
	{
		[XmlElement("ser")]
		public List<LineChartSerial> serials;

		[XmlElement("dLbls")]
		public DataLabels labels;
	}

	public class BarChart
	{
		[XmlElement("barDir")]
		public ElementValue<string> barDir;

		[XmlElement("ser")]
		public List<BarChartSerial> serials;

		[XmlElement("dLbls")]
		public DataLabels labels;
	}

	public class PieChart
	{
		[XmlElement("ser")]
		public List<PieChartSerial> serials;

		[XmlElement("dLbls")]
		public DataLabels labels;
	}

	public class AreaChart
	{
		[XmlElement("ser")]
		public List<AreaChartSerial> serials;

		[XmlElement("dLbls")]
		public DataLabels labels;
	}

	internal interface IChartSerial
	{
		ChartText ChartLabel { get; }
		ChartDataValues Values { get; }
	}

	public class LineChartSerial : IChartSerial
	{
		[XmlElement("tx")]
		public ChartText chartLabel;

		[XmlElement("val")]
		public ChartDataValues values;

		[XmlIgnore]
		public ChartText ChartLabel { get { return this.chartLabel; } }

		[XmlIgnore]
		public ChartDataValues Values { get { return this.values; } }
	}

	public class BarChartSerial : IChartSerial
	{
		[XmlElement("tx")]
		public ChartText chartLabel;

		[XmlElement("val")]
		public ChartDataValues values;

		[XmlIgnore]
		public ChartText ChartLabel { get { return this.chartLabel; } }

		[XmlIgnore]
		public ChartDataValues Values { get { return this.values; } }
	}

	public class PieChartSerial : IChartSerial
	{
		[XmlElement("tx")]
		public ChartText chartLabel;

		[XmlElement("val")]
		public ChartDataValues values;

		[XmlIgnore]
		public ChartText ChartLabel { get { return this.chartLabel; } }

		[XmlIgnore]
		public ChartDataValues Values { get { return this.values; } }
	}

	public class AreaChartSerial : IChartSerial
	{
		[XmlElement("tx")]
		public ChartText chartLabel;

		[XmlElement("val")]
		public ChartDataValues values;

		[XmlIgnore]
		public ChartText ChartLabel { get { return this.chartLabel; } }

		[XmlIgnore]
		public ChartDataValues Values { get { return this.values; } }
	}

	public class ChartText
	{
		[XmlElement("strRef")]
		public StringReference strRef;
	}

	public class StringReference
	{
		[XmlElement("f")]
		public ElementText formula;

		[XmlElement("strCache")]
		public StringCache strCache;
	}

	public class StringCache
	{
		[XmlElement("ptCount")]
		public ElementValue<int> ptCount;

		[XmlElement("pt")]
		public List<NumericPoint> ptList;
	}

	public class CategoryAxis
	{
	}

	public class CategoryAxisData
	{
	}

	public class ValueAxis
	{
		[XmlElement("scaling")]
		public Scaling scaling;
	}

	public class ChartDataValues
	{
		[XmlElement("numRef")]
		public NumberReference numRef;
	}

	public class NumberReference
	{
		[XmlElement("f")]
		public ElementText formula;
	}

	public class NumericPoint
	{
		[XmlAttribute("idx")]
		public int index;

		[XmlElement("v")]
		public ElementText value;
	}

	public class DataLabels
	{
		[XmlElement("showLegendKey")]
		public ElementValue<int> ShowLegendKey;

		[XmlElement("showVal")]
		public ElementValue<int> showVal;

		[XmlElement("showCatName")]
		public ElementValue<int> showCatName;

		[XmlElement("showSerName")]
		public ElementValue<int> showSerName;

		[XmlElement("showPercent")]
		public ElementValue<int> showPercent;

		[XmlElement("showBubbleSize")]
		public ElementValue<int> showBubbleSize;
	}

	public class Scaling
	{
		// <c:orientation val="minMax"/>
		[XmlElement("orientation")]
		public ElementValue<string> orientation;

		[XmlElement("max")]
		public ElementValue<int> max;

		[XmlElement("min")]
		public ElementValue<int> min;
	}
#endregion // PlotArea

#region Legend
	public class Legend
	{
		[XmlElement("legendPos")]
		public ElementValue<string> legendPos;
	}
#endregion // Legend

#endregion // Chart

#region Theme
	[XmlRoot("theme", Namespace = OpenXMLNamespaces.Drawing______)]
	public class Theme
	{
		[XmlAttribute("name")]
		public string name;

		[XmlElement("themeElements")]
		public ThemeElements elements;
	}

	public class ThemeElements
	{
		[XmlElement("clrScheme")]
		public ClrScheme clrScheme;

		//[XmlArray("themeElements"), XmlArrayItem("fontScheme")]
		//public ThemeElement fontScheme;

		[XmlElement("fmtScheme")]
		public FormatScheme fmtScheme;


	}

#region Theme Colors
	[XmlInclude(typeof(CompColor))]
	[XmlInclude(typeof(GradientFill))]
	public class ClrScheme
	{
		[XmlElement("dk1")]
		public CompColor dk1;
		[XmlElement("lt1")]
		public CompColor lt1;
		[XmlElement("dk2")]
		public CompColor dk2;
		[XmlElement("lt2")]
		public CompColor lt2;
		[XmlElement("accent1")]
		public CompColor accent1;
		[XmlElement("accent2")]
		public CompColor accent2;
		[XmlElement("accent3")]
		public CompColor accent3;
		[XmlElement("accent4")]
		public CompColor accent4;
		[XmlElement("accent5")]
		public CompColor accent5;
		[XmlElement("accent6")]
		public CompColor accent6;
		[XmlElement("hlink")]
		public CompColor hlink;
		[XmlElement("folHlink")]
		public CompColor folHlink;

		//[XmlIgnore]
		//internal List<SolidColor> _colorPallate;
	}

	public class CompColor
	{
		[XmlElement("sysClr")]
		public CompColorVar sysColor;

		[XmlElement("srgbClr")]
		public CompColorVar srgbColor;

		[XmlElement("schemeClr")]
		public CompColorVar schemeColor;

		[XmlIgnore]
		internal SolidColor _solidColor;
	}

	public class CompColorVar
	{
		[XmlAttribute("val")]
		public string val;
		[XmlAttribute("lastClr")]
		public string lastClr;

		[XmlElement("lumMod")]
		public ElementValue<int> lumMod;
		[XmlElement("lumOff")]
		public ElementValue<int> lumOff;
		[XmlElement("satMod")]
		public ElementValue<int> satMod;
		[XmlElement("tint")]
		public ElementValue<int> tint;
		[XmlElement("shade")]
		public ElementValue<int> shade;
		[XmlElement("alpha")]
		public ElementValue<int> alpha;
	}

#endregion // Theme Colors

#region Drawing Format
	public class FormatScheme
	{
		[XmlArray("fillStyleLst")]
		[XmlArrayItem("solidFill", Type = typeof(CompColor))]
		[XmlArrayItem("gradFill", Type = typeof(GradientFill))]
		public List<object> fillStyles;

		[XmlArray("lnStyleLst"), XmlArrayItem("ln")]
		public List<LineStyle> lineStyles;

		[XmlArray("effectStyleLst"), XmlArrayItem("effectStyle")]
		public List<EffectStyle> effectStyles;

		[XmlArray("bgFillStyleLst")]
		[XmlArrayItem("solidFill", Type = typeof(CompColor))]
		[XmlArrayItem("gradFill", Type = typeof(GradientFill))]
		public List<object> bgFillStyleLst;
	}

	//public class SolidFill : AbstractFillStyle
	//{
	//	[XmlElement("schemeClr")]
	//	public CompColor schemeClr;

	//	[XmlElement("srgbClr")]
	//	public CompColorVar srgbClr;
	//}

#region Gradient Fill
	public class GradientFill
	{
		[XmlAttribute("rotWithShape")]
		public byte rotWithShape;

		[XmlArray("gsLst"), XmlArrayItem("gs")]
		public List<GradientStop> gsLst;
	}

	public class GradientStop : CompColor
	{
		[XmlAttribute("pos")]
		public int pos;
	}
#endregion // Gradient Fill

#region Line Style
	public class LineStyle
	{
		[XmlAttribute("w")]
		public int weight;
		[XmlAttribute("cap")]
		public string cap;
		[XmlAttribute("cmpd")]
		public string cmpd;
		[XmlAttribute("algn")]
		public string algn;

		[XmlElement("solidFill")]
		public CompColor solidFill;

		[XmlElement("prstDash")]
		public ElementValue<string> prstDash;

		[XmlElement("miter")]
		public string miter;
	}

	public class MiterLineJoin
	{
		[XmlAttribute("lim")]
		public int limit;
	}
#endregion // Line Style

#region Effect Style
	public class EffectStyle
	{
		[XmlArray("effectLst"), XmlArrayItem("outerShdw")]
		public List<OuterShadow> outerShadow;
	}

	// <a:outerShdw blurRad="57150" dist="19050" dir="5400000" algn="ctr" rotWithShape="0">
	public class OuterShadow
	{
		[XmlElement("srgbClr")]
		public CompColorVar srgbClr;
	}
#endregion // Effect Style

#endregion // Drawing Format
#endregion // Theme

#region Relationships
	[XmlRoot("Relationships", Namespace = OpenXMLNamespaces.Relationships)]
	public class Relationships
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
			new System.Xml.XmlQualifiedName[] {
				new System.Xml.XmlQualifiedName(string.Empty, OpenXMLNamespaces.Relationships),
			});

		[XmlElement("Relationship")]
		public List<Relationship> relations;

		[XmlIgnore]
		internal string _xmlTarget;

		public Relationships() { }

		internal Relationships(string _rsTarget)
		{
			_xmlTarget = _rsTarget;
		}
	}

	public class Relationship
	{
		[XmlAttribute("Id")]
		public string id;
		[XmlAttribute("Type")]
		public string type;
		[XmlAttribute("Target")]
		public string target;
	}

	public partial class OpenXMLFile
	{
		[XmlIgnore]
		internal Relationships _relationFile;

		[XmlIgnore]
		internal string _resId;
		[XmlIgnore]
		internal string _xmlTarget;
		[XmlIgnore]
		internal string _path;
		[XmlIgnore]
		internal string _rsTarget;
	}
#endregion // Relationships

#region Typed Element
	public class InnerTextElement
	{
		[XmlText]
		public string value;

		public InnerTextElement() { }

		public InnerTextElement(string value)
		{
			this.value = value;
		}
	}

	public class OpenXMLTypedElement<T>
	{
		[XmlAttribute("type", Namespace = OpenXMLNamespaces.NET_XSI______)]
		public string type;
	}

	public class OpenXMLDateTime : OpenXMLTypedElement<DateTime>
	{
		[XmlText]
		public string value;

		public OpenXMLDateTime() { }

		public OpenXMLDateTime(DateTime value)
		{
			this.type = "dcterms:W3CDTF";
			this.value = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
		}
	}
#endregion // Typed Element
}

#pragma warning restore 1591
