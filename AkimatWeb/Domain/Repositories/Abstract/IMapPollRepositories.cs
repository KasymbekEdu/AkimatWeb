using AkimatWeb.Domain.Models;

namespace AkimatWeb.Domain.Repositories.Abstract;

public interface IMapObjectsRepository
{
    IQueryable<MapObject> GetAll();
    Task<MapObject?> GetByIdAsync(int id);
    Task CreateAsync(MapObject obj);
    Task UpdateAsync(MapObject obj);
    Task DeleteAsync(int id);
}

public interface IPollsRepository
{
    IQueryable<Poll> GetAll();
    Task<Poll?> GetByIdAsync(int id);
    Task<Poll?> GetByIdWithOptionsAsync(int id);
    Task CreateAsync(Poll poll);
    Task UpdateAsync(Poll poll);
    Task DeleteAsync(int id);
    Task<bool> HasVotedAsync(int pollId, string fingerprint);
    Task VoteAsync(int pollId, List<int> optionIds, string fingerprint, string? userId);
    Task<Dictionary<int, int>> GetVoteCountsAsync(int pollId);
}