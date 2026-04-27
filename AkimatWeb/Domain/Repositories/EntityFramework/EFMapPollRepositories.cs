using AkimatWeb.Domain.Models;
using AkimatWeb.Domain.Repositories.Abstract;
using AkimatWeb.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AkimatWeb.Domain.Repositories.EntityFramework;

public class EFMapObjectsRepository : IMapObjectsRepository
{
    private readonly AppDbContext _db;
    public EFMapObjectsRepository(AppDbContext db) => _db = db;

    public IQueryable<MapObject> GetAll() =>
        _db.MapObjects.OrderByDescending(m => m.CreatedAt);

    public async Task<MapObject?> GetByIdAsync(int id) =>
        await _db.MapObjects.FindAsync(id);

    public async Task CreateAsync(MapObject obj)
    {
        await _db.MapObjects.AddAsync(obj);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(MapObject obj)
    {
        _db.MapObjects.Update(obj);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.MapObjects.FindAsync(id);
        if (entity != null) { _db.MapObjects.Remove(entity); await _db.SaveChangesAsync(); }
    }
}

public class EFPollsRepository : IPollsRepository
{
    private readonly AppDbContext _db;
    public EFPollsRepository(AppDbContext db) => _db = db;

    public IQueryable<Poll> GetAll() =>
        _db.Polls.Include(p => p.Options)
                 .Include(p => p.Votes)
                 .OrderByDescending(p => p.CreatedAt);

    public async Task<Poll?> GetByIdAsync(int id) =>
        await _db.Polls.FindAsync(id);

    public async Task<Poll?> GetByIdWithOptionsAsync(int id) =>
        await _db.Polls
            .Include(p => p.Options.OrderBy(o => o.SortOrder))
            .Include(p => p.Votes)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreateAsync(Poll poll)
    {
        await _db.Polls.AddAsync(poll);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Poll poll)
    {
        _db.Polls.Update(poll);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Polls.FindAsync(id);
        if (entity != null) { _db.Polls.Remove(entity); await _db.SaveChangesAsync(); }
    }

    public async Task<bool> HasVotedAsync(int pollId, string fingerprint) =>
        await _db.PollVotes.AnyAsync(v =>
            v.PollId == pollId && v.VoterFingerprint == fingerprint);

    public async Task VoteAsync(int pollId, List<int> optionIds,
        string fingerprint, string? userId)
    {
        // Бұрынғы дауысты жою (қайта дауыс беруге болады)
        var old = _db.PollVotes.Where(v =>
            v.PollId == pollId && v.VoterFingerprint == fingerprint);
        _db.PollVotes.RemoveRange(old);

        foreach (var optId in optionIds)
        {
            await _db.PollVotes.AddAsync(new PollVote
            {
                PollId = pollId,
                OptionId = optId,
                VoterFingerprint = fingerprint,
                UserId = userId,
                VotedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task<Dictionary<int, int>> GetVoteCountsAsync(int pollId) =>
        await _db.PollVotes
            .Where(v => v.PollId == pollId)
            .GroupBy(v => v.OptionId)
            .Select(g => new { OptionId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OptionId, x => x.Count);
}