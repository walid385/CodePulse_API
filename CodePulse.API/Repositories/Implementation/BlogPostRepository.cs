using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await this.dbContext.BlogPosts.AddAsync(blogPost);
            await this.dbContext.SaveChangesAsync();
            return blogPost;
            
        }

        public async Task<BlogPost> DeleteAsync(Guid id)
        {
            var existingblogPost = await this.dbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);

            if (existingblogPost != null)
            {
                this.dbContext.BlogPosts.Remove(existingblogPost);
                await this.dbContext.SaveChangesAsync();
                return existingblogPost;
            };

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await this.dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<BlogPost> GetByIdAsync(Guid id)
        {
            return await this.dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost> GetByUrlHandleAsync(string urlHandle)
        {
            return await this.dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await this.dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlogPost != null)
            {
                this.dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
                existingBlogPost.Categories = blogPost.Categories;
                await this.dbContext.SaveChangesAsync();
                return blogPost;
            }

            return null;
        }
    }
}
