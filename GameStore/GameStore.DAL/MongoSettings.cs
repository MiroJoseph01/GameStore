namespace GameStore.DAL
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }

        public string Name { get; set; }

        public bool RewriteIds { get; set; }
    }
}
