using GameStore.DAL.Interfaces;

namespace GameStore.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameStoreContext _context;

        public UnitOfWork(GameStoreContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
