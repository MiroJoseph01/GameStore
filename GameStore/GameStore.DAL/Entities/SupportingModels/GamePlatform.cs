namespace GameStore.DAL.Entities.SupportingModels
{
    public class GamePlatform
    {
        public string GameId { get; set; }

        public virtual Game Game { get; set; }

        public string PlatformId { get; set; }

        public Platform Platform { get; set; }
    }
}
