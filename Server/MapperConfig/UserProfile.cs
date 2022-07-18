using AutoMapper;
using FilesInBlazor.Shared;

namespace FilesInBlazor.Server.MapperConfig
{
    internal class UserProfile : Profile
    {
        public UserProfile() : base()
        {
            CreateMap<Azure.Storage.Blobs.Models.BlobItem, BlobInfoDto>()
                .ForMember(dest =>
                    dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest =>
                    dest.Size,
                    opt => opt.MapFrom(src => src.Properties.ContentLength))
                .ForMember(dest =>
                    dest.LastModify,
                    opt =>
                    {
                        opt.PreCondition(src => src.Properties.LastModified.HasValue);
#pragma warning disable CS8629 // Nullable value type may be null.
                        opt.MapFrom(src => src.Properties.LastModified.Value.DateTime);
#pragma warning restore CS8629 // Nullable value type may be null.
                    })
                .ForMember(dest =>
                    dest.Size,
                    opt => opt.MapFrom(src => src.Properties.ContentLength))
                .ForMember(dest =>
                    dest.CreateOn,
                    opt =>
                    {
                        opt.PreCondition(src => src.Properties.CreatedOn.HasValue);
#pragma warning disable CS8629 // Nullable value type may be null.
                        opt.MapFrom(src => src.Properties.CreatedOn.Value.DateTime);
#pragma warning restore CS8629 // Nullable value type may be null.
                    })
                .ForMember(dest =>
                    dest.ContentType,
                    opt => opt.MapFrom(src => src.Properties.ContentType)
                );
            ;
        }
    }
}
