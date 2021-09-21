using AutoMapper;
using News.API.Model;

namespace News.API.Mapper
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<ArticleModel, NewsModel>()
                .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => src.Source.Name))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));
        }
    }
}