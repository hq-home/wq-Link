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
            ddlCat.SelectedIndexChanged += ddlCat_SelectedIndexChanged;

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

                if (ddlCat.Visible = ddlCat.DataSource != null)
                {
                    (ddlCat.DataSource as QuestCategoryList).Insert(0, new QuestCategory() { Name = "Not Selected" });
                    ddlCat.DataTextField = "Name";
                    ddlCat.DataValueField = "Id";
                    ddlCat.DataBind();
                }
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

            if (!string.IsNullOrEmpty(ddlCat2.SelectedValue) && !ddlCat.Visible)
                BindQuests(ddlCat2.SelectedValue);
		}


        public void ddlCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindQuests(ddlCat2.SelectedValue, ddlCat.SelectedValue);
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

        protected void BindQuests(string category2, string category = null)
        {
            var ql = Quest.LoadQuestsByCategory(category2, category);


			/*
            rpQLinks.DataSource = ql;
			rpQLinks.DataBind();
            */
            lvQuests.DataSource = ql;
            lvQuests.DataBind();

        }

		protected void btnWoWPedia_Click(object sender, EventArgs e)
		{
            var ql = Quest.LoadQuestsByCategory(ddlCat2.SelectedValue, ddlCat.SelectedValue);

            // ql.Sort()

			HtmlDocument htmlDoc = null;

			htmlDoc = LoadWoWPediaHtml(txtWoWPedia.Text, Helper.WowPediaDataPath);

			string wowPediaQuestPath = Path.Combine(Helper.WowPediaDataPath, GetLastUrlSegment(txtWoWPedia.Text));

			HtmlNodeCollection questLinkColl = htmlDoc.DocumentNode.SelectNodes("//div[@id='mw-content-text']//a[starts-with(@href, '/Quest')]");

			List<int> orderlist = new List<int>();

			if (!Directory.Exists(wowPediaQuestPath))
			{
				Directory.CreateDirectory(wowPediaQuestPath);			
			}


            List<string> loadedLinks = new List<string>();

			foreach (HtmlNode node in questLinkColl)
			{
				string qlink = node.GetAttributeValue("href", "");

                if (loadedLinks.Contains(qlink)) continue;
                else loadedLinks.Add(qlink);

				HtmlDocument hdQuest = LoadWoWPediaHtml("http://" + Helper.WowPediaHost + qlink, wowPediaQuestPath);
				HtmlNode nq = hdQuest.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']/ul[@class='elinks']/li[@class='wowhead']/a");
                int id = -1;

                if (nq == null)
                {
                    var mfq = ReadMalformedQuestDoc(hdQuest);
                    foreach (var q in mfq)
                    {
                        if (!orderlist.Contains(q.Id))
                            orderlist.Add(q.Id);
                    }
                    if(!mfq.Any())
                        _log.Debug("Can't XPath Wowpedia Malformed Quest Url: " + qlink);
                    continue;
                }
                
                string nqLink = nq.GetAttributeValue("href", "");
                if (string.IsNullOrEmpty(nqLink))
                {
                    _log.Debug("Can't read Wowpedia Quest Url: " + qlink);
                    continue;
                }
                string rQuest = "quest";
                nqLink = nqLink.Replace("/spell=", "/quest="); // some pages of Wowpedia are malformed
                string sID = Helper.GetIDByUrl(nqLink, ref rQuest);

                if (!int.TryParse(sID, out id))
                {
                    _log.Debug("Can't parse Wowpedia Quest ID: " + nqLink);
                    continue;
                }
                if (!orderlist.Contains(id))
                    orderlist.Add(id);
			}

            var qlOrdered = new List<Quest>();

            //ordering 
            foreach (int id in orderlist)
            { 
                var quest = ql.FirstOrDefault(q=> q.Id == id);
                if (quest == null)
                    quest = new Quest(id);
                else
                    ql.Remove(quest);

                quest.SourceType += "p";

                qlOrdered.Add(quest);
            }

            qlOrdered.AddRange(ql);

            lvQuests.DataSource = qlOrdered;
            lvQuests.DataBind();


			/*
            rpQLinks.DataSource = alist;
			rpQLinks.DataBind();
            */
			// <a href="http://wowpedia.org<%# XPath("@href") %>"><%# (Container.DataItem as HtmlAgilityPack.HtmlNode).InnerHtml %></a>
			// //div[@id='mw-content-text']//a
			//http://wowpedia.org/Mulgore_storyline
			



		}

        private List<Quest> ReadMalformedQuestDoc(HtmlDocument hdQuest)
        {
            string xpQuestName = "//head/meta[@name='og:title']"; //@content
            string xpTurnInName = "//div[@class='tooltip-content']/a[@title='End:']/following-sibling::a[1]";
            string xpTurnInNameAlt = "//div[@id='mw-content-text']/table[contains(@class,'infobox')]//td[@class='label' and text()='End']/following-sibling::td[1]//a[1]";

            HtmlNode nq = hdQuest.DocumentNode.SelectSingleNode(xpQuestName);
            string qName = nq.GetAttributeValue("content", null);
            qName = qName.Substring("Quest:".Length).TrimStart();

            nq = hdQuest.DocumentNode.SelectSingleNode(xpTurnInName);
            if (nq == null)
            {
                nq = hdQuest.DocumentNode.SelectSingleNode(xpTurnInNameAlt);
            }

            string qTurnInName = nq.InnerText;
            
            string whFilter = string.Format("na={0}", Server.UrlEncode(qName));

            var ql = Quest.LoadQuestsByFilter(whFilter);

            List<Quest> matchedQuests = new List<Quest>();

            foreach (Quest q in ql)
            {
                if (qName.Equals(q.Name, StringComparison.InvariantCultureIgnoreCase))
                { 
                    var dq = new Quest(q.Id);
                    if(dq.QuestTurnIn.Npc.Name.StartsWith(qTurnInName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matchedQuests.Add(q);
                    }
                }
            }

            return matchedQuests;
        }

		private HtmlDocument LoadWoWPediaHtml(string fileUrl, string saveTo)
		{
			string filepath = Path.Combine(saveTo, GetLastUrlSegment(fileUrl) + ".html");

			HtmlWeb hw = new HtmlWeb();
			HtmlDocument htmlDoc = null;

            if (!Directory.Exists(saveTo))
            {
                Directory.CreateDirectory(saveTo);
            }

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