using Api.DataAccess.Models;
using Api.Services;
using Api.Services.LogDecorator;
using Api.Services.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess.Repositories;

public class LatestRepository(MinitwitDbContext dbContext) : ILatestRepository
{
    [LogTime]
    [LogReturnValue]
    public async Task<int> GetLatest()
    {
        var latestProcessedCommandId = await dbContext.LatestProcessedSimActions
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (latestProcessedCommandId == null) return -1;
        return latestProcessedCommandId.Latest;
    }
    
    [LogTime]
    [LogMethodParameters]
    public async Task UpdateLatest(int latest)
    {
        var latestObj = await dbContext.LatestProcessedSimActions.FirstOrDefaultAsync();
        if (latestObj != null)
        {
            latestObj.Latest = latest; 
            dbContext.LatestProcessedSimActions.Update(latestObj);
        }
        else
        {
            latestObj = new LatestProcessedSimAction { Latest = latest };
            await dbContext.LatestProcessedSimActions.AddAsync(latestObj);
        }

        await dbContext.SaveChangesAsync();
    }
}