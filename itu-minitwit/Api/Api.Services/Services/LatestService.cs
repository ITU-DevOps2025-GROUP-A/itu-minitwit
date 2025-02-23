﻿using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface ILatestService
{
    public Task<int> GetLatest();
    public Task UpdateLatest(int? latest);
}

public class LatestService(ILatestRepository latestRepository) : ILatestService
{
    public Task<int> GetLatest()
    {
        return latestRepository.GetLatest();
    }

    public async Task UpdateLatest(int? latest)
    {
        var newLatest = latest ?? -1; 
        if (newLatest != -1)
        {
            await latestRepository.UpdateLatest(newLatest);
        }
    }
}