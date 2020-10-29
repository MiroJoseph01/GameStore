namespace GameStore.DAL.Entities.SupportingModels
{
    public class GameGenre
    {
        public string GameId { get; set; }

        public virtual Game Game { get; set; }

        public string GenreId { get; set; }

        public Genre Genre { get; set; }
    }
}
