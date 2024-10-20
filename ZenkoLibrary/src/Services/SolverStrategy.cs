using Zenko.Entities;

namespace Zenko.Services
{
    public abstract class SolverStrategy
    {
        public abstract void OnInitialize(TileSet initialTileSet);
    }
}