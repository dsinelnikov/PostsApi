using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsApi.Models;
using PostsApi.Services;

namespace PostsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/posts")]
    public class PostsController : Controller
    {
        private readonly PostsContext _postsContext;
        private readonly ISiteSettings _siteSettings;

        public PostsController(PostsContext postsContext, ISiteSettings siteSettings)
        {
            _postsContext = postsContext;
            _siteSettings = siteSettings;
        }

        [HttpGet("")]
        [HttpGet("page-{page}")]
        public IActionResult GetAll(int page = 0)
        {
            var pageSize = _siteSettings.PageSize;
            var posts = _postsContext.Posts
                .Skip(pageSize * page)
                .Take(pageSize)
                .ToArray();

            return new ObjectResult(posts);
        }

        [HttpGet("{id}", Name = "GetSingle")]
        public IActionResult Get(int id)
        {
            var post = _postsContext.Posts.FirstOrDefault(p => p.Id == id);

            if(post == null)
            {
                return NotFound();
            }

            return new ObjectResult(post);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _postsContext.Posts.Add(post);
            await _postsContext.SaveChangesAsync();

            return CreatedAtRoute("GetSingle", new { id = post.Id }, post);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _postsContext.Update(post);
            try
            {
                await _postsContext.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = new Post { Id = id };            
            _postsContext.Attach(post).State = EntityState.Deleted;

            try
            {
                await _postsContext.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }            
        }
    }
}