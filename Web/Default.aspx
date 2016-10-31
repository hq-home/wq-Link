<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Modify this template to jump-start your ASP.NET application.</h2>
            </hgroup>
            <p>
                To learn more about ASP.NET, visit <a href="http://asp.net" title="ASP.NET Website">http://asp.net</a>.
                The page features <mark>videos, tutorials, and samples</mark> to help you get the most from ASP.NET.
                If you have any questions about ASP.NET visit
                <a href="http://forums.asp.net/18.aspx" title="ASP.NET Forum">our forums</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
	<h3>Wowhead Navigation</h3>
    <span><asp:DropDownList ID="ddlCat2" runat="server" AutoPostBack="True" Width="200"></asp:DropDownList>&nbsp;<asp:DropDownList ID="ddlCat" runat="server" AutoPostBack="True" Width="200"></asp:DropDownList></span>
	<br />
    <h3>Wowpedia Link</h3>
    <span><asp:TextBox ID="txtWoWPedia" runat="server"></asp:TextBox> <asp:Button ID="btnWoWPedia" runat="server" Text="Refresh" /></span>
	
	
	<ol class="round">
	<asp:Repeater ID="rpQLinks" runat="server">
		<ItemTemplate>
			 <li><a style="" href='#' title="<%# Eval("Name") %>"><%# (Container.ItemIndex+1).ToString("0000") %> <span style="font-weight: bold; text-shadow: 1px 1px 2px black, 0 0 1em red; color:azure; width:16px; height:16px; display: inline-block; background:url('Images/wowhead.ico');" ><%# Eval("SourceType") %></span>
				 <span style="width:16px; height:16px; display: inline-block;" ></span>
				 [<%# Eval("Id") %>] <%# Eval("Name") %> [<%# Eval("RequiredLevel") %>-<%# Eval("Level") %>]</a></li>
		</ItemTemplate>

	</asp:Repeater>
<script type="text/javascript">
    jQuery.fn.sortElements = (function () {

        var sort = [].sort;

        return function (comparator, getSortable) {

            getSortable = getSortable || function () { return this; };

            var placements = this.map(function () {

                var sortElement = getSortable.call(this),
                    parentNode = sortElement.parentNode,

                    // Since the element itself will change position, we have
                    // to have some way of storing it's original position in
                    // the DOM. The easiest way is to have a 'flag' node:
                    nextSibling = parentNode.insertBefore(
                        document.createTextNode(''),
                        sortElement.nextSibling
                    );

                return function () {

                    if (parentNode === this) {
                        throw new Error(
                            "You can't sort elements if any one is a descendant of another."
                        );
                    }

                    // Insert before flag:
                    parentNode.insertBefore(this, nextSibling);
                    // Remove flag:
                    parentNode.removeChild(nextSibling);

                };

            });

            return sort.call(this, comparator).each(function (i) {
                placements[i].call(getSortable.call(this));
            });

        };

    })();



    $(document).ready(function () {
        $('.qlist>tbody').sortable();

        var qtbl = $('.qlist');

        $('.idx')
        .each(function (idx, el)
            {
            $(el).html($(el).html() + '<span class="s_mark">&nbsp;↕</span>')
            }
        )
        //.wrapInner('<span>|</span>')
        .each(function () {

            var th = $(this),
                thIndex = th.index(),
                inverse = false;

            th.click(function () {

                $(this).text()

                qtbl.find('td').filter(function (idx) {
                    
                    return $(this).index() === thIndex;

                })
                .sortElements(function (a, b) {
                    
                    return $(a).text() - $([b]).text() > 0 ?
                        //$.text([a]) > $.text([b]) ?
                        inverse ? -1 : 1
                        : inverse ? 1 : -1;

                }, function () {

                    // parentNode is the element we want to move
                    return this.parentNode;

                });

                inverse = !inverse;

                var sdir = inverse ? '↑' : '↓';
                th.find('.s_mark').replaceWith('<span class="s_mark">&nbsp;' + sdir + '</span>');

            });

        });
    });

</script>
<style>
    .qlist-wrapper{
        height:600px; width: 500px;
        /*border:solid 1px Red;*/
        overflow:auto;

    }
    .qlist th {
        background-color: darkgray;
        border:solid 1px White;
        color:white;
        font-weight:normal;
        padding:1px 3px;
    }

    .qlist th:hover {
       color:green;
       cursor:pointer;
    }

    .qrow:hover {
        background-color:lightgray;
    }
    .qcell {
        border:solid 1px gray;
        padding:1px 3px;
    }
    .cId {
        text-align: right;
        width:10%;
    }
    .cLvl {
        text-align: right;
    }

    .fl-wh {
        font-weight: bold; 
        text-shadow: 1px 1px 2px black, 0 0 1em red; 
        color:azure; 
        width:16px; height:16px; 
        display: inline-block; 
        background:url('Images/wowhead.ico');
    }
    .fl-wp {
        width:16px; height:16px; 
        display: inline-block; 
        background:url('Images/wowpedia.ico');
    }
</style>
    <div class="qlist-wrapper">
	<asp:ListView ID="lvQuests" runat="server">
        <LayoutTemplate>
            <table class="qlist">
                 <thead>
                  <tr>
                    <th scope="col" class="cId idx">#</th>
                    <th scope="col">Flags</th>
                    <th scope="col">Id</th>
                    <th scope="col" class="idx">Req.Lvl</th>
                    <th scope="col" class="idx">Level</th>
                    <th scope="col">Name</th>
                  </tr>
                </thead>
                <tbody>
                      <asp:PlaceHolder ID="itemPlaceholder" runat="server" />  
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr class="qrow">
                <td class="qcell cId"><%# (Container.DisplayIndex+1) %></td>
                <td class="qcell"><span class="fl-wh" ><%# ((string)Eval("SourceType")).TrimEnd('p') %></span>
				 <span class="<%# (((string)Eval("SourceType")).Contains('p') ? "fl-wp" : "")  %>" ></span></td>
                <td class="qcell cId"><%# Eval("Id") %></td>
                <td class="qcell cLvl"><%# Eval("RequiredLevel") %></td>
                <td class="qcell cLvl"><%# Eval("Level") %></td>
                <td class="qcell"><%# Eval("Name") %></td>
            </tr>
        </ItemTemplate>
	</asp:ListView>
	</div>
        <li class="one">
            <h5>Getting Started</h5>
            ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245146">Learn more…</a>
        </li>
        <li class="two">
            <h5>Add NuGet packages and jump-start your coding</h5>
            NuGet makes it easy to install and update free libraries and tools.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245147">Learn more…</a>
        </li>
        <li class="three">
            <h5>Find Web Hosting</h5>
            You can easily find a web hosting company that offers the right mix of features and price for your applications.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245143">Learn more…</a>
        </li>
    </ol>
</asp:Content>
