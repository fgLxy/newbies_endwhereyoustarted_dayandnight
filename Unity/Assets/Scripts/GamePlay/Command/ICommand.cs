using System.Threading.Tasks;

namespace SunAndMoon
{ 
    public interface ICommand
    {
        Task Execute();
    }
}
