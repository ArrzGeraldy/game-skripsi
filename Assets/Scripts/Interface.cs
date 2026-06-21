using System.Collections;

public interface PuzzleHandlerI
{
    public void CheckAnswer();
    public IEnumerator WrongAnswer();
    public IEnumerator CorrectAnswer();
    public void OnPuzzleActivated();
    public void OnPuzzleDeactivated();
    public void OnStartSimulation();
}