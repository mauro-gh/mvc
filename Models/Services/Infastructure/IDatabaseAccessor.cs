using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Services.Infastructure
{
    public interface IDatabaseAccessor
    {
        DataSet Query(FormattableString query);
    }
}