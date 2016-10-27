using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Wowhead;

namespace Web
{
	public partial class _Default : Page
	{
		#region [ Constants ]

		private QuestCategoryList _category = null;

		public QuestCategoryList Category
		{
			get
			{
				if (_category == null)
				{
					_category = new QuestCategoryList();
					_category.Load();
				}
				return _category;
			}
		}

		#endregion

        Logger _log = null;

		protected void Page_Init(object sender, EventArgs e)
		{
			btnWoWPedia.Click += btnWoWPedia_Click;

            _log = new Logger(Path.Combine(Server.MapPath("~/Logs"), DateTime.Now.ToString("yyyy-MM-dd") + ".txt"));

            Helper.RegisterLogger(LogDebug);
		}

        protected void LogDebug(object str, EventArgs e)
        {
            if (_log != null) _log.Debug((string)str);
        }
		protected void Page_Load(object sender, EventArgs e)
		{
			ddlCat2.PreRender += ddlCat_PreRender;
			ddlCat.PreRender += ddlCat_PreRender;
			ddlCat2.SelectedIndexChanged += ddlCat2_SelectedIndexChanged;

			if(!IsPostBack)
			{
				InitDDL_Category2();
				InitDDL_Category();
			}
			
		}

		private void InitDDL_Category()
		{
			if(ddlCat.Visible = !string.IsNullOrEmpty(ddlCat2.SelectedValue))
			{
				int idx = int.Parse(ddlCat2.SelectedValue);

				ddlCat.DataSource = Category.First(c => c.Id.HasValue && c.Id.Value == idx).Categories;
				(ddlCat.DataSource as QuestCategoryList).Insert(0, new QuestCategory() { Name = "Not Selected" });
				ddlCat.DataTextField = "Name";
				ddlCat.DataValueField = "Id";
				ddlCat.DataBind();

			}

		}

		private void InitDDL_Category2()
		{
			Category.Insert(0, new QuestCategory() { Name = "Not Selected" });

			ddlCat2.DataSource = Category;
			ddlCat2.DataTextField = "Name";
			ddlCat2.DataValueField = "Id";
			ddlCat2.DataBind();
		}

		protected void ddlCat2_SelectedIndexChanged(object sender, EventArgs e)
		{
			InitDDL_Category();
		}

		protected void ddlCat_PreRender(object sender, EventArgs e)
		{
			foreach (ListItem d in ((DropDownList)sender).Items)
			{
				if (string.IsNullOrEmpty(d.Value) && d.Text != "Not Selected")
				{
					d.Attributes["disabled"] = "disabled";
				}

			}
		}


		protected void btnWoWPedia_Click(object sender, EventArgs e)
		{
			
			HtmlDocument htmlDoc = null;

			htmlDoc = LoadWoWPediaHtml(txtWoWPedia.Text, Helper.WowPediaDataPath);

			string wowPediaQuestPath = Path.Combine(Helper.WowPediaDataPath, GetLastUrlSegment(txtWoWPedia.Text));

			HtmlNodeCollection questLinkColl = htmlDoc.DocumentNode.SelectNodes("//div[@id='mw-content-text']//a[starts-with(@href, '/Quest')]");

			List<HtmlAnchor> alist = new List<HtmlAnchor>();

			if (!Directory.Exists(wowPediaQuestPath))
			{
				Directory.CreateDirectory(wowPediaQuestPath);			
			}

			if (!Directory.Exists(Path.Combine(Helper.WowHeadDataPath, "Quest")))
			{
				Directory.CreateDirectory(Path.Combine(Helper.WowHeadDataPath, "Quest"));
			}

			foreach (HtmlNode node in questLinkColl)
			{
				HtmlAnchor a = new HtmlAnchor();

				string qlink = node.GetAttributeValue("href", "");
				a.Title = qlink;
				a.InnerText = node.InnerText;
				HtmlDocument hdQuest = LoadWoWPediaHtml("http://" + Helper.WowPediaHost + qlink, wowPediaQuestPath);
				HtmlNode nq = hdQuest.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']/ul[@class='elinks']/li[@class='wowhead']/a");

				string nqLink = nq.GetAttributeValue("href","");

				string rQuest = "quest";
				string sID = Helper.GetIDByUrl(nqLink, ref rQuest);

				int id = int.Parse(sID);

				a.HRef = "http://" + Helper.WowHeadHost + "/quest=" + sID;

				Quest whq = new Quest(id);


				alist.Add(a);
				
			}

			rpQLinks.DataSource = alist;
			rpQLinks.DataBind();
			// <a href="http://wowpedia.org<%# XPath("@href") %>"><%# (Container.DataItem as HtmlAgilityPack.HtmlNode).InnerHtml %></a>
			// //div[@id='mw-content-text']//a
			//http://wowpedia.org/Mulgore_storyline
			



		}

		private HtmlDocument LoadWoWPediaHtml(string fileUrl, string saveTo)
		{
			string filepath = Path.Combine(saveTo, GetLastUrlSegment(fileUrl) + ".html");

			HtmlWeb hw = new HtmlWeb();
			HtmlDocument htmlDoc = null;

			if (!File.Exists(filepath))
			{
				htmlDoc = hw.Load(fileUrl);
				htmlDoc.Save(filepath, Encoding.UTF8);
			}
			else
			{
				htmlDoc = new HtmlDocument();
				htmlDoc.Load(filepath);
			}
			return htmlDoc;
		}

		public static string GetLastUrlSegment(string fileUrl, bool withExtension = false)
		{
			int idx = fileUrl.LastIndexOf('/');
			string name = fileUrl.Substring(idx + 1)
				.Replace(':', '-')
				.Replace('#', '-')
				.Replace('=', '-')
				.Replace('&', '-')
				.Replace(';', '-')
				.Replace('%', '-')
				.Replace('+', '-')
				.Replace('/', '-')
				;
			return withExtension ? name : Path.GetFileNameWithoutExtension(name);
		}
	}
}