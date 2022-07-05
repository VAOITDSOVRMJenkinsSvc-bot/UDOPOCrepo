using System;

namespace VRM.Integration.UDO.ExamAndAppointments.Messages
{

    public class UDOcreateUDOExamResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public UDOcreateUDOExamsMultipleResponse[] UDOcreateUDOExamsAppointmentsInfo { get; set; }
    }

    public class UDOcreateUDOExamsMultipleResponse
    {
        public Guid newUDOcreateUDOExamAppointmentsId { get; set; }
    }
}