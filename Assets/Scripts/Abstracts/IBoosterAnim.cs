using TileMatchGame.Manager;

namespace TileMatchGame.Abstracts
{
    public interface IBoosterAnim
    {
        void ExecuteSound();
        void ExecuteAnim(Cell boosterCell, LevelManager level);
    }
}