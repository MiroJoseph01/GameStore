using AutoMapper;
using GameStore.Web.Util.AutoMapperProfiles;

namespace GameStore.Web.Util.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IMapper Configure()
        {
            MapperConfiguration mappingConfiguration =
                new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new GameProfile());
                    mc.AddProfile(new CommentProfile());
                    mc.AddProfile(new GenreProfile());
                    mc.AddProfile(new PlatformProfile());
                    mc.AddProfile(new PublisherProfile());
                    mc.AddProfile(new OrderProfile());
                });

            return mappingConfiguration.CreateMapper();
        }
    }
}
