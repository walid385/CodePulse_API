using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            //Convert from DTO to Domain
            var blogPost = new BlogPost
            {
                Author = request.Author,
                Title = request.Title,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                isVisible = request.isVisible,
                Categories = new List<Category>()

            };

            foreach(var categoryGuid in request.Categories) 
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory  != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            //Convert Domain to DTO

            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                isVisible = blogPost.isVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(response);


        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsnc()
        {
            var blogposts = await blogPostRepository.GetAllAsync();

            var response = new List<BlogPostDto>();
            {
                foreach (var blogpost in blogposts)
                {
                    response.Add(new BlogPostDto
                    {
                        Id = blogpost.Id,
                        Author = blogpost.Author,
                        Title = blogpost.Title,
                        Content = blogpost.Content,
                        UrlHandle= blogpost.UrlHandle,
                        FeaturedImageUrl= blogpost.FeaturedImageUrl,
                        isVisible= blogpost.isVisible,
                        PublishedDate= blogpost.PublishedDate,
                        ShortDescription= blogpost.ShortDescription,
                        Categories = blogpost.Categories.Select(x => new CategoryDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            UrlHandle = x.UrlHandle,
                        }).ToList()
                    });
                }

                return Ok(response);
            }

        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            var blogPost = await blogPostRepository.GetByIdAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Domain to DTO

            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                UrlHandle = blogPost.UrlHandle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                isVisible = blogPost.isVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrlhandle([FromRoute] string urlhandle)
        {
           var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlhandle);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Domain to DTO

            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                UrlHandle = blogPost.UrlHandle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                isVisible = blogPost.isVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPost([FromRoute] Guid id, UpdateBlogPostRequestDto request)
        {
            // DTO to Domain

            var blogPost = new BlogPost
            {
                Id = id,
                Author = request.Author,
                Title = request.Title,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                isVisible = request.isVisible,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
               var existingCategory = await categoryRepository.GetById(categoryGuid);

                if(existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }

            }

           var response = await blogPostRepository.UpdateAsync(blogPost);
            if (response == null)
            {
                return NotFound();
            }

            var updatedBlogPost = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                isVisible = blogPost.isVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(updatedBlogPost);

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id)
        {
            var blogPost = await blogPostRepository.DeleteAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                isVisible = blogPost.isVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
            };

            return Ok(response);
        }

    }


}
