﻿using Microsoft.EntityFrameworkCore;
using PhimmoiClone.Areas.Cinema.Models;
using PhimmoiClone.Areas.Cinema.ViewModel;
using PhimmoiClone.Data;
using System.Linq;

namespace PhimmoiClone.Areas.Cinema.Repository.MovieRepo
{
    public class MovieRepo : IMovieRepo
    {
        private MyDbContext _ctx;

        public MovieRepo(MyDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<Movie>> GetAllAsync()
        {
            var movies = await _ctx.Movies.ToListAsync();
            return movies;
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            var movie = await _ctx
                .Movies
                .Include(m => m.MovieActors)!
                .ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieGenres)!
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);
            return movie;
        }


        public async Task CreateAsync(MovieViewModel movieViewModel)
        {
            Movie movie = new Movie
            {
                Name = movieViewModel.Name,
                Description = movieViewModel.Description,
                Publish = movieViewModel.Publish,
            };
            await _ctx.Movies.AddAsync(movie);
        }

        public async Task DeleteAsync(int id)
        {
            var movie = await GetByIdAsync(id);
            if (movie != null)
                _ctx.Movies.Remove(movie);
        }

        
        public async Task UpdateAsync(int id, MovieViewModel movieViewModel)
        {
            var movie = await GetByIdAsync(id);
            if (movie != null)
            {
                movie.Name = movie.Name;
                movie.Description = movie.Description;
                movie.Publish = movie.Publish;
            }
        }

        

        public List<int>? GetAllActorIds(Movie movie)
        {
            var actorIds = movie?.MovieActors?.Select(m => m.ActorId).ToList();
            return actorIds;
        }

        public List<int>? GetAllGenreIds(Movie movie)
        {
            var genreIds = movie.MovieGenres?.Select(m => m.GenreId).ToList();
            return genreIds;
        }

        public async Task<bool> SaveAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
                return true;
            return false;
        }

        public async Task<bool> AddToActorsAsync(Movie movie, List<int> actorsId)
        {
            // xóa tất cả actor cũ
            movie.MovieActors?.Clear();

            var actors = await _ctx.Actors.Where(a => actorsId.Contains(a.Id)).ToListAsync();

            var movieActors = new List<MovieActor>();
            foreach (var actor in actors)
            {
                var movieActor = new MovieActor { Movie = movie, Actor = actor };
                movieActors.Add(movieActor);
            }

            _ctx.MovieActor?.AddRange(movieActors);
            return await SaveAsync();
        }

        public async Task<bool> AddToGenresAsync(Movie movie, List<int> genresId)
        {
            // xóa tất cả actor cũ
            movie.MovieGenres?.Clear();

            var genres = await _ctx.Genres.Where(g => genresId.Contains(g.Id)).ToListAsync();

            var movieGenres = new List<MovieGenre>();
            foreach (var genre in genres)
            {
                var movieActor = new MovieGenre { Movie = movie, Genre = genre };
                movieGenres.Add(movieActor);
            }

            _ctx.MovieGenre?.AddRange(movieGenres);
            return await SaveAsync();
        }

        
    }
}