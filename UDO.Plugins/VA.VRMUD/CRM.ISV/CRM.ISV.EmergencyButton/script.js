

function CallCrmFunction() {
    try {
        var comm = new ActiveXObject("Communicator.UIAutomation");
        var contact = comm.GetContact("bryan.broome@va.gov", comm.MyServiceId);
        var window = comm.InstantMessage(contact);
    }
    catch (err) {
        alert("You are not logged into Office Communicator.");
    }
}
