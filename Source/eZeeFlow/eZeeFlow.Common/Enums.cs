using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZeeFlow.Common
{
    class Enums
    {
        public enum FileProcessedStatus
        {
            None = 0,
            Initiated = 1,
            InProgress = 2,
            Success = 3,
            Failed = 4,
        }
    }
}
