// JavaScript Document
if (!$.toXmlString) {
	$.fn.extend(
	{
		toXmlString : function(node)
		{
			node = $(node || this);
			var map = {
			  "&": "&amp;",
			  "<": "&lt;",
			  ">": "&gt;",
			  '"': '&quot;',
			  "'": '&#39;',
			  "/": '&#x2F;'
			};

			function escape(o) {
				return o.toString().replace(/[&<>"'\/]/g, function (s) {return map[s];});
			}
		
			var text = "<" +  node.prop("nodeName");
			
			if (node[0].attributes && node[0].attributes!=null)
				$.each(node[0].attributes, function( i, a ) { 
					text+=" "+a.name+'="'+escape(a.value)+'"';
				});
			
			text+=">";
			
			node.children().each(function(i,c) {
				text+=$(c).toXmlString();
			});
			
			text+="</"+node.prop("nodeName")+">";
	  
			return text;
		}
	});
}