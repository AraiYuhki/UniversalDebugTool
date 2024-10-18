using Cysharp.Threading.Tasks;
using System.Threading;

public interface IControllable
{
    void Up();
    void Down();
    void Left();
    void Right();
    void Cancel();
    void Submit();
    UniTask OpenAsync(CancellationToken token);
    UniTask CloseAsync(CancellationToken token);
}
