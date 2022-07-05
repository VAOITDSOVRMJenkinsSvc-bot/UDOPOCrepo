using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class CallAttemptCombination
    {
        public int Dispostion { get; set; }
        public int Direction { get; set; }
        public int Attempts { get; set; }
        public string MessageString { get; set; }
    }

    public class Result
    {
        public int AllowedCallAttempt { get; set; }
        public string Message { get; set; }
    }

    public class CombinationOutput
    {
        static string outboundMessage = "You cannot create more than one call attempt for Left Voicemail, Do Not Contact Request, Unable to Contact/No Voicemail Required and Bad Phone Number in one day.";
        static string outinMessageSuccessful = "You cannot create more than one Successful Outbound and one Successful Inbound call attempt";
        static string inboundMessage = "No Inbound call attempts are allowed for disposition type Left Voicemail, Do Not Contact Request, Unable to Contact/No Voicemail Required and Bad Phone Number";

        static List<CallAttemptCombination> lstCombination = new List<CallAttemptCombination>()
        {
            //Successful Contact, Outbound
            new CallAttemptCombination() { Dispostion = 752280000, Direction = 752280000, Attempts = 1, MessageString = outinMessageSuccessful },
            //Successful Contact, Inbound
            new CallAttemptCombination() { Dispostion = 752280000, Direction = 752280001, Attempts = 1, MessageString = outinMessageSuccessful },
            //Left Voicemail, Outbound            
            new CallAttemptCombination() { Dispostion = 752280002, Direction = 752280000, Attempts = 1, MessageString = outboundMessage},
            //Left Voicemail, Inbound
            new CallAttemptCombination() { Dispostion = 752280002, Direction = 752280001, Attempts = 0, MessageString = inboundMessage},
            //Do Not Contact Request, Outbound
            new CallAttemptCombination() { Dispostion = 752280003, Direction = 752280000, Attempts = 1, MessageString = outboundMessage },
            //Do Not Contact Request, Inbound
            new CallAttemptCombination() { Dispostion = 752280003, Direction = 752280001, Attempts = 0, MessageString = inboundMessage},
            //Unable to Contact/No Voicemail Required, Outbound
            new CallAttemptCombination() { Dispostion = 752280001, Direction = 752280000, Attempts = 1, MessageString = outboundMessage},
            //Unable to Contact/No Voicemail Required, Inbound
            new CallAttemptCombination() { Dispostion = 752280001, Direction = 752280001, Attempts = 0, MessageString = inboundMessage},
            //Bad Phone Number, Outbound
            new CallAttemptCombination() { Dispostion = 752280005, Direction = 752280000, Attempts = 1, MessageString = outboundMessage},
            //Bad Phone Number, Inbound
            new CallAttemptCombination() { Dispostion = 752280005, Direction = 752280001, Attempts = 0, MessageString = inboundMessage}

        };
        
        public Result GetAttemptsMessages(int disposition, int direction)

        {
            Result res = new Result();
           
            foreach (var item in lstCombination)
            {
                if (item.Dispostion == disposition && item.Direction == direction)
                {
                    res.AllowedCallAttempt = item.Attempts;
                    res.Message = item.MessageString;
                };
            }

            return res;
        }
    }
}
