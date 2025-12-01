using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Wpf.ViewModels
{
    public class NewsViewModel : ValidatableViewModelBase
    {
        private readonly IRssService _rssService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewsViewModel> _logger;

        private readonly List<string> _maritimeKeywords = new List<string>
        {
            "maritime", "shipping", "vessel", "port", "kapal", "pelabuhan", "pelayaran", 
            "cargo", "logistik", "shipyard", "offshore", "imo", "sail", "tanker", "container"
        };

        private List<NewsArticle> _allLoadedArticles = new List<NewsArticle>();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterAndDisplayArticles();
            }
        }

        public ObservableCollection<NewsArticle> Articles { get; } = new();

        public ICommand LoadNewsCommand { get; }

        public NewsViewModel(IRssService rssService, IConfiguration configuration, ILogger<NewsViewModel> logger)
        {
            _rssService = rssService;
            _configuration = configuration;
            _logger = logger;
            LoadNewsCommand = new AsyncRelayCommand(LoadNewsAsync);
            
            // Auto-load news on initialization
            _ = LoadNewsAsync(null);
        }

        private async Task LoadNewsAsync(object? parameter)
        {
            if (IsLoading) return;

            IsLoading = true;
            _allLoadedArticles.Clear();
            
            var feedUrls = _configuration.GetSection("RssFeeds").Get<List<string>>();
            if (feedUrls == null || !feedUrls.Any())
            {
                _logger.LogWarning("No RSS feeds configured in appsettings.json");
                IsLoading = false;
                return;
            }

            var tasks = feedUrls.Select(url => _rssService.FetchNewsAsync(url));
            var results = await Task.WhenAll(tasks);

            _allLoadedArticles = results.SelectMany(articles => articles)
                .Where(article => !string.IsNullOrWhiteSpace(article.Title) &&
                                  _maritimeKeywords.Any(keyword => 
                                      article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                      (article.Description != null && article.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                  ))
                .DistinctBy(a => a.Link)
                .OrderByDescending(a => a.PublishDate)
                .ToList();
            
            FilterAndDisplayArticles();

            IsLoading = false;
        }

                        private void FilterAndDisplayArticles()
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                Articles.Clear();

                                IEnumerable<NewsArticle> filteredArticles = _allLoadedArticles;

                                if (!string.IsNullOrWhiteSpace(SearchText))
                                {
                                    filteredArticles = filteredArticles.Where(a => {
                                        bool titleMatch = a.Title != null && a.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                                        bool descriptionMatch = a.Description != null && a.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                                        return titleMatch || descriptionMatch;
                                    });
                                }

                                foreach (var article in filteredArticles.Take(50))
                                {
                                    Articles.Add(article);
                                }
                            });
                        }    }
}