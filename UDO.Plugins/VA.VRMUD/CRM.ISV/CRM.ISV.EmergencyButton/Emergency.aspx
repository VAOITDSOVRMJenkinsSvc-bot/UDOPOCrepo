<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Emergency.aspx.cs" Inherits="VA.VRMUD.Web.Emergency" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<style type="text/css">
		#form1
		{
			width: 311px;
		}
	</style>
	<link rel="stylesheet" href="style.css" type="text/css" media="screen" />
</head>
<body>
<form id="form2" runat="server">
<div id="art-main">
	<div class="art-sheet">
		<div class="art-sheet-tl"></div>
		<div class="art-sheet-tr"></div>
		<div class="art-sheet-bl"></div>
		<div class="art-sheet-br"></div>
		<div class="art-sheet-tc"></div>
		<div class="art-sheet-bc"></div>
		<div class="art-sheet-cl"></div>
		<div class="art-sheet-cr"></div>
		<div class="art-sheet-cc"></div>
		<div class="art-sheet-body">
		<div class="art-content-layout">
			<div class="art-content-layout-row">
				<div class="art-layout-cell art-sidebar1">
					<div class="art-block">
						<div class="art-block-tl"></div>
						<div class="art-block-tr"></div>
						<div class="art-block-bl"></div>
						<div class="art-block-br"></div>
						<div class="art-block-tc"></div>
						<div class="art-block-bc"></div>
						<div class="art-block-cl"></div>
						<div class="art-block-cr"></div>
						<div class="art-block-cc"></div>
						<div class="art-block-body">
							<div class="art-blockheader">
								<div class="l"></div>
								<div class="r"></div>
								<h3 class="t">Suicide</h3>
							</div>
								<div class="art-blockcontent">
								<div class="art-blockcontent-body">
									<p>
									<a href="http://vbaw.vba.va.gov/bl/27/quality_training/pc/docs/suicide.doc" target="_blank">Suicide Guidance</a><br />
									<a href="http://www.suicidehotlines.com" target="_blank">Suicide Hotline</a><br />
									<a href="http://www.mentalhealth.va.gov" target="_blank">Mental Health Website</a><br />
									</p>                
								</div>
							</div>
							<div class="cleared"></div>
						</div>
						<div class="art-block-body">
							<div class="art-blockheader">
								<div class="l"></div>
								<div class="r"></div>
								<h3 class="t">Threats</h3>
							</div>
								<div class="art-blockcontent">
								<div class="art-blockcontent-body">
									<p>
									<a href="http://vbaw.vba.va.gov/bl/27/quality_training/pc/docs/threats.doc" target="_blank">Physical Threat of an employee</a><br />
									<a href="http://vbaw.vba.va.gov/bl/21/publicat/docs/bombthreat.pdf" target="_blank">Bomb Threat</a><br />
									</p>                
								</div>
							</div>
							<div class="cleared"></div>
						</div>
						<div class="art-block-body">
							<div class="art-blockheader">
								<div class="l"></div>
								<div class="r"></div>
								<h3 class="t">Employee Emergency</h3>
							</div>
							<div class="art-blockcontent">
								<div class="art-blockcontent-body">
									<p><asp:Label ID="Label1" runat="server" Text="Recepients (semicolon-separated)" Font-Size="X-Small"></asp:Label> </p>
									<p>
									<asp:TextBox ID="txtRecepients" runat="server" Width="330px" MaxLength="350" TextMode="MultiLine" Rows="3"  />
									</p>                
									<div class="cleared"></div>
								</div>
							</div>
							<div class="art-blockcontent">
								<div class="art-blockcontent-body">
									<p><asp:Label ID="lblEmText" runat="server" Text="Emergency Text" Font-Size="X-Small"></asp:Label> </p>
									<p>
									<asp:TextBox ID="txtEmergencyText" runat="server" Width="330px" MaxLength="350" TextMode="MultiLine" Rows="4"  />
									</p>                
									<div class="cleared"></div>
								</div>
							</div>
							<div class="art-blockcontent">
								<div class="art-blockcontent-body">
									<p>
									<asp:Button ID="EmployeeEmergency_Button"  runat="server" Text="Emergency" onclick="EmployeeEmergency_Button_Click" Width="200px"  />
									</p>                
									<p><asp:Label ID="lblMessage" runat="server" Font-Size="X-Small" Font-Bold="true"></asp:Label> </p>
									<div class="cleared"></div>
								</div>
							</div>
							<div class="cleared"></div>
						</div>                       
					</div>
				</div>
			</div>
		</div>
		</div>
	</div>
</div>
</form>
</body>
			
				
			
				
			
		
		
</html>
