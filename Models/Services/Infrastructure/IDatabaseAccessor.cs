using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        
        
        // per le query
        Task<DataSet> QueryAsync(FormattableString query, CancellationToken token = default);
        // per le query con risultato scalare
        Task<T> QueryScalarAsync<T>(FormattableString formattableQuery, CancellationToken token = default);
        // per le update
        Task<int> CommandAsync(FormattableString formattableCommand, CancellationToken token = default);

    }
}