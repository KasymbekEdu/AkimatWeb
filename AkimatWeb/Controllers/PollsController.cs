using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AkimatWeb.Controllers;

public class PollsController : Controller
{
    private readonly DataManager _data;
    public PollsController(DataManager data) => _data = data;

    public IActionResult Index()
    {
        var polls = _data.Polls.GetAll()
            .Where(p => p.IsActive)
            .ToList();
        return View(polls);
    }

    public async Task<IActionResult> Details(int id)
    {
        var poll = await _data.Polls.GetByIdWithOptionsAsync(id);
        if (poll is null || !poll.IsActive) return NotFound();

        var fingerprint = GetFingerprint();
        ViewBag.HasVoted = await _data.Polls.HasVotedAsync(id, fingerprint);
        ViewBag.VoteCounts = await _data.Polls.GetVoteCountsAsync(id);
        ViewBag.TotalVotes = poll.Votes
            .Select(v => v.VoterFingerprint).Distinct().Count();

        return View(poll);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int pollId, List<int> optionIds)
    {
        if (!optionIds.Any())
        {
            TempData["VoteError"] = "Кем дегенде бір нұсқаны таңдаңыз";
            return RedirectToAction(nameof(Details), new { id = pollId });
        }

        var fingerprint = GetFingerprint();
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        await _data.Polls.VoteAsync(pollId, optionIds, fingerprint, userId);

        TempData["VoteSuccess"] = "Сіздің дауысыңыз қабылданды!";
        return RedirectToAction(nameof(Details), new { id = pollId });
    }

    // IP + UserAgent негізінде анонимді хэш жасайды
    private string GetFingerprint()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = Request.Headers.UserAgent.ToString();
        var raw = $"{ip}|{ua}";
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(raw)))[..16];
    }
}