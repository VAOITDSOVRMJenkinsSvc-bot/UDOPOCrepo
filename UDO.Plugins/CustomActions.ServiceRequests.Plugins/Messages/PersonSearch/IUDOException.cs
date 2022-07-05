using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.Core.Interfaces
{
    public interface IUDOException
    {
        bool ExceptionOccurred { get; set; }
        string ExceptionMessage { get; set; }
    }
}
