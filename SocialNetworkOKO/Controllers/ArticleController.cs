using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkOKO.DbContext;
using SocialNetworkOKO.Models;
using System;
using System.Security.Claims;

namespace SocialNetworkOKO.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("CreateArticle")]
        public IActionResult CreateArticle()
        {
            var model = new Article
            {
                ArticleTags = new List<ArticleTag>(),
                CreatedDate = DateTime.Now
            };
            ViewBag.Tags = _context.Tags.ToList(); // Загружаем существующие теги
            return View(model);
        }

        [HttpPost]
        [Route("CreateArticle")]
        public async Task<IActionResult> CreateArticle(Article model, string? newTagName, string[] ArticleTags)
        {
            if (ModelState.IsValid)
            {
                // Создаем новую статью
                var article = new Article
                {
                    Author = model.Author,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedDate = DateTime.UtcNow
                };

                // Сохраняем статью в базе данных
                _context.Articles.Add(article);
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных

                // Обработка тегов
                if (ArticleTags != null)
                {
                    foreach (var tagId in ArticleTags)
                    {
                        if (int.TryParse(tagId, out int tagIdInt))
                        {
                            var tag = _context.Tags.Find(tagIdInt);
                            if (tag != null)
                            {
                                article.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
                            }
                            else
                            {
                                ModelState.AddModelError("ArticleTags", $"Тег с ID {tagIdInt} не найден.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ArticleTags", $"Некорректный ID тега: {tagId}");
                        }
                    }
                }

                // Если был введен новый тег, добавляем его в базу данных
                if (!string.IsNullOrWhiteSpace(newTagName))
                {
                    var existingTag = _context.Tags.FirstOrDefault(t => t.Name == newTagName);
                    if (existingTag == null)
                    {
                        var newTag = new Tag { Name = newTagName };
                        _context.Tags.Add(newTag);
                        await _context.SaveChangesAsync(); // Сохраняем новый тег в базе данных

                        // Связываем новый тег со статьей
                        article.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = newTag.Id });
                    }
                    else
                    {
                        // Если тег уже существует, связываем его со статьей
                        article.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = existingTag.Id });
                    }
                }

                // Сохраняем изменения статьи с тегами
                await _context.SaveChangesAsync();

                return RedirectToAction("Articles"); 
            }

            ViewBag.Tags = _context.Tags.ToList(); // Загружаем теги снова в случае ошибки
            return View(model);
        }

        // Метод для получения списка всех статей
        [HttpPost]
        [Route("Articles")]
        public async Task<IActionResult> Articles()
        {
            var articles = await _context.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .ToListAsync();
            return View(articles);
        }

        // Метод для получения деталей конкретной статьи
        [HttpGet]
        [Route("Articles/DetailsArticle/{id:int}")]
        public async Task<IActionResult> DetailsArticle(int id)
        {
            // Логируем запрос
            Console.WriteLine($"Запрос на получение статьи с ID: {id}");

            // Получаем конкретную статью с тегами асинхронно
            var article = await GetArticleByIdAsync(id);

            // Проверяем, найдена ли статья
            if (article == null)
            {
                Console.WriteLine($"Статья с ID: {id} не найдена.");
                return NotFound(); // Если статья не найдена, возвращаем 404
            }

            var relatedArticles = await _context.Articles
            .Where(a => a.ArticleTags.Any(at => article.ArticleTags.Select(t => t.TagId).Contains(at.TagId)) && a.Id != id)
            .ToListAsync();

            article.RelatedArticles = relatedArticles;

            return View(article); // Возвращаем представление с найденной статьей
        }

        // Асинхронный метод для получения статьи по ID
        private async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        //Метод добавления комментариев к статье
        [HttpPost]
        public async Task<IActionResult> AddComment(int articleId, string text, string author)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(author))
            {
                ModelState.AddModelError("", "Имя и комментарий не могут быть пустыми.");
                return RedirectToAction("Details", new { id = articleId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем строковый ID текущего пользователя

            var comment = new Comment
            {
                Text = text,
                Author = author,
                Date = DateTime.Now,
                ArticleId = articleId,
                UserId = userId // Используем строковый ID пользователя
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsArticle", new { id = articleId });
        }

        [HttpGet]
        [Route("SearchArticle")]
        public async Task<IActionResult> SearchArticle(string searchString, string tagString)
        {
            var articles = _context.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag) // Загружаем теги для фильтрации
                .AsQueryable(); // Преобразуем в IQueryable для дальнейшей фильтрации

            // Фильтрация по названию статьи
            if (!string.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(a => a.Title.Contains(searchString));
            }

            // Фильтрация по тегам
            if (!string.IsNullOrEmpty(tagString))
            {
                articles = articles.Where(a => a.ArticleTags.Any(at => at.Tag.Name.Contains(tagString)));
            }

            var result = await articles.ToListAsync(); // Выполняем запрос и получаем результат

            return View(result); // Возвращаем отфильтрованные статьи в представление
        }
    }
}

