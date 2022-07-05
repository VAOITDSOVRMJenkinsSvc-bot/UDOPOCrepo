using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
//using Ionic;
//using Ionic.Zip;
//using Ionic.Zlib;

namespace VA.VRMUD.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String decompressedLetter = Request.Form["letter"];
            String letterTitle = Request.Form["title"];
            String fileExtension = Request.Form["fileExtension"];

            Response.AddHeader("Content-disposition", "attachment; filename=\"" + letterTitle + "." + fileExtension + "\"");
            Response.ContentType = "application/rtf";
            Response.Write(decompressedLetter);

            Response.End();
        }
    }
}