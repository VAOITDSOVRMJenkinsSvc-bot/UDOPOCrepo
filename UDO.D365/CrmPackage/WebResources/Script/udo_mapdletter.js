function mapDLetterOnLoad() {

    LookForAttachments();
}


function LookForAttachments() {

    debugger;
    $(".attachmentFileName").each(function (att) {
        debugger;
        var url = $(this).attr('href');
        if (url != undefined) {
            //window.open(url, "_blank");
        }
    });
}