﻿using Microsoft.EntityFrameworkCore;
using PhimmoiClone.Areas.Cinema.Models;
using PhimmoiClone.Areas.Cinema.ViewModel;
using PhimmoiClone.Data;

namespace PhimmoiClone.Areas.Cinema.Repository.EpisodeRepo;

public class EpisodeRepo : IEpisodeRepo
{
    private readonly MyDbContext _ctx;

    public EpisodeRepo(MyDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Episode>> GetAllAsync()
    {
        var episodes = await _ctx.Episodes.ToListAsync();
        return episodes;
    }

    public async Task<Episode?> GetByIdAsync(int id)
    {
        var episode = await _ctx.Episodes
            .Include(e => e.Movie)!
            .FirstOrDefaultAsync(a => a.Id == id);
        return episode;
    }

    public async Task CreateAsync(EpisodeViewModel episodeViewModel)
    {
        var episode = new Episode()
        {
            MovieId = episodeViewModel.MovieId,
            EpNumber = episodeViewModel.EpNumber,
            EpString = episodeViewModel.EpString,
            LinkEmbed = episodeViewModel.LinkEmbed
        };
        await _ctx.Episodes.AddAsync(episode);
    }

    
    public async Task UpdateAsync(int id, EpisodeViewModel episodeViewModel)
    {
        var episode = await GetByIdAsync(id);
        if (episode != null)
        {
            episode.EpNumber = episodeViewModel.EpNumber;
            episode.EpString = episodeViewModel.EpString;
            episode.LinkEmbed = episodeViewModel.LinkEmbed;
        }
        
    }

    public async Task DeleteAsync(int id)
    {
        var episode = await GetByIdAsync(id);
        if (episode != null) _ctx.Episodes.Remove(episode);
    }

    public async Task<bool> SaveAsync()
    {
        return await _ctx.SaveChangesAsync() > 0;
    }
}