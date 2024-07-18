using Blog.Business.Dtos.ArticleDtos;
using Blog.Business.Dtos.HomeDtos;
using Blog.Business.Dtos.TagDtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Blog.WebUI.Models
{
    public class HomeViewModel
    {
        public List<HomeArticleDto> Articles { get; set; } = new List<HomeArticleDto>();
        public string SearchTerm { get; set; }
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public List<TagInfoDto> Tags { get; set; } = new List<TagInfoDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public SelectList TagsSelectList => new SelectList(Tags, "Id", "Name");
    }
}
